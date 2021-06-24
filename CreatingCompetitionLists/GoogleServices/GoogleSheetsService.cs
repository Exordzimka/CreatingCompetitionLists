using System.Security.Claims;
using CreatingCompetitionLists.Services;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace CreatingCompetitionLists.GoogleServices
{
    public static class GoogleSheetsService
    {
        private static readonly string[] Scopes = {SheetsService.Scope.Drive, SheetsService.Scope.DriveFile};

        public static SheetsService GetSheetsService(ClaimsPrincipal user)
        {
            return new SheetsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = CredentialService.GetCredentials(user, Scopes),
                ApplicationName = "creatingcompetitionlists",
            });
        }
    }
}