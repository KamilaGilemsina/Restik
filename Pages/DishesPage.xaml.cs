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
    /// Логика взаимодействия для DishesPage.xaml
    /// </summary>
    public partial class DishesPage : Page
    {
        private List<Dishes> allDishes = new List<Dishes>();

        public DishesPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();

                if (CurrentUser.RoleID == 1 || CurrentUser.RoleID == 2)
                    btnAdd.Visibility = Visibility.Visible;
                else
                    btnAdd.Visibility = Visibility.Collapsed;

                var categories = DBClass.connect.Categories.ToList();
                if (!categories.Any())
                {
                    MessageBox.Show("В базе нет категорий! Выполните скрипт 1.sql.");
                    return;
                }
                categories.Insert(0, new Categories { ID = 0, Name = "Все категории" });
                cmbCategory.ItemsSource = categories;
                cmbCategory.DisplayMemberPath = "Name";
                cmbCategory.SelectedValuePath = "ID";
                cmbCategory.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void LoadData()
        {
            allDishes = DBClass.connect.Dishes.Include(d => d.Categories).ToList();
            UpdateDishesList(allDishes);
        }

        private void UpdateDishesList(IEnumerable<Dishes> dishes)
        {
            if (dishes == null)
            {
                lvDishes.ItemsSource = new List<object>();
                return;
            }

            var displayList = dishes.Select(d => new
            {
                d.ID,
                d.Name,
                d.Description,
                d.Price,
                d.IsAvailable,
                CategoryName = d.Categories?.Name ?? "Без категории",
                CategoryImage = GetCategoryImage(d.Categories?.Name),
                d.CategoryID
            }).ToList();

            lvDishes.ItemsSource = displayList;
        }

        private string GetCategoryImage(string categoryName)
        {
            switch (categoryName)
            {
                case "Закуски": return "/Images/zaglushZak.jpg";
                case "Супы": return "/Images/zaglushSoap.jpg";
                case "Горячие блюда": return "/Images/zaglushHot.png";
                case "Десерты": return "/Images/zaglushDessert.png";
                case "Напитки": return "/Images/zaglushDrinks.jpg";
                default: return "/Images/icon.png";
            }
        }

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void cmbCategory_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();

        private void ApplyFilters()
        {
            if (allDishes == null) return;

            var filtered = allDishes.AsEnumerable();

            string searchText = tbSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
                filtered = filtered.Where(d => d.Name.Contains(searchText));

            var selectedCategory = cmbCategory.SelectedItem as Categories;
            if (selectedCategory != null && selectedCategory.ID != 0)
                filtered = filtered.Where(d => d.CategoryID == selectedCategory.ID);

            switch (cmbSort.SelectedIndex)
            {
                case 1: filtered = filtered.OrderBy(d => d.Price); break;
                case 2: filtered = filtered.OrderByDescending(d => d.Price); break;
                default: filtered = filtered.OrderBy(d => d.Name); break;
            }

            UpdateDishesList(filtered);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var window = new AddEditDishWindow();
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void EditDish_Click(object sender, RoutedEventArgs e)
        {
            int id = (int)((Button)sender).Tag;
            var window = new AddEditDishWindow(id);
            window.Owner = Application.Current.MainWindow;
            if (window.ShowDialog() == true)
                LoadData();
        }

        private void DeleteDish_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUser.RoleID != 1 && CurrentUser.RoleID != 2)
            {
                MessageBox.Show("У вас нет прав на удаление", "Доступ запрещён", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int id = (int)((Button)sender).Tag;
            var dish = DBClass.connect.Dishes.Find(id);
            if (dish != null && MessageBox.Show($"Удалить блюдо \"{dish.Name}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                DBClass.connect.Dishes.Remove(dish);
                DBClass.connect.SaveChanges();
                LoadData();
            }

        }
    }
}

