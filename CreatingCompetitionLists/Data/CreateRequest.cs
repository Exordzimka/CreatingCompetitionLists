using System.Collections.Generic;

namespace CreatingCompetitionLists.Data
{
    public class CreateRequest
    {
        private string _tableName;
        private string _facultetId;
        private List<string> _directionIds;
        private string _stage;
        private string _possibleDirections;
        public string TableName
        {
            get => _tableName;
            set => _tableName = value;
        }

        public string FacultetId
        {
            get => _facultetId;
            set => _facultetId = value;
        }

        public List<string> DirectionIds
        {
            get => _directionIds;
            set => _directionIds = value;
        }

        public string Stage
        {
            get => _stage;
            set => _stage = value;
        }

        public string PossibleDirections
        {
            get => _possibleDirections;
            set => _possibleDirections = value;
        }
    }
}