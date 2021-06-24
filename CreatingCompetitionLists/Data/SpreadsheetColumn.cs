namespace CreatingCompetitionLists.Data
{
    public class SpreadsheetColumn
    {
        private string _headName;
        private string _formula;
        private string _column;

        public string HeadName
        {
            get => _headName;
            set => _headName = value;
        }

        public string Formula
        {
            get => _formula;
            set => _formula = value;
        }

        public string Column
        {
            get => _column;
            set => _column = value;
        }
    }
}