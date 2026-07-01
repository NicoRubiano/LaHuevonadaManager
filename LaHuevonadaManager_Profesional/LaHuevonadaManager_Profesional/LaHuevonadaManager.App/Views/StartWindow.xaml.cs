using System.Windows;
using Microsoft.Win32;
using LaHuevonadaManager.Models;
using LaHuevonadaManager.Services;

namespace LaHuevonadaManager.Views;

public partial class StartWindow : Window
{
    private readonly FarmDocumentService _service = new();
    public StartWindow() => InitializeComponent();

    private async void CreateFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new SaveFileDialog { Filter = "Archivo LaHuevonada (*.lahuevonada)|*.lahuevonada", FileName = "MiGranja.lahuevonada" };
        if (dialog.ShowDialog() == true)
        {
            AppState.Document = new FarmDocument();
            AppState.CurrentFilePath = dialog.FileName;
            await _service.SaveAsync(dialog.FileName, AppState.Document);
            OpenMain();
        }
    }

    private async void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Filter = "Archivo LaHuevonada (*.lahuevonada)|*.lahuevonada" };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                AppState.Document = await _service.OpenAsync(dialog.FileName);
                AppState.CurrentFilePath = dialog.FileName;
                OpenMain();
            }
            catch (Exception ex) { MessageBox.Show($"No se pudo abrir el archivo: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
    }

    private void OpenMain()
    {
        new MainWindow().Show();
        Close();
    }
}
