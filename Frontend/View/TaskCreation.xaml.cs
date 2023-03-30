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
    /// Interaction logic for TaskCreation.xaml
    /// </summary>
    public partial class TaskCreation : Window
    {
        private MUser user;
        private MBoard board;
        private TaskCreationVM taskcreationVM;



        internal TaskCreation(MUser user, MBoard board)
        {
            InitializeComponent();
            taskcreationVM = new TaskCreationVM(user, board);
            this.DataContext = taskcreationVM;
            this.board = board;
            this.user = user;

        }

        private void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            taskcreationVM.Logout();
            var loginPage = new Login();
            loginPage.Show();
            this.Close();
        }

        private void CreateTask_Click(object sender, RoutedEventArgs e)
        {
            if (taskcreationVM.CreateTask())
            {
                var boardPage = new BoardPage(user, board);
                boardPage.Show();
                this.Close();
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            var boardPage = new BoardPage(user, board);
            boardPage.Show();
            this.Close();
        }
        
    }
}
