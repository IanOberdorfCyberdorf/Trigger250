using System.Windows;

namespace Trigger250
{
    public partial class App : System.Windows.Application // Explicitly specify WPF Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}