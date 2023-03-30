using Frontend.ViewModel;
using IntroSE.Kanban.Backend.ServiceLayer;
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

namespace Frontend.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private LoginVM loginVM;
        public Login()
        {
            InitializeComponent();
            this.loginVM = new LoginVM();
            this.DataContext = loginVM;
        }

        private void Login_Button_Click(object sender, RoutedEventArgs e)
        {
            var user = loginVM.Login();
            if (user==null) { }
            else
            {
                var homepage = new HomePage(user);
                homepage.Show();
                this.Close();
            }
        }

        private void Regieser_Button_Click(object sender, RoutedEventArgs e)
        {
            loginVM.Register();
        }
    }
}
