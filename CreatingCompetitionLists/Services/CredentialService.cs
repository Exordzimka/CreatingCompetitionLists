using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;

namespace CreatingCompetitionLists.Services
{
    public static class CredentialService
    {
        public static UserCredential GetCredentials(ClaimsPrincipal user, string[] scopes)
        {
            var userMail = user.Claims.FirstOrDefault(claim => claim.Value.Contains("@"))?.Value ?? "user";
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                return  GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    scopes,
                    userMail,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
        }
    }
}