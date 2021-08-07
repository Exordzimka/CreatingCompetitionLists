using System;
using System.Collections.Generic;

#nullable disable

namespace CreatingCompetitionLists.Models
{
    public partial class Direction
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long? FacultyId { get; set; }
        public string ShortTitle { get; set; }
        public int? CountForEnrollee { get; set; }

        public virtual Faculty Faculty { get; set; }
    }
}
