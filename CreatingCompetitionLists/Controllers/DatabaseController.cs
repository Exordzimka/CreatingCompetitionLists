using System.Collections.Generic;
using System.Linq;
using CreatingCompetitionLists.Models;
using Microsoft.AspNetCore.Mvc;

namespace CreatingCompetitionLists.Controllers
{
    [ApiController]
    [Route("database")]
    public class DatabaseController : ControllerBase
    {
        [Route("get-directions")]
        public List<Direction> GetDirections()
        {
            using var db = new competition_listContext();
            return db.Directions.ToList();
        }

        [Route("get-faculties")]
        public List<Faculty> GetFaculties()
        {
            using var db = new competition_listContext();
            return db.Faculties.ToList();
        }
    }
}