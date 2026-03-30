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

namespace Restik.Pages
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string userName = tbUserName.Text.Trim();
            string password = pbPassword.Password;

            var user = DBClass.connect.Users.FirstOrDefault(u => u.UserName == userName && u.Passwords == password);
            if (user != null && user.IsActive == true)
            {
                CurrentUser.ID = user.ID;
                CurrentUser.UserName = user.UserName;
                CurrentUser.FullName = user.FullName;
                CurrentUser.RoleID = user.RoleID;
                CurrentUser.IsAuthenticated = true;

                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow.ShowNavigation();
                NavigationService.Navigate(new DishesPage());
            }
            else
            {
                tbMessage.Text = "Неверное имя пользователя или пароль";
            }
        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }
    }
}
