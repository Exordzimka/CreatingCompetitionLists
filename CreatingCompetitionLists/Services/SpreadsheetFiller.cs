using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using CreatingCompetitionLists.Data;
using CreatingCompetitionLists.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using Google.Apis.Drive;

namespace CreatingCompetitionLists.Services
{
    public class SpreadsheetFiller
    {
        private UserCredential _userCredential;
        
        private string _numberColumn = "B";
        private string _fullNameColumn = "C";
        private string _snilsColumn = "D";
        private string _preferentialColumn = "E";
        private string _sumEgeColumn = "F";
        private string _documentColumn = "G";
        private string _agreementColumn = "H";
        private string _startDirectionColumn = "I";
        private string _originalColumn = "L";
        private string _predictionColumn = "M";
        private string _enrolledOnColumn = "N";
        private string _commentColumn = "O";
        private string _phoneColumn = "P";
        private int _headRow = 16;
        private int _startRow = 17;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public void FillSpreadsheet(Spreadsheet spreadsheet,
            System.Security.Claims.ClaimsPrincipal user,
            List<string> directions, int countWave, int possibleDirections)
        {
            var columnAfterDirection = GetNumberByLetter(_startDirectionColumn) + directions.Count;
            _originalColumn = GetLetterByNumber(columnAfterDirection + 1);
            _predictionColumn = GetLetterByNumber(GetNumberByLetter(_originalColumn) + 1);
            _enrolledOnColumn = GetLetterByNumber(GetNumberByLetter(_predictionColumn) + 1);
            _commentColumn = GetLetterByNumber(GetNumberByLetter(_enrolledOnColumn) + 1);
            _phoneColumn = GetLetterByNumber(GetNumberByLetter(_commentColumn) + 1);
            var spreadsheetId = spreadsheet.SpreadsheetId;
            var startValues = new List<object>
            {
                "№", "ФИО", "СНИЛС", "Преимущ. право", "Сумма баллов", "Вид документа об образовании",
                "Согласие на зачисление"
            };
            for (int i = 1; i <= directions.Count; i++)
            {
                startValues.Add("Н_" + i);
            }

            startValues.Add("Оригинал");
            startValues.Add("Прогноз");
            startValues.Add("Зачислен НА");
            startValues.Add("Комментарий");
            startValues.Add("Телефон");
            var baseValues = new List<object>
            {
                "Прогноз (зачислен (Ц, ОП, 1 волна), ДА, НЕТ)", "Комментарий", "ЕГЭ", "Номер", "ФИО"
            };
            for (var i = 1; i <= possibleDirections; i++)
            {
                baseValues.Add("Н_" + i);
            }

            baseValues.Add("Оригинал");
            baseValues.Add("Русский язык");
            baseValues.Add("Математика");
            baseValues.Add("География");
            baseValues.Add("Биология");
            baseValues.Add("История");
            baseValues.Add("Об-во");
            baseValues.Add("Химия");
            baseValues.Add("Физика");
            baseValues.Add("Литература");
            baseValues.Add("Английский язык");
            baseValues.Add("Французский язык");
            baseValues.Add("Немецкий язык");
            baseValues.Add("");
            baseValues.Add("ИКТ");
            baseValues.Add("Телефон");
            baseValues.Add("E-mail");
            baseValues.Add("Адрес по прописке");
            baseValues.Add("Общежитие");
            baseValues.Add("Зачислен НА");

            var headValues = new RowData {Values = new List<CellData>()};
            var baseHeadValues = new RowData {Values = new List<CellData>()};
            var columns = new List<SpreadsheetColumn>();
            for (var i = 0; i < startValues.Count; i++)
            {
                headValues.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = startValues[i].ToString()}});
                columns.Add(new SpreadsheetColumn{HeadName = startValues[i].ToString(), Column = GetLetterByNumber(i+1)});
            }

            foreach (var baseValue in baseValues)
            {
                baseHeadValues.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = baseValue.ToString()}});
            }
            
            
            foreach (var sheet in spreadsheet.Sheets)
            {
                var valueRange = new ValueRange();
                var simpleBorder = new Border {Style = "SOLID", Color = new Color {Blue = 0, Green = 0, Red = 0}};
                var rows = new List<RowData>();
                Request updateRequest;
                Request repeatRequest;
                Request appendRequest;
                BatchUpdateSpreadsheetRequest bussr;
                SpreadsheetsResource.BatchUpdateRequest bur;
                var userEnteredFormat = new CellFormat
                {
                    Borders = new Borders
                    {
                        Bottom = simpleBorder,
                        Top = simpleBorder,
                        Left = simpleBorder,
                        Right = simpleBorder
                    }
                };
                switch (sheet.Properties.Title)
                {
                    case "БАЗА":
                        rows = new List<RowData>();
                        rows.Add(baseHeadValues);
                        SetBaseFormula(rows);
                        updateRequest = new Request
                        {
                            UpdateCells = new UpdateCellsRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 0,
                                    EndColumnIndex = baseHeadValues.Values.Count,
                                    EndRowIndex = 900
                                },
                                Rows = rows,
                                Fields = "UserEnteredValue"
                            }
                        };
                        appendRequest = new Request()
                        {
                            AppendDimension = new AppendDimensionRequest()
                            {
                                SheetId = sheet.Properties.SheetId,
                                Dimension = "COLUMNS",
                                Length = 50
                            }
                        };
                        repeatRequest = new Request
                        {
                            RepeatCell = new RepeatCellRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 0,
                                    EndColumnIndex = baseHeadValues.Values.Count,
                                    EndRowIndex = 900
                                },
                                Cell = new CellData
                                {
                                    UserEnteredFormat = userEnteredFormat
                                },

                                Fields = "UserEnteredFormat(Borders)"
                            }
                        };
                        bussr = new BatchUpdateSpreadsheetRequest();
                        bussr.Requests = new List<Request>();
                        bussr.Requests.Add(appendRequest);
                        bussr.Requests.Add(updateRequest);
                        bussr.Requests.Add(repeatRequest);
                        bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                        bur.Execute();
                        break;
                    case "ЧИСЛО МЕСТ":
                        rows = new List<RowData>();
                        var placesColumns = new Dictionary<string, string>();
                        placesColumns.Add("A","КЦП");
                        placesColumns.Add("B",DateTime.Today.Year.ToString());
                        placesColumns.Add("C","Целевики");
                        placesColumns.Add("D","Особые права");
                        placesColumns.Add("E","Число мест на 1 волну");
                        placesColumns.Add("F","Проходной балл");
                        if (countWave == 2)
                        {
                            placesColumns.Add("G","Число мест на 2 волну");
                            placesColumns.Add("H","Проходной балл");   
                        }

                        var placesHeadRow = new RowData{Values = new List<CellData>()};
                        placesColumns.Values.ToList().ForEach(x =>
                            placesHeadRow.Values.Add(new CellData {UserEnteredValue = new ExtendedValue {StringValue = x}}));
                        rows.Add(placesHeadRow);
                        foreach (var direction in directions)
                        {
                            using var db = new competition_listContext();
                            var directionDb = db.Directions.FirstOrDefault(x => x.Id.Equals(int.Parse(direction)));
                            var placesRow = new RowData{Values = new List<CellData>()};
                            placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = directionDb.CountForEnrollee.Value.ToString()}});
                            placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = directionDb.ShortTitle}});
                            placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{NumberValue = 0}});
                            placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{NumberValue = 0}});
                            if (countWave == 1)
                            {
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = directionDb.CountForEnrollee.Value.ToString()}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = ""}});
                            }
                            else
                            {
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = (directionDb.CountForEnrollee.Value*0.8).ToString()}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = ""}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = (directionDb.CountForEnrollee.Value*0.2).ToString()}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = ""}});
                            }
                            rows.Add(placesRow);
                        }
                        updateRequest = new Request
                        {
                            UpdateCells = new UpdateCellsRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 0,
                                    EndColumnIndex = placesColumns.Count,
                                    EndRowIndex = directions.Count+1
                                },
                                Rows = rows,
                                Fields = "UserEnteredValue"
                            }
                        };
                        repeatRequest = new Request
                        {
                            RepeatCell = new RepeatCellRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 0,
                                    EndColumnIndex = placesColumns.Count,
                                    EndRowIndex = directions.Count+1
                                },
                                Cell = new CellData
                                {
                                    UserEnteredFormat = userEnteredFormat
                                },

                                Fields = "UserEnteredFormat(Borders)"
                            }
                        };
                        
                        bussr = new BatchUpdateSpreadsheetRequest();
                        bussr.Requests = new List<Request>();
                        bussr.Requests.Add(updateRequest);
                        bussr.Requests.Add(repeatRequest);
                        bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                        bur.Execute();
                        break;
                    default:
                        rows = new List<RowData>();
                        rows.Add(headValues);
                        SetFormulas(columns,sheet.Properties.Title, rows);
                        updateRequest = new Request
                        {
                            UpdateCells = new UpdateCellsRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 15,
                                    EndColumnIndex = columns.Count,
                                    EndRowIndex = 900
                                },
                                Rows = rows,
                                Fields = "UserEnteredValue"
                            }
                        };
                        repeatRequest = new Request
                        {
                            RepeatCell = new RepeatCellRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 15,
                                    EndColumnIndex = columns.Count,
                                    EndRowIndex = 900
                                },
                                Cell = new CellData
                                {
                                    UserEnteredFormat = userEnteredFormat
                                },

                                Fields = "UserEnteredFormat(Borders)"
                            }
                        };
                        bussr = new BatchUpdateSpreadsheetRequest();
                        bussr.Requests = new List<Request>();
                        bussr.Requests.Add(updateRequest);
                        bussr.Requests.Add(repeatRequest);
                        bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                        bur.Execute();
                        break;
                }
            }
        }

        private void SetBaseFormula(List<RowData> values)
        {
            for (int i = 0; i < 800; i++)
            {
                values.Add(new RowData());
                var last = values.Last();
                var row = i + 2;
                last.Values = new List<CellData>();
                for (var j = 0; j < values[0].Values.Count; j++)
                {
                    last.Values.Add(values[0].Values.Count - 1 == j
                        ? new CellData
                            {UserEnteredValue = new ExtendedValue {FormulaValue = EnrolledOnBaseFormula(row)}}
                        : new CellData {UserEnteredValue = new ExtendedValue {StringValue = ""}});
                }
            }
        }
        
        private void SetFormulas(List<SpreadsheetColumn> columns, string sheetName, List<RowData> values)
        {
            for (var i = 0; i < 800; i++)
            {
                values.Add(new RowData());
                var last = values.Last();
                last.Values = new List<CellData>();
                foreach (var column in columns)
                {
                    var row = i + _startRow;
                    switch (column.HeadName)
                    {
                        case "Вид документа об образовании":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = DocumentFormula(sheetName, row)}});
                            break;
                        case "Согласие на зачисление":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = AgreementFormula(row)}});
                            break;
                        case "Оригинал":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = OriginalFormula(row)}});
                            break;
                        case "Прогноз":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = PredictionFormula(row)}});
                            break;
                        case "Зачислен НА":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = EnrolledOnFormula(row)}});
                            break;
                        case "Комментарий":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = CommentFormula(row)}});
                            break;
                        case "Телефон":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = PhoneFormula(row)}});
                            break;
                        case "ФИО":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = FioFormula(row)}});
                            break;
                        default:
                            last.Values.Add(column.HeadName.Contains("Н_")
                                ? new CellData
                                {
                                    UserEnteredValue = new ExtendedValue
                                        {FormulaValue = DirectionFormula(sheetName, column.Column, row)}
                                }
                                : new CellData {UserEnteredValue = new ExtendedValue {StringValue = " "}});
                            break;
                    }
                }
            }   
        }
        
        private readonly string[] _scopes = {SheetsService.Scope.Drive, SheetsService.Scope.DriveFile};

        private SheetsService Service(System.Security.Claims.ClaimsPrincipal user)
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
            } while (tempNumber >= limit );

            return letter;
        }

        private int GetNumberByLetter(string letter)
        {
            return letter.Sum(t => Chars.IndexOf(t) == 0 && letter.Length > 1 ? Chars.Length : Chars.IndexOf(t));
        }

        private string DirectionFormula(string sheetName, string directionColumn, int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({directionColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0))=\"{sheetName}\";\"\";ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({directionColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0)));\"\"))";
        }

        private string DocumentFormula(string sheetName, int row)
        {
            return
                $"=ArrayFormula(ЕСЛИ(ИЛИ(ЕЧИСЛО(ПОИСК(\"{sheetName}\";{_originalColumn}{row}));ЕЧИСЛО(ПОИСК(\"{sheetName}\";{_enrolledOnColumn}{row})));\"Оригинал\";\"Копия\"))";
        }

        private string AgreementFormula(int row)
        {
            return $"=ArrayFormula(ЕСЛИ({_documentColumn}{row}=\"Оригинал\";\"✓\";\"\"))";
        }

        private string OriginalFormula(int row)
        {
            return
                $"=ArrayFormula(ЕСЛИ({_enrolledOnColumn}{row}<>\"\";\"\";ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ($C{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_originalColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_originalColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0)));\"\")))";
        }

        private string PredictionFormula(int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);1)=0;\"\";ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);1));\"\"))";
        }

        private string EnrolledOnFormula(int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AB;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_enrolledOnColumn}${_headRow};'БАЗА'!$A$1:$AB$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:$AB;ПОИСКПОЗ($C{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_enrolledOnColumn}${_headRow};'БАЗА'!$A$1:$AB$1;0)));\"\"))";
        }

        private string CommentFormula(int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_commentColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_commentColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0)));\"\"))";
        }

        private string FioFormula(int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_snilsColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_snilsColumn}${_headRow};БАЗА!$A$1:$AA$1;0)));\"\"))";
        }

        private string PhoneFormula(int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_phoneColumn}${_headRow};'БАЗА'!$A$1:$AA$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:$AA;ПОИСКПОЗ(${_fullNameColumn}{row};'БАЗА'!$E$2:$E;0);ПОИСКПОЗ({_phoneColumn}${_headRow};БАЗА!$A$1:$AA$1;0)));\"\"))";
        }

        private string EnrolledOnBaseFormula(int row)
        {
            return $"=ArrayFormula(ЕСЛИ(ИЛИ(A{row}=\"1В\";A{row}=\"Ц\";A{row}=\"ОП\");I{row};\"\"))";
        }
    }
}