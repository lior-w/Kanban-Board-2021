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
    /// Interaction logic for MyBoards.xaml
    /// </summary>
    public partial class MyBoards : Window
    {
        private MUser user;
        private MyBoardsVM myboardsVM;
        internal MyBoards(MUser user)
        {
            InitializeComponent();
            this.user = user;
            myboardsVM = new MyBoardsVM(user);
            this.DataContext = myboardsVM;
        }

        private void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            myboardsVM.Logout();
            var loginPage = new Login();
            loginPage.Show();
            this.Close();
        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            myboardsVM.deleteBoard();
        }

        private void Show_Button_Click(object sender, RoutedEventArgs e)
        {
            var boardPage = new BoardPage(user, myboardsVM.SelectedBoard);
            boardPage.Show();
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
