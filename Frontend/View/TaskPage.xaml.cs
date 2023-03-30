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
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Window
    {
        private MUser user;
        private MTask task;
        private MBoard board;
        private TaskPageVM taskpageVM;
        internal TaskPage(MUser user, MTask task, MBoard board, int columnOrdinal)
        {
            InitializeComponent();
            this.user = user;
            this.task = task;
            this.board = board;
            this.taskpageVM = new TaskPageVM(user, task, board, columnOrdinal);
            this.DataContext = taskpageVM;
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            taskpageVM.Logout();
            var loginPage = new Login();
            loginPage.Show();
            this.Close();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            var boardPage = new BoardPage(user, board);
            boardPage.Show();
            this.Close();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            taskpageVM.commitChanges();
        }
    }
}
