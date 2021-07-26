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

        [HttpPost]
        [Route("add-faculty")]
        public void AddFaculty([FromBody] Faculty faculty)
        {
            using var db = new competition_listContext();
            db.Add(faculty);
            db.SaveChanges();
        }
        [HttpPut]
        [Route("update-faculty")]
        public void UpdateFaculty([FromBody] Faculty faculty)
        {
            using var db = new competition_listContext();
            db.Update(faculty);
            db.SaveChanges();
        }
        [HttpDelete]
        [Route("delete-faculty")]
        public void DeleteFaculty([FromBody] long id)
        {
            using var db = new competition_listContext();
            var faculty = db.Faculties.FirstOrDefault(x => x.Id == id);
            db.Remove(faculty);
            db.SaveChanges();
        }
        
        [HttpPost]
        [Route("add-direction")]
        public void AddDirection([FromBody] Direction direction)
        {
            using var db = new competition_listContext();
            db.Add(direction);
            db.SaveChanges();
        }
        [HttpPut]
        [Route("update-direction")]
        public void UpdateDirection([FromBody] Direction direction) 
        {
            using var db = new competition_listContext();
            db.Update(direction);
            db.SaveChanges();
        }
        [HttpDelete]
        [Route("delete-direction")]
        public void DeleteDirection([FromBody] long id)
        {
            using var db = new competition_listContext();
            var direction = db.Directions.FirstOrDefault(x => x.Id == id);
            db.Remove(direction);
            db.SaveChanges();
        }
    }
}