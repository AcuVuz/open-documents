using System;
using System.Diagnostics;
using System.IO;
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

    public static bool SaveReportDocx(Stream inputStream, string reportName, string docxUrl)
    {
        // Получить путь рабочего стола
        const string path = "D:";

        var file = path + $"\\documents-cont\\{reportName.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.docx";
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
        _ = Process.Start(@$"""{docxUrl}""", file);
        
       /* if (MessageBox.Show("Сохранить файл?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) !=
            MessageBoxResult.No) return true;
        File.Delete(file);
        return false;
        */
       return true;
    }
    public static bool SaveReportExcel(Stream inputStream, string reportName , string docxUrl)
    {
        // Получить путь рабочего стола
        const string path = "D:";

        var file = path + $"\\documents-cont\\{reportName.Replace(" ", "_")}_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.xlsx";
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
        _ = Process.Start(@$"""{docxUrl}""", file);
        return true;

    }
}