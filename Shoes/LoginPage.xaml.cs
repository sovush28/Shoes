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

namespace Shoes
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

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            if(LoginTB.Text.Trim()==""||PasswTB.Text.Trim() == "")
            {
                MessageBox.Show("Заполните пустые поля");
                return;
            }

            User user = ShoesDE2026Entities.GetContext().User.ToList().Find(u => u.UserLogin == LoginTB.Text.Trim() && u.UserPassword == PasswTB.Text.Trim());
            if (user == null)
            {
                MessageBox.Show("Введены неверные данные");
                return;
            }

            Manager.MainFrame.Navigate(new ProductPage(user));
        }

        private void LoginGuestBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            LoginTB.Text = "";
            PasswTB.Text = "";
        }
    }
}
