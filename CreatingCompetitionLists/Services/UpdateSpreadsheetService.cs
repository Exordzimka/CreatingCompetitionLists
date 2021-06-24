using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
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

        public void HighlightOriginals(string spreadsheetId, ClaimsPrincipal user)
        {
            var spreadsheet = Service(user).Spreadsheets.Get(spreadsheetId).Execute();
            var startRow = 17;
            var placesSheet = spreadsheet.Sheets.FirstOrDefault(x =>
                x.Properties.Title.Equals("число мест", StringComparison.OrdinalIgnoreCase));
            var simpleBorder = new Border {Style = "SOLID", Color = new Color {Blue = 0, Green = 0, Red = 0}};
            var borders = new Borders
                {Top = simpleBorder, Bottom = simpleBorder, Left = simpleBorder, Right = simpleBorder};
            var standartFormat = new CellFormat
            {
                BackgroundColor = new Color {Blue = 255, Green = 255, Red = 255},
                TextFormat = new TextFormat {Bold = false, Italic = false, Strikethrough = false, Underline = false, ForegroundColor = new Color{Blue = 0, Green = 0, Red = 0}},
                Borders = borders
            };
            var googleGreen = new Color
                {Blue = (float)systemColor.Lime.B/255, Red = (float)systemColor.Lime.R/255, Green = (float)systemColor.Lime.G/255};
            var googleYellow = new Color
                {Blue = (float)systemColor.Yellow.B/255, Green = (float)systemColor.Yellow.G/255, Red = (float)systemColor.Yellow.R/255};
            var googleGray = new Color
                {Blue = (float)systemColor.DimGray.B/255, Green = (float)systemColor.DimGray.G/255, Red = (float)systemColor.DimGray.R/255};
            var greenFormat = new CellFormat
            {
                BackgroundColor = googleGreen,
                Borders = borders
            };
            var yellowFormat = new CellFormat
            {
                BackgroundColor = googleYellow,
                Borders = new Borders
                {
                    Bottom = new Border
                    {
                        Color = new Color
                        {
                            Blue = systemColor.Red.B, Green = systemColor.Red.G, Red = systemColor.Red.R
                        },
                        Style = "SOLID_THICK"
                    },
                    Top = simpleBorder,
                    Left = simpleBorder,
                    Right = simpleBorder
                }
            };
            var grayFormat = new CellFormat
            {
                BackgroundColor = googleGray,
                TextFormat = new TextFormat {Strikethrough = true},
                Borders = borders
            };
            var grayTextFormat = new CellFormat
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
            var batchGet = Service(user).Spreadsheets.Values.BatchGet(spreadsheetId);
            batchGet.DateTimeRenderOption = SpreadsheetsResource.ValuesResource.BatchGetRequest
                .DateTimeRenderOptionEnum.FORMATTEDSTRING;
            batchGet.ValueRenderOption =
                SpreadsheetsResource.ValuesResource.BatchGetRequest.ValueRenderOptionEnum.FORMULA;
            batchGet.MajorDimension = SpreadsheetsResource.ValuesResource.BatchGetRequest.MajorDimensionEnum.ROWS;
            batchGet.Ranges = new Repeatable<string>(new []{"АТПП!A17:N40"});
            var test = batchGet.Execute().ValueRanges[0].Values;
            foreach (var sheet in spreadsheet.Sheets)
            {
                var countRow = 800;
                var countPlaces = 0;
                var columnCount = sheet.Properties.GridProperties.ColumnCount.Value;
                var placesRange = $"{placesSheet.Properties.Title}!A1:B";
                
                // var placesValues = batchGet.Execute().ValueRanges[0].Values;
                // for (int i = 1; i < placesValues.Count; i++)
                // {
                    // for (int j = 1; j < placesValues[i].Count; j++)
                    // {
                        // if (placesValues[i][j].ToString() == sheet.Properties.Title)
                            // countPlaces = int.Parse(placesValues[i][0].ToString());
                    // }
                // }

                if (sheet.Properties.Title.Equals("База", StringComparison.OrdinalIgnoreCase) ||
                    sheet.Properties.Title.Equals("Число мест", StringComparison.OrdinalIgnoreCase)) continue;
                var repeatRequest = new Request
                {
                    RepeatCell = new RepeatCellRequest
                    {
                        Range = new GridRange
                        {
                            SheetId = sheet.Properties.SheetId,
                            StartColumnIndex = 0,
                            StartRowIndex = 16,
                            EndColumnIndex = columnCount,
                            EndRowIndex = 900
                        },
                        Cell = new CellData
                        {
                            UserEnteredFormat = standartFormat
                        },

                        Fields = "UserEnteredFormat"
                    }
                };
                var rows = new List<RowData>();
                // for (var i = 0; i < countRow; i++)
                // {
                    // rows.Add(new RowData {Values = new List<CellData>()});
                    // rows[^1].Values.Add(new CellData {UserEnteredValue = new ExtendedValue {StringValue = ""}});
                // }

                // var updateRequest = new Request
                // {
                    // UpdateCells = new UpdateCellsRequest
                    // {
                        // Range = new GridRange
                        // {
                            // SheetId = sheet.Properties.SheetId,
                            // StartColumnIndex = 0,
                            // StartRowIndex = 16,
                            // EndColumnIndex = columnCount,
                            // EndRowIndex = 900
                        // },
                        // Rows = rows,
                        // Fields = "UserEnteredValue"
                    // }
                // };
                var bussr = new BatchUpdateSpreadsheetRequest
                {
                    Requests = new List<Request> {repeatRequest}
                };
                var bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                // bur.Execute();
                var range = $"{sheet.Properties.Title}!A{startRow - 1}:{GetLetterByNumber(columnCount)}";
                var valueRange = Service(user).Spreadsheets.Values.Get(spreadsheet.SpreadsheetId, range).Execute();
                var data = new List<RowData>();
                for (int i = 0; i < valueRange.Values.Count; i++)
                {
                    data.Add(new RowData{Values = new List<CellData>()});
                    for (int j = 0; j < valueRange.Values[i].Count; j++)
                    {
                        data[^1].Values.Add((CellData)valueRange.Values[i][j]);
                    }
                }
                var range1 = valueRange.Range;
                var countOriginals = 1;
                var countDirection = 0;
                for (int i = 0; i < 1; i++)
                {
                    // for (int j = 0; j < data[i].Count; j++)
                    // {
                        // if (data[i][j].ToString().Contains("Н_"))
                            // countDirection++;
                    // }
                }

                var agreementColumn = "H";
                var documentColumn = "G";
                var predictionColumn = GetLetterByNumber(GetNumberByLetter(agreementColumn) + countDirection + 1);
                var enrolledOnColumn = GetLetterByNumber(GetNumberByLetter(agreementColumn) + countDirection + 2);
                var updatedRows = new List<RowData>();
                for (int i = 1; i < data.Count; i++)
                {
                    updatedRows.Add(new RowData{Values = new List<CellData>()});
                    if (predictionColumn.Contains("нет", StringComparison.OrdinalIgnoreCase))
                    {
                        for (var j = 0; j < GetNumberByLetter(agreementColumn); j++)
                        {
                            updatedRows[^1].Values.Add(new CellData{UserEnteredFormat = grayFormat});
                        }
                    }
                    // else if (data[i][GetNumberByLetter(predictionColumn)].ToString()
                                 // .Contains("1в", StringComparison.OrdinalIgnoreCase) ||
                             // data[i][GetNumberByLetter(predictionColumn)].ToString()
                                 // .Contains("ц", StringComparison.OrdinalIgnoreCase) ||
                             // data[i][GetNumberByLetter(predictionColumn)].ToString()
                                 // .Contains("оп", StringComparison.OrdinalIgnoreCase))
                    // {
                        // for (var j = 0; j < sheet.Properties.GridProperties.ColumnCount; j++)
                        // {
                            // updatedRows[^1].Values.Add(new CellData{UserEnteredFormat = grayTextFormat});
                        // }
                    // }
                    // else if (data[i][GetNumberByLetter(documentColumn)].ToString()
                                 // .Contains("оригинал", StringComparison.OrdinalIgnoreCase) &&
                             // data[i][GetNumberByLetter(predictionColumn)].ToString() == "" ||
                             // data[i][GetNumberByLetter(predictionColumn)].ToString()
                                 // .Contains("да", StringComparison.OrdinalIgnoreCase))
                    // {
                        // for (int j = 0; j < GetNumberByLetter(agreementColumn); j++)
                        // {
                            // updatedRows[^1].Values.Add(new CellData{UserEnteredFormat = greenFormat});
                        // }
                    // }
                    // else if (data[i][GetNumberByLetter(predictionColumn)].ToString()
                        // .Contains("да", StringComparison.OrdinalIgnoreCase))
                    // {
                        // for (int j = 0; j < GetNumberByLetter(predictionColumn)-1; j++)
                        // {
                            // updatedRows[^1].Values.Add(new CellData());
                        // }
                        // updatedRows[^1].Values.Add(new CellData{UserEnteredFormat = greenFormat});
                    // }
                   
                    // if (data[i][GetNumberByLetter(agreementColumn)].ToString() != "" &&
                        // data[i][GetNumberByLetter(predictionColumn)].ToString() == "" ||
                        // data[i][GetNumberByLetter(predictionColumn)].ToString()
                            // .Contains("да", StringComparison.OrdinalIgnoreCase) &&
                        // data[i][GetNumberByLetter(enrolledOnColumn)].ToString() == "")
                    // {
                        // if (countOriginals != countPlaces)
                        // {
                            // updatedRows[^1].Values.Add(new CellData
                            // {
                                // UserEnteredValue = new ExtendedValue {NumberValue = countOriginals},
                            // });
                        // }
                        // else
                        // {
                            // updatedRows[^1].Values.Add(new CellData
                            // {
                                // UserEnteredValue = new ExtendedValue {NumberValue = countOriginals},
                                // UserEnteredFormat = yellowFormat
                            // });
                            // for (var j = 1; j < GetNumberByLetter(agreementColumn); j++)
                            // {
                                // updatedRows[^1].Values.Add(new CellData{UserEnteredFormat = yellowFormat});
                            // }
                        // }
                        // countOriginals++;
                    // }
                }

                var dataRequest = new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Range = new GridRange
                        {
                            SheetId = sheet.Properties.SheetId,
                            StartColumnIndex = 0,
                            StartRowIndex = 16,
                            EndColumnIndex = columnCount,
                            EndRowIndex = 900
                        },
                        Rows = updatedRows,
                        Fields = "UserEnteredValue"
                    }
                };
                var formatRequest = new Request
                {
                    UpdateCells = new UpdateCellsRequest
                    {
                        Range = new GridRange
                        {
                            SheetId = sheet.Properties.SheetId,
                            StartColumnIndex = 0,
                            StartRowIndex = 16,
                            EndColumnIndex = columnCount,
                            EndRowIndex = 900
                        },
                        Rows = updatedRows,
                        Fields = "UserEnteredFormat"
                    }
                };
                bussr = new BatchUpdateSpreadsheetRequest {Requests = new List<Request> {dataRequest, formatRequest}};
                bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                bur.Execute();
            }
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
    }
}