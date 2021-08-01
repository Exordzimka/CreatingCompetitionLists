using System;
using System.Collections.Generic;
using CreatingCompetitionLists.Data;
using CreatingCompetitionLists.Services;
using Google.Apis.Sheets.v4.Data;
using Google.GData.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CreatingCompetitionLists.Controllers
{
    [ApiController]
    [Route("spreadsheets")]
    public class SpreadsheetController : ControllerBase
    {
        private readonly IOptions<GoogleCredentials> _googleCredentials;
        private readonly CreateSpreadSheetsService _createSpreadSheetsService = new CreateSpreadSheetsService();
        private readonly UpdateSpreadsheetService _updateSpreadsheetService = new UpdateSpreadsheetService();
        private readonly SpreadsheetFiller _spreadsheetFiller = new SpreadsheetFiller();

        public SpreadsheetController(IOptions<GoogleCredentials> googleCredentials)
        {
            _googleCredentials = googleCredentials;
        }

        [HttpPost("create")]
        public void CreateSpreadSheet([FromBody] CreateRequest createRequest)
        {
            var spreadsheet =
                _createSpreadSheetsService.CreateSpreadSheet(_googleCredentials.Value, User, createRequest);
            _spreadsheetFiller.FillSpreadsheet(spreadsheet,User, createRequest.DirectionIds, int.Parse(createRequest.Stage), int.Parse(createRequest.PossibleDirections));
        }

        [Route("getData")]
        public CreateFormData GetCreateFormData()
        {
            return new CreateFormData();
        }

        [Route("highlight-originals")]
        public string HighlightOriginals([FromQuery]string spreadsheetId)
        {
            try
            {
                return _updateSpreadsheetService.HighlightOriginals(spreadsheetId, User);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        [Route("reduce-directions")]
        public string ReduceDirections([FromQuery]string spreadsheetId)
        {
            try
            {
                return _updateSpreadsheetService.directionReduct(spreadsheetId, User);
            }
            catch(Exception e)
            {
                return e.ToString();
            }
        }
    }
}