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
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            string userName = tbUserName.Text.Trim();
            string password = pbPassword.Password;
            string fullName = tbFullName.Text.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName))
            {
                tbMessage.Text = "Все поля обязательны";
                return;
            }

            var exist = DBClass.connect.Users.FirstOrDefault(u => u.UserName == userName);
            if (exist != null)
            {
                tbMessage.Text = "Пользователь с таким логином уже существует";
                return;
            }

            Users newUser = new Users
            {
                UserName = userName,
                Passwords = password,
                FullName = fullName,
                RoleID = 3,
                IsActive = true
            };
            DBClass.connect.Users.Add(newUser);
            DBClass.connect.SaveChanges();

            tbMessage.Text = "Регистрация успешна! Теперь войдите.";
            tbMessage.Foreground = System.Windows.Media.Brushes.Green;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }
    }
}
