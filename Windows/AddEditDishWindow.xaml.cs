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
    /// Логика взаимодействия для AddEditDishWindow.xaml
    /// </summary>
    public partial class AddEditDishWindow : Window
    {
        private Dishes editingDish;
        private bool isEdit = false;

        public AddEditDishWindow()
        {
            InitializeComponent();
            LoadCategories();
            chkAvailable.IsChecked = true;
        }

        public AddEditDishWindow(int dishId)
        {
            InitializeComponent();
            LoadCategories();
            editingDish = DBClass.connect.Dishes.Find(dishId);
            if (editingDish != null)
            {
                isEdit = true;
                tbName.Text = editingDish.Name;
                tbDescription.Text = editingDish.Description;
                tbPrice.Text = editingDish.Price.ToString();
                cmbCategory.SelectedValue = editingDish.CategoryID;
                chkAvailable.IsChecked = editingDish.IsAvailable;
            }
        }

        private void LoadCategories()
        {
            var cats = DBClass.connect.Categories.ToList();
            cmbCategory.ItemsSource = cats;
            if (cats.Any())
                cmbCategory.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                tbMessage.Text = "Введите название";
                return;
            }
            if (!decimal.TryParse(tbPrice.Text, out decimal price))
            {
                tbMessage.Text = "Некорректная цена";
                return;
            }
            if (cmbCategory.SelectedValue == null)
            {
                tbMessage.Text = "Выберите категорию";
                return;
            }

            if (!isEdit)
            {
                Dishes newDish = new Dishes
                {
                    Name = tbName.Text.Trim(),
                    Description = tbDescription.Text.Trim(),
                    Price = price,
                    CategoryID = (int)cmbCategory.SelectedValue,
                    IsAvailable = chkAvailable.IsChecked ?? false
                };
                DBClass.connect.Dishes.Add(newDish);
            }
            else
            {
                editingDish.Name = tbName.Text.Trim();
                editingDish.Description = tbDescription.Text.Trim();
                editingDish.Price = price;
                editingDish.CategoryID = (int)cmbCategory.SelectedValue;
                editingDish.IsAvailable = chkAvailable.IsChecked ?? false;
            }

            try
            {
                DBClass.connect.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                tbMessage.Text = "Ошибка сохранения: " + ex.Message;
            }
        }
    }
}
