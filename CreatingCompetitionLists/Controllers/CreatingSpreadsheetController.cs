using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;

namespace CreatingCompetitionLists.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CreatingSpreadsheetController : ControllerBase
    {
        private UserCredential _userCredential;

        public CreatingSpreadsheetController()
        {
            
        }
    }
}