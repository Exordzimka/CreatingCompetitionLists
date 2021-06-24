using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using CreatingCompetitionLists.Data;
using CreatingCompetitionLists.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace CreatingCompetitionLists.Services
{
    public class CreateSpreadSheetsService
    {
        private readonly string[] _scopes = {SheetsService.Scope.Drive, SheetsService.Scope.DriveFile};

        public Spreadsheet CreateSpreadSheet(GoogleCredentials googleCredentials, System.Security.Claims.ClaimsPrincipal user, CreateRequest createRequest)
        {
            UserCredential credential;
            var userMail = user.Claims.FirstOrDefault(claim => claim.Value.Contains("@"))?.Value ?? "user";
            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    _scopes,
                    userMail,
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            
            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "creatingcompetitionlists",
            });

            var myNewSheet = new Spreadsheet
            {
                Properties = new SpreadsheetProperties {Title = createRequest.TableName},
                Sheets = new List<Sheet> {new Sheet {Properties = new SheetProperties {Title = "БАЗА"}}}
            };
            foreach (var directionId in createRequest.DirectionIds.Select(int.Parse))
            {
                using var db = new competition_listContext();
                myNewSheet.Sheets.Add(new Sheet{Properties = new SheetProperties{Title = db.Directions.FirstOrDefault(x => x.Id == directionId)?.ShortTitle}});
            }
            myNewSheet.Sheets.Add(new Sheet {Properties = new SheetProperties {Title = "ЧИСЛО МЕСТ"}});

            return service.Spreadsheets.Create(myNewSheet).Execute();
        }
    }
}