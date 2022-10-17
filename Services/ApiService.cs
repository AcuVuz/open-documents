using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace open_documents.Services;

public static class ApiService
{
    public static async Task<bool> JsonModalPostWithToken(string token, string queryUrl, string httpMethod, string reportName , string typeDocuments , string docxUrl)
    {
#pragma warning disable SYSLIB0014 // Тип или член устарел
        var req = (HttpWebRequest)WebRequest.Create(queryUrl);    
#pragma warning restore SYSLIB0014 // Тип или член устарел
        req.Method = httpMethod;                                  
        req.Headers.Add("auth-token", token);
        req.Accept = "application/json";
        using var response = await req.GetResponseAsync();
        var headers = response.Headers;

        for (var i = 0; i < headers.Count; i++)
        {
            if (!headers[i]!.Contains("filename")) continue;
            var fileName = headers[i]?.Split("filename=");
            if (fileName[1].Contains(';'))
            {
                reportName = fileName[1].Split(';')[0];
            }
            else
            {
                reportName = fileName[1];
            }
            
            //reportName = fileName[1];
           

        }
        await using var responseStream = response.GetResponseStream();
        // MessageBox.Show(docxUrl);
        return await FileService.SaveModalReportDocxAsync(responseStream, reportName, docxUrl);
        /*return typeDocuments == "word"
            ? await FileService.SaveModalReportDocxAsync(responseStream, reportName, docxUrl)
            : FileService.SaveReportExcel(responseStream, reportName, docxUrl);
        */
    }

    public static async Task<bool> JsonPostWithToken(string token, string queryUrl, string httpMethod, string reportName, string typeDocuments, string docxUrl)
    {
#pragma warning disable SYSLIB0014 // Тип или член устарел
        var req = (HttpWebRequest)WebRequest.Create(queryUrl);
#pragma warning restore SYSLIB0014 // Тип или член устарел
        req.Method = httpMethod;
        req.Headers.Add("auth-token", token);
        req.Accept = "application/json";
        using var response = await req.GetResponseAsync();

        var headers = response.Headers;

        for (var i = 0; i < headers.Count; i++)
        {
            if (!headers[i]!.Contains("filename")) continue;
            var fileName = headers[i]?.Split("filename=");
            reportName = fileName[1];

        }
        await using var responseStream = response.GetResponseStream();

        return FileService.SaveReportDocxAsync(responseStream, reportName, docxUrl);

    }
    public static async Task<bool> JsonPostSave(string token, string queryUrl, string httpMethod, object payload)
    {
#pragma warning disable SYSLIB0014 // Тип или член устарел
        var req = (HttpWebRequest)WebRequest.Create(queryUrl);
#pragma warning restore SYSLIB0014 // Тип или член устарел
        req.Method = httpMethod;
        req.Headers.Add("auth-token", token);
        req.Accept = "application/json";

        await using (StreamWriter streamWriter = new(req.GetRequestStream()))
        {
            req.ContentType = "application/json";
            var json = JsonSerializer.Serialize(payload);
            await streamWriter.WriteAsync(json);
            // Записывает тело
            streamWriter.Close();
        }

        using var response = await req.GetResponseAsync();


        await using var responseStream = response.GetResponseStream();
        using StreamReader reader = new(responseStream, Encoding.UTF8);

        return true;
    }


}