using Restik.DB;
using System;
using System.Collections.Generic;
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
using Restik.Pages;

namespace Restik
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new LoginPage());
        }

        private void btnDishes_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DishesPage());
        }

        private void btnOrders_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new OrdersPage());
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            CurrentUser.Clear();
            MainFrame.Navigate(new LoginPage());
            btnDishes.Visibility = Visibility.Collapsed;
            btnOrders.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Collapsed;
        }

        public void ShowNavigation()
        {
            btnDishes.Visibility = Visibility.Visible;
            btnOrders.Visibility = (CurrentUser.RoleID == 1 || CurrentUser.RoleID == 2)
                                   ? Visibility.Visible
                                   : Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Visible;
        }
    }
}
