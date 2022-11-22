using System;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Windows;
using open_documents.Services;

namespace open_documents;

public partial class App 
{
    private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    private const string Host = "http://jmu.api.lgpu.org";

    private async void AppStartup(object sender, StartupEventArgs e)
    {
        if (e.Args.Length <= 0)
        {
            var mainWindow = new MainWindow();
            // Если реестр не найден, тогда создать
            //if (!FileService.CheckClasses())
            //{
                // Создать реестр
                FileService.CreateRegister();
           // }
            mainWindow.Show();
        }
        else
        {

            var split = e.Args[0].Split("jmuagent://");
            try
            {
                var jsonSettings = JsonSerializer.Deserialize<Settings>(await File.ReadAllTextAsync($"{AppData}\\settings.json"));
                // Миллионы сплитов
                // Если сохранить документ
                if (split[1].Contains("withSave"))
                {
                    split[1] = split[1].Replace("withSave", string.Empty);
     
                    if (split[1].Contains("word"))
                    {
                        split[1] = split[1].Replace("word/", string.Empty);
                        //MessageBox.Show(split[1]);
                        await ApiService.JsonModalPostWithToken("secret", Host + split[1], "GET", "Документ.docx", "word", jsonSettings?.Word);

                    }
                    else if (split[1].Contains("excel"))
                    {
                        split[1] = split[1].Replace("excel/", string.Empty);

                        //MessageBox.Show(split[1]);
                        await ApiService.JsonModalPostWithToken("secret", Host + split[1], "GET", "Документ.xlsx", "excel", jsonSettings?.Excel);

                    }
                }
                else
                {
                    if (split[1].Contains("word"))
                    {
                        split[1] = split[1].Replace("word", string.Empty);

                        // MessageBox.Show(split[1]);

                        await ApiService.JsonPostWithToken("secret", Host + split[1], "GET", "Документ.docx", "word", jsonSettings?.Word);

                    }
                    else if (split[1].Contains("excel"))
                    {
                        split[1] = split[1].Replace("excel", string.Empty);

                        await ApiService.JsonPostWithToken("secret", Host + split[1], "GET", "Документ.xlsx", "excel", jsonSettings?.Excel);

                    }
                }
            }
            catch (WebException ex)
            {
                
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    if (ex.Response is HttpWebResponse response)
                    {
                        using StreamReader reader = new(response.GetResponseStream());
                        var message = await reader.ReadToEndAsync();
                        _ = MessageBox.Show(message);
                    }
                }
                else
                {
                    _ = MessageBox.Show(ex.Message);
                }
            }
            finally 
            {
                Current.Shutdown();
            }
        }
        

    }
}

