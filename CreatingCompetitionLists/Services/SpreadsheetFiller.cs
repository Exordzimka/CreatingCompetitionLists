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
        private string _baseOriginalColumn = "";
        private string _predictionColumn = "M";
        private string _enrolledOnColumn = "N";
        private string _commentColumn = "O";
        private string _phoneColumn = "P";
        private int _headRow = 7;
        private int _startRow = 2;
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private List<RowData> placesSheetRows;

        public void FillSpreadsheet(Spreadsheet spreadsheet,
            System.Security.Claims.ClaimsPrincipal user,
            List<string> directions, int countWave, int possibleDirections)
        {
            var columnAfterDirection = GetNumberByLetter(_startDirectionColumn) + directions.Count;
            _baseOriginalColumn = GetLetterByNumber(7 + directions.Count);
            _originalColumn = GetLetterByNumber(columnAfterDirection);
            _predictionColumn = GetLetterByNumber(GetNumberByLetter(_originalColumn) + 1);
            _enrolledOnColumn = GetLetterByNumber(GetNumberByLetter(_predictionColumn) + 1);
            _commentColumn = GetLetterByNumber(GetNumberByLetter(_enrolledOnColumn) + 1);
            _phoneColumn = GetLetterByNumber(GetNumberByLetter(_commentColumn) + 1);
            var spreadsheetId = spreadsheet.SpreadsheetId;
            var startValues = new List<object>
            {
                "", "№", "ФИО", "СНИЛС", "Преимущ. право", "Сумма баллов", "Вид документа об образовании",
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
                "Прогноз (зачислен (Ц, ОП, 1 волна), ДА, НЕТ)", "Комментарий", "ЕГЭ", "Номер", "ФИО", "СНИЛС"
            };
            for (var i = 1; i <= possibleDirections; i++)
            {
                baseValues.Add("Н_" + i);
            }

            baseValues.Add("Оригинал");
            baseValues.Add("Дополнительные баллы");
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
            baseValues.Add("Испанский язык");
            baseValues.Add("ИКТ");
            baseValues.Add("Русский язык_ВИ");
            baseValues.Add("Математика_ВИ");
            baseValues.Add("География_ВИ");
            baseValues.Add("Биология_ВИ");
            baseValues.Add("История_ВИ");
            baseValues.Add("Об-во_ВИ");
            baseValues.Add("Химия_ВИ");
            baseValues.Add("Физика_ВИ");
            baseValues.Add("Литература_ВИ");
            baseValues.Add("Английский язык_ВИ");
            baseValues.Add("Французский язык_ВИ");
            baseValues.Add("Немецкий язык_ВИ");
            baseValues.Add("Испанский язык_ВИ");
            baseValues.Add("ИКТ_ВИ");
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
                columns.Add(new SpreadsheetColumn{HeadName = startValues[i].ToString(), Column = GetLetterByNumber(i)});
            }

            foreach (var baseValue in baseValues)
            {
                baseHeadValues.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = baseValue.ToString()}});
            }
            
            
            foreach (var sheet in spreadsheet.Sheets)
            {
                var simpleBorder = new Border {Style = "SOLID", Color = new Color {Blue = 0, Green = 0, Red = 0}};
                var rows = new List<RowData>();
                Request updateRequest;
                Request repeatRequest;
                var countAppendRow = 2000;
                var countAppendColumn = 50;
                var appendColumnsRequest = new Request
                {
                    AppendDimension = new AppendDimensionRequest
                    {
                        SheetId = sheet.Properties.SheetId,
                        Dimension = "COLUMNS",
                        Length = countAppendColumn
                    }
                };
                var appendRowRequest = new Request
                {
                    AppendDimension = new AppendDimensionRequest
                    {
                        SheetId = sheet.Properties.SheetId,
                        Dimension = "ROWS",
                        Length = countAppendRow
                    }
                };
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
                        SetBaseFormula(rows, countAppendRow);
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
                                    EndRowIndex = countAppendRow
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
                                    EndColumnIndex = baseHeadValues.Values.Count,
                                    EndRowIndex = countAppendRow
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
                        bussr.Requests.Add(appendColumnsRequest);
                        bussr.Requests.Add(appendRowRequest);
                        bussr.Requests.Add(updateRequest);
                        bussr.Requests.Add(repeatRequest);
                        bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                        bur.Execute();
                        break;
                    case "ЧИСЛО МЕСТ":
                        placesSheetRows = new List<RowData>();
                        var placesColumns = new Dictionary<string, string>();
                        placesColumns.Add("A","КЦП");
                        placesColumns.Add("B",DateTime.Today.Year.ToString());
                        placesColumns.Add("C","Целевики");
                        placesColumns.Add("D","Особые права");
                        placesColumns.Add("E","Число мест на 1 волну");
                        placesColumns.Add("F","Проходной балл для 1 волны");
                        if (countWave == 2)
                        {
                            placesColumns.Add("G","Число мест на 2 волну");
                            placesColumns.Add("H","Проходной балл для 2 волны");   
                        }

                        var placesHeadRow = new RowData{Values = new List<CellData>()};
                        placesColumns.Values.ToList().ForEach(x =>
                            placesHeadRow.Values.Add(new CellData {UserEnteredValue = new ExtendedValue {StringValue = x}}));
                        placesSheetRows.Add(placesHeadRow);
                        var i = 2;
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
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = $"={directionDb.CountForEnrollee.Value}-${GetLetterByNumber(2)}{i}-${GetLetterByNumber(3)}{i}"}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = ""}});
                            }
                            else
                            {
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = $"={directionDb.CountForEnrollee.Value*0.8}-${GetLetterByNumber(2)}{i}-${GetLetterByNumber(3)}{i}"}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = ""}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = $"={directionDb.CountForEnrollee.Value*0.2}-${GetLetterByNumber(2)}{i}-${GetLetterByNumber(3)}{i}"}});
                                placesRow.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = ""}});
                            }
                            placesSheetRows.Add(placesRow);
                            i++;
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
                                Rows = placesSheetRows,
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
                        
                        SetFormulas(columns,sheet.Properties.Title, rows, baseHeadValues, countAppendRow, headValues);
                        updateRequest = new Request
                        {
                            UpdateCells = new UpdateCellsRequest
                            {
                                Range = new GridRange
                                {
                                    SheetId = sheet.Properties.SheetId,
                                    StartColumnIndex = 0,
                                    StartRowIndex = 0,
                                    EndColumnIndex = columns.Count,
                                    EndRowIndex = countAppendRow+1
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
                                    StartRowIndex = 6,
                                    EndColumnIndex = columns.Count,
                                    EndRowIndex = countAppendRow+1
                                },
                                Cell = new CellData
                                {
                                    UserEnteredFormat = userEnteredFormat
                                },

                                Fields = "UserEnteredFormat(Borders)"
                            }
                        };
                        bussr = new BatchUpdateSpreadsheetRequest();
                        bussr.Requests = new List<Request>{appendColumnsRequest, appendRowRequest, updateRequest, repeatRequest};
                        bur = Service(user).Spreadsheets.BatchUpdate(bussr, spreadsheet.SpreadsheetId);
                        bur.Execute();
                        break;
                }
            }
        }

        private void SetBaseFormula(List<RowData> values, int rowCount)
        {
            for (var i = 0; i < rowCount-1; i++)
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
        
        private void SetFormulas(List<SpreadsheetColumn> columns, string sheetName, List<RowData> values, RowData baseHeadValues, int rowCount, RowData headValues)
        {
            var lastColumn = GetLetterByNumber(baseHeadValues.Values.Count);
            for (var i = 0; i < 6; i++)
            {
                values.Add(new RowData{Values = new List<CellData>()});    
            }
            values.Add(headValues);
            using var db = new competition_listContext();
            var direction =
                db.Directions.FirstOrDefault(x => x.ShortTitle == sheetName);
            var faculty = db.Faculties.FirstOrDefault(x => x.Id == direction.FacultyId);
            values[0].Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = direction.Title}});
            values[1].Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = faculty.Title}});
            values[2].Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = "Число бюджетных мест: " + direction.CountForEnrollee}});
            var passingScore =
                $"=ЕслиОшибка(ИНДЕКС('ЧИСЛО МЕСТ'!$A$2:$Z;ПОИСКПОЗ({sheetName};'ЧИСЛО МЕСТ'!$B$2:$B;0);ПОИСКПОЗ(\"Проходной балл для 1 волны\";'ЧИСЛО МЕСТ'!$A$1:$Z$1;0));\"\")";
            values[3].Values.Add(new CellData{UserEnteredValue = new ExtendedValue{StringValue = "Текущий проходной балл: "}});
            values[3].Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = passingScore}});

            for (var i = 6; i < rowCount; i++)
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
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = DocumentFormula(sheetName, lastColumn, row)}});
                            break;
                        case "Согласие на зачисление":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = AgreementFormula(row)}});
                            break;
                        case "Оригинал":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = OriginalFormula(row, lastColumn)}});
                            break;
                        case "Прогноз":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = PredictionFormula(row, lastColumn)}});
                            break;
                        case "Зачислен НА":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = EnrolledOnFormula(row, lastColumn)}});
                            break;
                        case "Комментарий":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = CommentFormula(row, lastColumn)}});
                            break;
                        case "Телефон":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = PhoneFormula(row, lastColumn)}});
                            break;
                        case "ФИО":
                            last.Values.Add(new CellData{UserEnteredValue = new ExtendedValue{FormulaValue = FioFormula(row, lastColumn)}});
                            break;
                        default:
                            last.Values.Add(column.HeadName.Contains("Н_")
                                ? new CellData
                                {
                                    UserEnteredValue = new ExtendedValue
                                        {FormulaValue = DirectionFormula(lastColumn, sheetName, column.Column, row)}
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

        private string DirectionFormula(string lastColumn, string sheetName, string directionColumn, int row)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({directionColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0))=\"{sheetName}\";\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({directionColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0)));\"\"))";
        }

        private string DocumentFormula(string sheetName, string lastColumn,  int row)
        {
            return
                $"=ArrayFormula(ЕСЛИ(ИЛИ(ЕЧИСЛО(ПОИСК(\"{sheetName}\";{_originalColumn}{row}));ЕЧИСЛО(ПОИСК(\"{sheetName}\";{_enrolledOnColumn}{row})));\"Оригинал\";\"Копия\"))";
        }

        private string AgreementFormula(int row)
        {
            return $"=ArrayFormula(ЕСЛИ({_documentColumn}{row}=\"Оригинал\";\"✓\";\"\"))";
        }

        private string OriginalFormula(int row, string lastColumn)
        {
            return
                $"=ArrayFormula(ЕСЛИ({_enrolledOnColumn}{row}<>\"\";\"\";ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_originalColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_originalColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0)));\"\")))";
        }

        private string PredictionFormula(int row, string lastColumn)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);1)=0;\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);1));\"\"))";
        }

        private string EnrolledOnFormula(int row, string lastColumn)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_enrolledOnColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_enrolledOnColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0)));\"\"))";
        }

        private string CommentFormula(int row, string lastColumn)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_commentColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_commentColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0)));\"\"))";
        }

        private string FioFormula(int row, string lastColumn)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_fullNameColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_fullNameColumn}${_headRow};БАЗА!$A$1:${lastColumn}$1;0)));\"\"))";
        }

        private string PhoneFormula(int row, string lastColumn)
        {
            return
                $"=ArrayFormula(ЕСЛИОШИБКА(ЕСЛИ(ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_phoneColumn}${_headRow};'БАЗА'!$A$1:${lastColumn}$1;0))=0;\"\";ИНДЕКС('БАЗА'!$A$2:${lastColumn};ПОИСКПОЗ(${_snilsColumn}{row};'БАЗА'!$F$2:$F;0);ПОИСКПОЗ({_phoneColumn}${_headRow};БАЗА!$A$1:${lastColumn}$1;0)));\"\"))";
        }

        private string EnrolledOnBaseFormula(int row)
        {
            return $"=ArrayFormula(ЕСЛИ(ИЛИ(A{row}=\"1В\";A{row}=\"Ц\";A{row}=\"ОП\");{_baseOriginalColumn}{row};\"\"))";
        }
    }
}