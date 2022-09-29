using System.Net;
using System.Threading.Tasks;

namespace open_documents.Services;

public static class ApiService
{
    public static async Task<bool> JsonPostWithToken(string token, string queryUrl, string httpMethod, string reportName , string typeDocuments , string docxUrl)
    {
#pragma warning disable SYSLIB0014 // Тип или член устарел
        var req = (HttpWebRequest)WebRequest.Create(queryUrl);    
#pragma warning restore SYSLIB0014 // Тип или член устарел
        req.Method = httpMethod;                                  
        req.Headers.Add("auth-token", token);
        req.Accept = "application/json";

        using var response = await req.GetResponseAsync();

        await using var responseStream = response.GetResponseStream();

        return typeDocuments == "word" 
            ? FileService.SaveReportDocx(responseStream, reportName, docxUrl) 
            : FileService.SaveReportExcel(responseStream, reportName, docxUrl);
       

    }
}