using Frontend.Model;
using Frontend.ViewModel;
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
    /// Interaction logic for MyInprogressList.xaml
    /// </summary>
    public partial class MyInprogressList : Window
    {
        private MUser user;
        private MyInprogressListVM myinprogersslistVM;
        internal MyInprogressList(MUser user)
        {
            InitializeComponent();
            this.user = user;
            myinprogersslistVM = new MyInprogressListVM(user);
            this.DataContext = myinprogersslistVM;
        }


        private void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            myinprogersslistVM.Logout();
            var loginPage = new Login();
            loginPage.Show();
            this.Close();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            var homePage = new HomePage(user);
            homePage.Show();
            this.Close();
        }
    }
}
