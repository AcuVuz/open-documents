using System;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace open_documents;

public partial class MainWindow 
{
    private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public MainWindow()
    {
        InitializeComponent();
    }

    private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!File.Exists(Path.Combine(AppData, "settings.json"))) 
        {
            var settings = new Settings {  Word = "", Excel = ""};
            using var createStream = File.Create(Path.Combine(AppData, "settings.json"));
            JsonSerializer.Serialize(createStream, settings);
        }
        
        var json = JsonSerializer.Deserialize<Settings>(File.ReadAllText($"{AppData}\\settings.json"));
        DocxUrl.Text = json?.Word;
        ExcelUrl.Text = json?.Excel;

    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(DocxUrl.Text) || string.IsNullOrEmpty(ExcelUrl.Text))
        {
            MessageBox.Show("Данные не заполнены!");
            return;
        }
        
        var settings = new Settings { Word = DocxUrl.Text, Excel = ExcelUrl.Text};
        var settingsText = JsonSerializer.Serialize(settings);
                
        await File.WriteAllTextAsync(Path.Combine(AppData, "settings.json"), settingsText);
        
        Application.Current.Shutdown();
    }
}

