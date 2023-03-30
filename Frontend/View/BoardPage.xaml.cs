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
    /// Interaction logic for BoardPage.xaml
    /// </summary>
    public partial class BoardPage : Window
    {
        private MUser user;
        private BoardPageVM boardpageVM;
        private MBoard board;

        internal BoardPage(MUser user, MBoard board)
        {
            InitializeComponent();
            this.board = board;
            this.user = user;
            boardpageVM = new BoardPageVM(user, board);
            this.DataContext = boardpageVM;
        }

        private void logout_Button_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.Logout();
            var loginPage = new Login();
            loginPage.Show();
            this.Close();
        }

        private void AddNewTask_Click(object sender, RoutedEventArgs e)
        {
            var taskCreationPage = new TaskCreation(user, board);
            taskCreationPage.Show();
            this.Close();
        }

        private void AdvanceTask_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.advanceTask();
        }

        private void AddNewColumn_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.addNewColumn();
        }

        private void removeColumn_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.removeColumn();
        }


        private void EditTask_Click(object sender, RoutedEventArgs e)
        {
            var TaskPage = new TaskPage(user, boardpageVM.SelectedTask, board, boardpageVM.SelectedColumn.ColumnOrdinal);
            TaskPage.Show();
            this.Close();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            var myBoardsPage = new MyBoards(user);
            myBoardsPage.Show();
            this.Close();
        }

        private void MoveColumn_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.MoveColumn();
        }

        private void changeColumnsLim_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.ChangeColLim();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.Filter();
        }

        private void RenameColumn_Click(object sender, RoutedEventArgs e)
        {
            boardpageVM.renameColumn();
        }
    }
}
