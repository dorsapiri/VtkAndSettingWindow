using System.Configuration;
using System.Data;
using System.Windows;
using VtkAndSettingWindow_sample.View;
using VtkAndSettingWindow_sample.ViewModel;

namespace VtkAndSettingWindow_sample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            VtkWindowVM vtkWindowVM = new VtkWindowVM();
            VtkWindow vtkWindow = new VtkWindow();
            vtkWindow.DataContext = vtkWindowVM;
            vtkWindow.Show();
        }
    }

}
