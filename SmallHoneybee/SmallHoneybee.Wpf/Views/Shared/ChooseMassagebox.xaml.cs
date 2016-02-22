using System.Windows;
using System.Windows.Input;

namespace SmallHoneybee.Wpf.Views.Shared
{
    /// <summary>
    /// ChooseMassagebox.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseMassagebox : Window
    {
        public int windowStyle = 1;
        public ChooseMassagebox()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnRegin_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)closeStyle.IsChecked)
            {
                windowStyle = 1;
            }
            else
            {
                windowStyle = 2;
            }
            this.Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            windowStyle = 0;
            this.Close();
        }
    }
}
