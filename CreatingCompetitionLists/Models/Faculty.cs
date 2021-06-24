using System;
using System.Collections.Generic;

#nullable disable

namespace CreatingCompetitionLists.Models
{
    public partial class Faculty
    {
        public Faculty()
        {
            Directions = new HashSet<Direction>();
        }

        public long Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<Direction> Directions { get; set; }
    }
}
