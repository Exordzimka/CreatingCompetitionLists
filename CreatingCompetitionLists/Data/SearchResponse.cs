using System.Collections.Generic;
using Google.Apis.Drive.v3.Data;

namespace CreatingCompetitionLists.Data
{
    public class SearchResponse
    {
        private List<File> _files = new List<File>();
        private string _nextPageToken;
        private string _previousPageToken;
        
        public List<File> Files
        {
            get => _files;
            set => _files = value;
        }

        public string NextPageToken
        {
            get => _nextPageToken;
            set => _nextPageToken = value;
        }

        public string PreviousPageToken
        {
            get => _previousPageToken;
            set => _previousPageToken = value;
        }
    }
}