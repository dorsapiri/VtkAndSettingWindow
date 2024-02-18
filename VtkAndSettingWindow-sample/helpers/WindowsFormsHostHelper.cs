using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows;
using Kitware.VTK;

namespace VtkAndSettingWindow_sample.helpers
{
    public class WindowsFormsHostHelper
    {
        public static readonly DependencyProperty LoadedCommandProperty =
            DependencyProperty.RegisterAttached("LoadedCommand", typeof(ICommand),
                typeof(WindowsFormsHostHelper), new PropertyMetadata(OnLoadedCommandChanged));

        public static ICommand GetLoadedCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(LoadedCommandProperty);
        }

        public static void SetLoadedCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(LoadedCommandProperty, value);
        }

        private static void OnLoadedCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WindowsFormsHost host)
            {
                host.Loaded += (sender, args) =>
                {
                    ICommand command = GetLoadedCommand(host);
                    if (command != null && command.CanExecute(null))
                        command.Execute(null);
                };
            }
            
        }
    }
}
