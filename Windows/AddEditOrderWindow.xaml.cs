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
using System.Windows.Shapes;

namespace Restik.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddEditOrderWindow.xaml
    /// </summary>
    public partial class AddEditOrderWindow : Window
    {
        private Orders editingOrder;
        private bool isEdit = false;

        public AddEditOrderWindow()
        {
            InitializeComponent();
            LoadData();
        }

        public AddEditOrderWindow(int orderId)
        {
            InitializeComponent();
            editingOrder = DBClass.connect.Orders.Find(orderId);
            if (editingOrder != null)
            {
                isEdit = true;
                LoadData();
                FillOrderDetails();
            }
        }

        private void LoadData()
        {
            var statuses = DBClass.connect.Status.ToList();
            cmbStatus.ItemsSource = statuses;
            cmbStatus.DisplayMemberPath = "Name";
            cmbStatus.SelectedValuePath = "ID";

            var waiters = DBClass.connect.Users.Where(u => u.RoleID == 3 && u.IsActive == true).ToList();
            cmbWaiter.ItemsSource = waiters;
            cmbWaiter.DisplayMemberPath = "FullName";
            cmbWaiter.SelectedValuePath = "ID";

            var dishes = DBClass.connect.Dishes.Where(d => d.IsAvailable == true).ToList();
            var comboColumn = dgOrderItems.Columns[0] as DataGridComboBoxColumn;
            comboColumn.ItemsSource = dishes;
        }

        private void FillOrderDetails()
        {
            tbTable.Text = editingOrder.TableNumber.ToString();
            tbCustomers.Text = editingOrder.CustomerCount.ToString();
            cmbStatus.SelectedValue = editingOrder.StatusID;
            cmbWaiter.SelectedValue = editingOrder.UserID;
            tbNotes.Text = editingOrder.Notes;

            var items = editingOrder.OrderItems.ToList();
            dgOrderItems.ItemsSource = items;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tbTable.Text, out int table))
            {
                tbMessage.Text = "Некорректный номер столика";
                return;
            }
            if (!int.TryParse(tbCustomers.Text, out int customers))
            {
                tbMessage.Text = "Некорректное количество гостей";
                return;
            }
            if (cmbStatus.SelectedValue == null)
            {
                tbMessage.Text = "Выберите статус";
                return;
            }
            if (cmbWaiter.SelectedValue == null)
            {
                tbMessage.Text = "Выберите официанта";
                return;
            }

            var orderItems = dgOrderItems.ItemsSource as List<OrderItems>;
            if (orderItems == null || !orderItems.Any())
            {
                tbMessage.Text = "Добавьте хотя бы одно блюдо в заказ";
                return;
            }

            decimal total = orderItems.Sum(i => i.Subtotal ?? 0);

            string orderNumber;
            if (!isEdit)
            {
                int nextNumber = DBClass.connect.Orders.Count() + 1;
                orderNumber = $"ORD-{DateTime.Now:yyyyMMdd}-{nextNumber:D3}";
            }
            else
            {
                orderNumber = editingOrder.OrderNumber;
            }

            if (!isEdit)
            {
                editingOrder = new Orders
                {
                    OrderNumber = orderNumber,
                    TableNumber = table,
                    CustomerCount = customers,
                    OrderDate = DateTime.Now.Date,
                    StatusID = (int)cmbStatus.SelectedValue,
                    UserID = (int)cmbWaiter.SelectedValue,
                    TotalAmount = total,
                    Notes = tbNotes.Text.Trim()
                };
                DBClass.connect.Orders.Add(editingOrder);
            }
            else
            {
                editingOrder.TableNumber = table;
                editingOrder.CustomerCount = customers;
                editingOrder.StatusID = (int)cmbStatus.SelectedValue;
                editingOrder.UserID = (int)cmbWaiter.SelectedValue;
                editingOrder.TotalAmount = total;
                editingOrder.Notes = tbNotes.Text.Trim();

                var oldItems = DBClass.connect.OrderItems.Where(i => i.OrderID == editingOrder.ID).ToList();
                foreach (var item in oldItems)
                    DBClass.connect.OrderItems.Remove(item);
            }

            foreach (var item in orderItems)
            {
                item.OrderID = editingOrder.ID;
                item.Subtotal = item.Quantity * item.UnitPrice;
                DBClass.connect.OrderItems.Add(item);
            }

            try
            {
                DBClass.connect.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                tbMessage.Text = "Ошибка: " + ex.Message;
            }
        }
    }
}
