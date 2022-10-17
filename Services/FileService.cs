using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;

namespace open_documents.Services;

public static class FileService
{
    public static bool CheckClasses()
    {
        // Если реестр есть 
        #pragma warning disable CA1416 // Validate platform compatibility
        return Registry.ClassesRoot.OpenSubKey("jmuagent\\shell\\open\\command") != null;
        #pragma warning restore CA1416 // Validate platform compatibility
    }

    public static void CreateRegister()
    {
        // путь к файлу
        var exePath = AppDomain.CurrentDomain.BaseDirectory;
        #pragma warning disable CA1416 // Validate platform compatibility
        Registry.ClassesRoot.CreateSubKey("jmuagent").SetValue("", "");
        Registry.ClassesRoot.CreateSubKey("jmuagent").SetValue("URL Protocol", "");
        Registry.ClassesRoot.CreateSubKey("jmuagent\\shell\\open\\command").SetValue("", $"\"{exePath}open-documents.exe\" \"%1\"");
        #pragma warning restore CA1416 // Validate platform compatibility
    }

    public static async Task<bool> SaveModalReportDocxAsync(Stream inputStream, string reportName, string docxUrl)
    { 
        // Получить путь рабочего стола
        const string path = "D:";
        var file = path + $"\\documents-cont\\{reportName.Replace(" ", "_").Trim('"')}";
        //MessageBox.Show(file);
        if (Directory.Exists(path + $"\\documents-cont"))
        {
            await using var outputFileStream = new FileStream(file, FileMode.Create);
            await inputStream.CopyToAsync(outputFileStream);
        }
        else
        {
            Directory.CreateDirectory(path + $"\\documents-cont");
            await using var outputFileStream = new FileStream(file, FileMode.Create);
            await inputStream.CopyToAsync(outputFileStream);
        }
        await inputStream.DisposeAsync();
        if (string.IsNullOrEmpty(docxUrl)) return false;


        var idProcess = Process.Start(@$"""{docxUrl}""", file);

        if (MessageBox.Show("Сохранить файл?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.No)
        {
            object fileName = new
            {

                filename = reportName,
            };

            await ApiService.JsonPostSave("secret", "http://jmu.api.lgpu.org/reports-education" + "/general/statements/save", "POST", fileName);
        }
        else
        {
            idProcess.Kill();
            File.Delete(file);
        }
        return false;
    }

    public static bool SaveReportDocxAsync(Stream inputStream, string reportName, string docxUrl)
    {
        // Получить путь рабочего стола
        const string path = "D:";

        var file = path + $"\\documents-cont\\{reportName.Replace(" ", "_")}";
        if (Directory.Exists(path + $"\\documents-cont"))
        {
            using var outputFileStream = new FileStream(file, FileMode.Create);
            inputStream.CopyTo(outputFileStream);
        }
        else
        {
            Directory.CreateDirectory(path + $"\\documents-cont");
            using var outputFileStream = new FileStream(file, FileMode.Create);
            inputStream.CopyTo(outputFileStream);
        }
        inputStream.Dispose();
        
        if (string.IsNullOrEmpty(docxUrl)) return false;

        Process.Start(@$"""{docxUrl}""", file);

        return true;
    }
    public static bool SaveReportExcel(Stream inputStream, string reportName , string docxUrl)
    {
        // Получить путь рабочего стола
        const string path = "D:";

        var file = path + $"\\documents-cont\\{reportName.Replace(" ", "_")}";
        if (Directory.Exists(path + $"\\documents-cont"))
        {
            using var outputFileStream = new FileStream(file, FileMode.Create);
            inputStream.CopyTo(outputFileStream);
        }
        else
        {
            Directory.CreateDirectory(path + $"\\documents-cont");
            using var outputFileStream = new FileStream(file, FileMode.Create);
            inputStream.CopyTo(outputFileStream);
        }
        inputStream.Dispose();

        if (string.IsNullOrEmpty(docxUrl)) return false;

        Process.Start(@$"""{docxUrl}""", file);

        return true;

    }
}