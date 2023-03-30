using Frontend.Model;
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
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Window
    {
        private MUser user;
        private HomePageVM homepageVM;


        internal HomePage(MUser user)
        {
            InitializeComponent();
            this.homepageVM = new HomePageVM(user);
            this.user = user;
            this.DataContext = homepageVM;
        }

        private void MyBoards_Button_Click(object sender, RoutedEventArgs e)
        {
            var myBoardsPage = new MyBoards(user);
            myBoardsPage.Show();
            this.Close();
        }

        private void CreatBoard_Button_Click(object sender, RoutedEventArgs e)
        {
            homepageVM.CreateBoard();
        }

        private void InprogressList_Button_Click(object sender, RoutedEventArgs e)
        {
            var myInprogressListPage = new MyInprogressList(user);
            myInprogressListPage.Show();
            this.Hide();
        }

        private void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            homepageVM.Logout();
            var loginPage = new Login();
            loginPage.Show();
            this.Close();
        }

        private void JoinBoards_Button_Click(object sender, RoutedEventArgs e)
        {
            homepageVM.joinBoard();
        }

    }
}
