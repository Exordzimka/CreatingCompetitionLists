using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using systemColor = System.Drawing.Color;
using Color = Google.Apis.Sheets.v4.Data.Color;

namespace CreatingCompetitionLists.Services
{
    public class UpdateSpreadsheetService
    {
        private UserCredential _userCredential;
        private readonly string[] _scopes = {SheetsService.Scope.Drive, SheetsService.Scope.DriveFile};
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        int startRow = 17;
        int startCountColumns = 12;

        private static readonly Border simpleBorder = new Border
            {Style = "SOLID", Color = new Color {Blue = 0, Green = 0, Red = 0}};

        private static readonly Borders borders = new Borders
            {Top = simpleBorder, Bottom = simpleBorder, Left = simpleBorder, Right = simpleBorder};

        private static readonly CellFormat standartFormat = new CellFormat
        {
            BackgroundColor = new Color {Blue = 1, Green = 1, Red = 1},
            TextFormat = new TextFormat
            {
                Bold = false, Italic = false, Strikethrough = false, Underline = false,
                ForegroundColor = new Color {Blue = 0, Green = 0, Red = 0}
            },
            Borders = borders
        };

        private static readonly Color googleGreen = new Color
        {
            Blue = (float) systemColor.Lime.B / 255, Red = (float) systemColor.Lime.R / 255,
            Green = (float) systemColor.Lime.G / 255
        };

        private static readonly Color googleYellow = new Color
        {
            Blue = (float) systemColor.Yellow.B / 255, Green = (float) systemColor.Yellow.G / 255,
            Red = (float) systemColor.Yellow.R / 255
        };

        private static readonly Color googleGray = new Color
        {
            Blue = (float) systemColor.DimGray.B / 255, Green = (float) systemColor.DimGray.G / 255,
            Red = (float) systemColor.DimGray.R / 255
        };

        private static readonly CellFormat greenFormat = new CellFormat
        {
            BackgroundColor = googleGreen,
            Borders = borders
        };

        private static readonly CellFormat yellowFormat = new CellFormat
        {
            BackgroundColor = googleYellow,
            Borders = new Borders
            {
                Bottom = new Border
                {
                    Color = new Color
                    {
                        Red = systemColor.Red.R / 255, Green = systemColor.Red.G / 255,
                        Blue = systemColor.Red.B / 255
                    },
                    Style = "SOLID_THICK"
                },
                Top = simpleBorder,
                Left = simpleBorder,
                Right = simpleBorder
            }
        };

        private static readonly CellFormat grayFormat = new CellFormat
        {
            BackgroundColor = googleGray,
            TextFormat = new TextFormat {Strikethrough = true},
            Borders = borders
        };

        private static readonly CellFormat grayTextFormat = new CellFormat
        {
            TextFormat = new TextFormat
            {
                ForegroundColor = new Color
                {
                    Blue = (float) systemColor.Gray.B / 255, Green = (float) systemColor.Gray.G / 255,
                    Red = (float) systemColor.Gray.R / 255
                }
            },
            Borders = borders
        };

        string agreementColumn = "H";
        string documentColumn = "G";
        string sumEgeColumn = "F";


        public async void HighlightOriginals(string spreadsheetId, ClaimsPrincipal user)
        {
            var spreadsheet = Service(user).Spreadsheets.Get(spreadsheetId).Execute();
            var batchFormulaGet = Service(user).Spreadsheets.Values.BatchGet(spreadsheetId);
            batchFormulaGet.DateTimeRenderOption = SpreadsheetsResource.ValuesResource.BatchGetRequest
                .DateTimeRenderOptionEnum.FORMATTEDSTRING;
            batchFormulaGet.ValueRenderOption =
                SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.FORMULA;
            batchFormulaGet.MajorDimension =
                SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.ROWS;
            var batchValueGet = Service(user).Spreadsheets.Values.BatchGet(spreadsheetId);
            batchValueGet.DateTimeRenderOption = SpreadsheetsResource.ValuesResource.BatchGetRequest
                .DateTimeRenderOptionEnum.FORMATTEDSTRING;
            batchValueGet.ValueRenderOption =
                SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.UNFORMATTEDVALUE;
            batchValueGet.MajorDimension =
                SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.ROWS;
            var placesSheet = spreadsheet.Sheets.FirstOrDefault(x =>
                x.Properties.Title.Equals("число мест", StringComparison.OrdinalIgnoreCase));
            foreach (var sheet in spreadsheet.Sheets)
            {
                await updateSheet(sheet, batchFormulaGet, batchValueGet, spreadsheet, user);
            }
        }

        private async Task updateSheet(Sheet sheet, SpreadsheetsResource.ValuesResource.BatchGetRequest batchFormulaGet,
            SpreadsheetsResource.ValuesResource.BatchGetRequest batchValueGet, Spreadsheet spreadsheet,
            ClaimsPrincipal user)
        {
            if (sheet.Properties.Title.Equals("База", StringComparison.OrdinalIgnoreCase) ||
                sheet.Properties.Title.Equals("Число мест", StringComparison.OrdinalIgnoreCase)) return;
            batchFormulaGet.Ranges = new Repeatable<string>(new[] {sheet.Properties.Title + "!A16:Z16"});
            var headValues = batchFormulaGet.Execute().ValueRanges[0].Values;
            var countOfDirection = getCountDirections(headValues[0]);
            batchFormulaGet.Ranges = new Repeatable<string>(new[]
                {sheet.Properties.Title + $"!A17:{GetLetterByNumber(startCountColumns + countOfDirection)}"});
            var formulaValues = batchFormulaGet.Execute().ValueRanges[0].Values;
            batchValueGet.Ranges = new Repeatable<string>(new[]
                {sheet.Properties.Title + $"!A17:{GetLetterByNumber(startCountColumns + countOfDirection)}"});
            var valueValues = batchValueGet.Execute().ValueRanges[0].Values;
            batchValueGet.Ranges = new Repeatable<string>(new[] {"ЧИСЛО МЕСТ!A1:F" + countOfDirection + 1});


            var predictionColumn = GetLetterByNumber(GetNumberByLetter(agreementColumn) + countOfDirection + 2);
            var enrolledOnColumn = GetLetterByNumber(GetNumberByLetter(agreementColumn) + countOfDirection + 3);
            var countOriginals = 1;
            var placesSheetValues = batchValueGet.Execute().ValueRanges[0].Values;
            var countPlaces = getCountPlaces(placesSheetValues, DateTime.Now.Year.ToString(),
                "Число мест на 1 волну", sheet.Properties.Title);

            var columnCount = formulaValues[0].Count;
            var updatedRows = new List<RowData>();
            for (int i = 0; i < formulaValues.Count; i++)
            {
                updatedRows.Add(new RowData {Values = new List<CellData>()});
                int j = 0;
                foreach (var value in formulaValues[i])
                {
                    if (value.ToString().Contains("ArrayFormula") || value.ToString().Contains("="))
                    {
                        updatedRows[i].Values.Add(new CellData()
                        {
                            UserEnteredValue = new ExtendedValue
                                {FormulaValue = value.ToString()},
                            UserEnteredFormat = standartFormat
                        });
                    }
                    else
                    {
                        if (j == 0)
                        {
                            updatedRows[i].Values.Add(new CellData
                            {
                                UserEnteredValue = new ExtendedValue
                                    {StringValue = ""},
                                UserEnteredFormat = standartFormat
                            });
                        }
                        else
                        {
                            updatedRows[i].Values.Add(new CellData
                            {
                                UserEnteredValue = new ExtendedValue
                                    {StringValue = value.ToString()},
                                UserEnteredFormat = standartFormat
                            });
                        }
                    }

                    j++;
                }

                if (valueValues[i].Count >= formulaValues[i].Count) continue;
                while (valueValues[i].Count < formulaValues[i].Count)
                {
                    valueValues[i].Add("");
                }
            }

            for (int i = 0; i < formulaValues.Count; i++)
            {
                if (valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                    .Contains("нет", StringComparison.OrdinalIgnoreCase))
                {
                    for (var k = 0; k < formulaValues[i].Count; k++)
                    {
                        updatedRows[i].Values[k].UserEnteredFormat = grayFormat;
                    }
                }
                else if (valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                             .Contains("1в", StringComparison.OrdinalIgnoreCase) ||
                         valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                             .Contains("ц", StringComparison.OrdinalIgnoreCase) ||
                         valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                             .Contains("оп", StringComparison.OrdinalIgnoreCase))
                {
                    for (var k = 0; k < valueValues[i].Count; k++)
                    {
                        updatedRows[i].Values[k].UserEnteredFormat = grayTextFormat;
                    }
                }
                else if (valueValues[i][GetNumberByLetter(documentColumn)].ToString()
                             .Contains("оригинал", StringComparison.OrdinalIgnoreCase) &&
                         valueValues[i][GetNumberByLetter(predictionColumn)].ToString() == "" ||
                         valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                             .Contains("да", StringComparison.OrdinalIgnoreCase))
                {
                    for (int k = 0; k <= GetNumberByLetter(agreementColumn); k++)
                    {
                        updatedRows[i].Values[k].UserEnteredFormat = greenFormat;
                    }
                }

                if (valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                    .Contains("да", StringComparison.OrdinalIgnoreCase))
                {
                    updatedRows[i].Values[GetNumberByLetter(predictionColumn)].UserEnteredFormat = greenFormat;
                }

                if (valueValues[i][GetNumberByLetter(agreementColumn)].ToString() != "" &&
                    valueValues[i][GetNumberByLetter(predictionColumn)].ToString() == "" ||
                    valueValues[i][GetNumberByLetter(predictionColumn)].ToString()
                        .Contains("да", StringComparison.OrdinalIgnoreCase) &&
                    valueValues[i][GetNumberByLetter(enrolledOnColumn)].ToString() == "")
                {
                    if (countOriginals != countPlaces)
                    {
                        updatedRows[i].Values[0].UserEnteredValue =
                            new ExtendedValue {NumberValue = countOriginals};
                    }
                    else
                    {
                        updatedRows[i].Values[0].UserEnteredValue =
                            new ExtendedValue {NumberValue = countOriginals};
                        var secondWave = false;
                        foreach (var t in valueValues)
                        {
                            if (t[GetNumberByLetter(predictionColumn)].ToString()
                                .Contains("1В", StringComparison.OrdinalIgnoreCase))
                            {
                                secondWave = true;
                            }
                        }

                        await SetPassingScore(placesSheetValues, sheet.Properties.Title,
                            spreadsheet.Sheets.FirstOrDefault(x => x.Properties.Title.Equals("Число мест", StringComparison.OrdinalIgnoreCase)),
                            int.Parse(valueValues[i][GetNumberByLetter(sumEgeColumn)].ToString()), user,
                            spreadsheet, secondWave);
                        for (var k = 0; k < GetNumberByLetter(agreementColumn); k++)
                        {
                            updatedRows[i].Values[k].UserEnteredFormat = yellowFormat;
                        }
                    }

                    countOriginals++;
                }
            }

            var updateRequest = new Request
            {
                UpdateCells = new UpdateCellsRequest
                {
                    Range = new GridRange
                    {
                        SheetId = sheet.Properties.SheetId,
                        StartColumnIndex = 0,
                        StartRowIndex = 16,
                        EndColumnIndex = columnCount,
                    },
                    Rows = updatedRows,
                    Fields = "UserEnteredValue, userEnteredFormat"
                }
            };
            var bussr = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> {updateRequest}
            };
            var bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
            bur.Execute();
        }

        private string GetLetterByNumber(int number)
        {
            var letter = "";
            var tempNumber = number;
            int limit;
            do
            {
                letter = Chars[tempNumber % Chars.Length] + letter;
                tempNumber -= tempNumber % Chars.Length == 0 ? Chars.Length : tempNumber % Chars.Length;
                limit = letter[^1] == 'A' ? 0 : 1;
            } while (tempNumber >= limit);

            return letter;
        }

        private int GetNumberByLetter(string letter)
        {
            return letter.Sum(t => Chars.IndexOf(t) == 0 && letter.Length > 1 ? Chars.Length : Chars.IndexOf(t));
        }

        private SheetsService Service(ClaimsPrincipal user)
        {
            var userMail = user.Claims.FirstOrDefault(claim => claim.Value.Contains("@"))?.Value ?? "user";
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                _userCredential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    userMail,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = _userCredential,
                ApplicationName = "creatingcompetitionlists",
            });
        }

        private int getCountDirections(IList<object> head)
        {
            int countDirection = 0;
            for (int i = 0; i < head.Count; i++)
            {
                if (head[i].ToString().Contains("Н_"))
                    countDirection++;
            }

            return countDirection;
        }

        private int getCountPlaces(IList<IList<object>> sheetValues, string firstFindColumnHead,
            string secondFindColumnHead, string findValue)
        {
            var firstColumnNumber = findColumnNumber(sheetValues, firstFindColumnHead);
            var secondColumnNumber = findColumnNumber(sheetValues, secondFindColumnHead);
            var firstRowNumber = findRowNumber(sheetValues, firstColumnNumber, findValue);
            return int.Parse(sheetValues[firstRowNumber][secondColumnNumber].ToString());
        }

        private async Task SetPassingScore(IList<IList<object>> sheetValues, string findValue, Sheet sheet,
            int passingScore,
            ClaimsPrincipal user, Spreadsheet spreadsheet, bool secondWave = false)
        {
            var firstColumnNumber = findColumnNumber(sheetValues, DateTime.Now.Year.ToString());
            var secondColumnNumber = findColumnNumber(sheetValues, "Проходной балл");
            var rowNumber = findRowNumber(sheetValues, firstColumnNumber, findValue);
            var updatedRows = new List<RowData> {new RowData{Values = new List<CellData>()}};
            updatedRows[0].Values.Add(new CellData {UserEnteredValue = new ExtendedValue {NumberValue = passingScore}});
            var updateRequest = new Request
            {
                UpdateCells = new UpdateCellsRequest
                {
                    Range = new GridRange
                    {
                        SheetId = sheet.Properties.SheetId,
                        StartColumnIndex = secondWave ? secondColumnNumber + 2 : secondColumnNumber,
                        StartRowIndex = rowNumber,
                        EndColumnIndex = secondWave ? secondColumnNumber + 2 + 1 : secondColumnNumber + 1,
                        EndRowIndex = rowNumber + 1
                    },
                    Rows = updatedRows,
                    Fields = "UserEnteredValue"
                }
            };
            var bussr = new BatchUpdateSpreadsheetRequest
            {
                Requests = new List<Request> {updateRequest}
            };
            var bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
            bur.Execute();
        }

        private int findRowNumber(IList<IList<object>> baseSheetValues, int columnNumber, string expectedRowHead)
        {
            for (int i = 0; i < baseSheetValues.Count; i++)
            {
                if (baseSheetValues[i][columnNumber].ToString() == expectedRowHead)
                {
                    return i;
                }
            }

            return 0;
        }

        private int findColumnNumber(IList<IList<object>> values, string columnHead)
        {
            for (int i = 0; i < values[0].Count; i++)
            {
                if (values[0][i].ToString() == columnHead)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}