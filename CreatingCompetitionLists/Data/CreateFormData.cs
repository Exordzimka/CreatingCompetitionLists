using System.Collections.Generic;
using System.Linq;
using CreatingCompetitionLists.Models;

namespace CreatingCompetitionLists.Data
{
    public class CreateFormData
    {
        private List<Faculty> _faculties;
        private List<Direction> _directions;

        public CreateFormData()
        {
            using var db = new competition_listContext();
            _faculties = db.Faculties.ToList();
            _directions = db.Directions.ToList();
        }

        public List<Faculty> Faculties
        {
            get => _faculties;
            set => _faculties = value;
        }

        public List<Direction> Directions
        {
            get => _directions;
            set => _directions = value;
        }
    }
}