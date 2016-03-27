using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SmallHoneybee.Common;
using SmallHoneybee.DataModel.Model;
using SmallHoneybee.Wpf.Common;
using SmallHoneybee.Wpf.Views.Shared;
using Produce = SmallHoneybee.Wpf.Views.Produce;
using PurchaseOrder = SmallHoneybee.Wpf.Views.PurchaseOrder;
using SaleOrder = SmallHoneybee.Wpf.Views.SaleOrder;

namespace SmallHoneybee.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            frame_run.Content = new Views.SaleOrder();

            SamllHoneybeeSaleOrderMenu.IsChecked = true;
            DataContext = new
            {
                ResourcesHelper.CurrentUserRolePermission
            };

            TxtCurrentUser.Text = string.Format("当前登录用户: {0}", ResourcesHelper.CurrentUser.Name);
            //MaxWindows.RepairWindowBehavior(this);
        }

        private void RTReportMenuNavigation_Click(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new SaleOrder();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void HSReportMenuNavigation_Click(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new Produce();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void PollutePortMenuNavigation_Click(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new PurchaseOrder();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void ServiceCommMenuNavigation_Click(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new Views.User();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void ClearMeunChecked()
        {
            var menuItems = SamllHoneybeeMenu.Items.SourceCollection.GetEnumerator();
            while (menuItems.MoveNext())
            {
                var item = (MenuItem)menuItems.Current;
                if (item.IsChecked)
                {
                    item.IsChecked = false;
                }
            }
        }

        private void SamllHoneybeeSystemSettingMenu_OnClick(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new Views.SystemSetting();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void SamllHoneybeeDayBookMenu_OnClick(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new Views.DayBook();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void ButExit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ChooseMassagebox chooseMassagebox = new ChooseMassagebox(this);
            chooseMassagebox.Show();
        }

        private void SamllHoneybeeMemberCardMenu_OnClick(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new Views.MemberCard();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }

        private void ButMin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            ChooseMassagebox chooseMassagebox = new ChooseMassagebox(this);
            chooseMassagebox.Show();
        }

        private void SamllHoneybeeMemberCardLogMenu_OnClick(object sender, RoutedEventArgs e)
        {
            ClearMeunChecked();

            frame_run.Content = new Views.MemberCardDetail();
            var menuItem = (MenuItem)sender;
            menuItem.IsChecked = true;
        }
    }
}
