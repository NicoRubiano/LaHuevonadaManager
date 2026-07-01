using System.Windows;
using LaHuevonadaManager.Views;

namespace LaHuevonadaManager;

public partial class App : Application
{
    private async void Application_Startup(object sender, StartupEventArgs e)
    {
        var splash = new SplashWindow();
        splash.Show();
        await Task.Delay(1700);
        var start = new StartWindow();
        start.Show();
        splash.Close();
    }
}
