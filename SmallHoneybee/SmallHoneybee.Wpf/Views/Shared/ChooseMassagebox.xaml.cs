using System.Windows;
using System.Windows.Input;

namespace SmallHoneybee.Wpf.Views.Shared
{
    /// <summary>
    /// ChooseMassagebox.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseMassagebox : Window
    {
        private Window _mainWindow;
        public ChooseMassagebox(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            InitializeComponent();
        }

        private void btnRegin_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)closeStyle.IsChecked)
            {
                this.Close();
                _mainWindow.WindowState = WindowState.Minimized;
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
