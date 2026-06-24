namespace MovieAPI.Extensions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SeedFromGoogleAPI
{
    public async Task<List<IList<object>>> GetSheetDataAsync(string spreadsheetId, string sheetName, string credentialsFilePath)
    {
        GoogleCredential? credential = GoogleCredential.FromFile(credentialsFilePath)
            .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);

        SheetsService service = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Sheets API C#"
        });

        string range = $"Movies"; // Or specify a range like "Sheet1!A1:D10"
        SpreadsheetsResource.ValuesResource.GetRequest? request = service.Spreadsheets.Values.Get(spreadsheetId, range);

        ValueRange? response = await request.ExecuteAsync();
        return (List<IList<object>>)response.Values;
    }
}