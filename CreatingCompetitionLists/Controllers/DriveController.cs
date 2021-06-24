using CreatingCompetitionLists.Data;
using CreatingCompetitionLists.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreatingCompetitionLists.Controllers
{
    [ApiController]
    [Route("drive")]
    public class DriveController : ControllerBase
    {
        private readonly DriveSearchService _driveSearchService = new DriveSearchService();

        [Route("search")]
        public SearchResponse SearchFiles([FromQuery(Name = "page-token")] string pageToken)
        {
            return _driveSearchService.GetFilesPage(User, pageToken);
        }
    }
}