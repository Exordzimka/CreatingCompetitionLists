using System.Security.Claims;
using CreatingCompetitionLists.Services;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace CreatingCompetitionLists.GoogleServices
{
    public static class GoogleDriveService
    {
        private static readonly string[] Scopes = {DriveService.Scope.Drive, DriveService.Scope.DriveFile};

        public static DriveService GetDriveService(ClaimsPrincipal user)
        {
            return new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = CredentialService.GetCredentials(user, Scopes),
                ApplicationName = "creatingcompetitionlists",
            });
        }
    }
}