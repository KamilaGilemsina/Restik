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
using Restik.Windows;
using System.Data.Entity;

namespace Restik.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        private List<Orders> allOrders = new List<Orders>();

        public OrdersPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
                btnAdd.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private void LoadData()
        {
            allOrders = DBClass.connect.Orders
                .Include(o => o.Status)
                .Include(o => o.Users)
                .ToList();

            var displayList = allOrders.Select(o => new
            {
                o.ID,
                o.OrderNumber,
                o.TableNumber,
                o.CustomerCount,
                o.OrderDate,
                o.TotalAmount,
                StatusName = o.Status?.Name ?? "Неизвестно",
                WaiterName = o.Users?.FullName ?? "Неизвестно"
            }).ToList();

            dgOrders.ItemsSource = displayList;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditOrderWindow();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.RoleID != 1 && CurrentUser.RoleID != 2)
            {
                MessageBox.Show("Только администратор или менеджер могут редактировать заказы", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int id = (int)((Button)sender).Tag;
            var window = new AddEditOrderWindow(id);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.RoleID != 1 && CurrentUser.RoleID != 2)
            {
                MessageBox.Show("Только администратор или менеджер могут удалять заказы",
                                "Доступ запрещен",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            int id = (int)((Button)sender).Tag;
            var order = DBClass.connect.Orders.Find(id);
            if (order == null) return;

            var result = MessageBox.Show($"Удалить заказ #{order.OrderNumber}?\nВсе позиции заказа также будут удалены.",
                                         "Подтверждение",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            try
            {
                var orderItems = DBClass.connect.OrderItems.Where(oi => oi.OrderID == id).ToList();

                foreach (var item in orderItems)
                {
                    DBClass.connect.OrderItems.Remove(item);
                }

                DBClass.connect.Orders.Remove(order);

                DBClass.connect.SaveChanges();

                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
