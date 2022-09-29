using System;
using System.IO;
using System.Text.Json;
using System.Windows;
using open_documents.Services;

namespace open_documents;

public partial class App 
{
    private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private async void AppStartup(object sender, StartupEventArgs e)
    {
        if (e.Args.Length <= 0)
        {
            var mainWindow = new MainWindow();
            // Если реестр не найден, тогда создать
            if (!FileService.CheckClasses())
            {
                // Создать реестр
                FileService.CreateRegister();
            }
            mainWindow.Show();
        }
        else
        {
            var split = e.Args[0].Split("jmuagent://");
            try
            {
                var jsonSettings = JsonSerializer.Deserialize<Settings>(await File.ReadAllTextAsync($"{AppData}\\settings.json"));
                
                if (split[1].Contains("word"))
                {
                    var result = split[1].Split("word");
                    // Делаем запрос на программу
                    await ApiService.JsonPostWithToken(
                            "secret",
                            "http://jmu.api.lgpu.org/reports-education/" + result[1],
                            "GET",
                            "Отчет" ,
                            "word", 
                            jsonSettings?.Word);
                        //.ConfigureAwait(false);
                        
                }
                else if (split[1].Contains("excel"))
                {
                    var result = split[1].Split("excel");
                    // Делаем запрос на программу
                    await ApiService.JsonPostWithToken(
                            "secret",
                            "http://jmu.api.lgpu.org/reports-education/" + result[1],
                            "GET",
                            "Отчет",
                            "excel",
                            jsonSettings?.Excel);
                       // .ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally 
            {
                Current.Shutdown();
            }
        }
        

    }
}

