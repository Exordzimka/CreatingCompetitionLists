using System;
using System.Collections.Generic;
using System.Linq;
using CreatingCompetitionLists.Data;
using CreatingCompetitionLists.GoogleServices;
using Google.Apis.Drive.v3;

namespace CreatingCompetitionLists.Services
{
    public class DriveSearchService
    {
        private static int i = 0;
        private static List<string> tokens = new List<string>{""};
        private static Queue<string> previousTokens = new Queue<string>();
        public SearchResponse GetFilesPage(System.Security.Claims.ClaimsPrincipal user,bool next, bool previous)
        {
            var service = GoogleDriveService.GetDriveService(user);
            // var startPageToken = service.Changes.GetStartPageToken();
           
            // startPageToken.DriveId = "";
            var listRequest = service.Files.List();
            listRequest.PageSize = 25;
            listRequest.Fields = "nextPageToken, files(id, name, webViewLink)";
            listRequest.Q =
                "mimeType contains 'spreadsheet'";
            
            if (next)
            {
                i++;
                listRequest.PageToken = tokens[i];
            }

            if (previous)
            {
                i--;
                listRequest.PageToken = tokens[i];
            }

            if (!next && !previous)
            {
                i = 0;
            }
            var listResponse = listRequest.Execute();
            var files = listResponse.Files.ToList();
            if (!tokens.Contains(listResponse.NextPageToken))
            {
                tokens.Add(listResponse.NextPageToken);
            }
            
            return new SearchResponse{Files = files, NextPageToken = i+1==tokens.Count ? null : tokens[i+1], PreviousPageToken = i-1<0 ? null : tokens[i-1]};
        }
    }
}