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
        public SearchResponse GetFilesPage(System.Security.Claims.ClaimsPrincipal user, string pageToken)
        {
            var service = GoogleDriveService.GetDriveService(user);
            var listRequest = service.Files.List();
            listRequest.PageSize = 25;
            listRequest.Fields = "nextPageToken, files(id, name, webViewLink)";
            listRequest.Q =
                "mimeType contains 'spreadsheet'";
            string previousPageToken = null;
            if (!string.IsNullOrWhiteSpace(pageToken))
            {
                previousPageToken = listRequest.PageToken;
                listRequest.PageToken = pageToken;
            }
            // List files.
            var listResponse = listRequest.Execute();
            var files = listResponse.Files.ToList();
            var nextPageToken = listResponse.NextPageToken;
            return new SearchResponse{Files = files, NextPageToken = nextPageToken, PreviousPageToken = previousPageToken};
        }
    }
}