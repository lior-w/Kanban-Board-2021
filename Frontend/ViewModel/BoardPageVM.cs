using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    class BoardPageVM : NotifiableObject
    {
        private Model.BackendController controller;
        public MUser user;
        public MBoard Board { get; private set; }

        private string title;
        public string Title
        {
            get => title;
            set { title = value; RaisePropertyChanged("Title"); }
        }
        private string subtitle;
        public string Subtitle
        {
            get => subtitle;
            set { subtitle = value; RaisePropertyChanged("Subtitle"); }
        }

        private MColumn selectedColumn;
        public MColumn SelectedColumn
        {
            get => selectedColumn;
            set { selectedColumn = value; if (value!=null) ListOfTasks = value.TasksList.ToList(); RaisePropertyChanged("SelectedColumn"); }
        }

        private MTask selectedTask;
        public MTask SelectedTask
        {
            get => selectedTask;
            set { selectedTask = value; RaisePropertyChanged("SelectedTask"); }
        }
        private string newColumnOrdinal = "";
        public string NewColumnOrdinal
        {
            get => newColumnOrdinal;
            set { newColumnOrdinal = value; RaisePropertyChanged("NewColumnOrdinal"); }
        }

        private string newColumnName = "";
        public string NewColumnName
        {
            get => newColumnName;
            set { newColumnName = value; RaisePropertyChanged("NewColumnName"); }
        }

        private string moveColumnOrdinal = "";
        public string MoveColumnOrdinal
        {
            get => moveColumnOrdinal;
            set { moveColumnOrdinal = value; RaisePropertyChanged("MoveColumnOrdinal"); }
        }

        private string message = "";
        public string Message
        {
            get => message;
            set { message = value; RaisePropertyChanged("Message"); }
        }

        private string keyString = "";
        public string KeyString
        {
            get => keyString;
            set { keyString = value; RaisePropertyChanged("KeyString"); }
        }

        private string messageColor = "Red";
        public string MessageColor
        {
            get => messageColor;
            set { messageColor = value; RaisePropertyChanged("MessageColor"); }
        }

        private List<MTask> listOfTasks;
        public List<MTask> ListOfTasks
        {
            get => listOfTasks;
            set { listOfTasks = value; SortList();  RaisePropertyChanged("ListOfTasks"); }
        }

        public int compareByDueDate(MTask t1, MTask t2)
        {
             return t1.DueDate.CompareTo(t2.DueDate);
        }
        public void SortList()
         {
            ListOfTasks.Sort(compareByDueDate);
         }
        

        internal void renameColumn()
        {
            try
            {
                controller.RenameColumn(user.Email, Board.Creator_email, Board.Name, SelectedColumn.ColumnOrdinal, SelectedColumn.Name);
                Message = "changed to column's Name successfully";
                MessageColor = "Green";
            }
            catch(Exception e)
            {
                Message = e.Message;
                MessageColor = "Red";
            }
        }

        public BoardPageVM(MUser user, MBoard board)
        {
            this.user = user;
            this.Board = board;
            this.controller = user.Controller;
            Title = board.Name;
            Subtitle = "created by " + board.Creator_email;
            SelectedColumn = Board.Columns[0];
        }

        internal void Filter()
        {
            var filterd = ListOfTasks.Where(t => (t.Title.Contains(KeyString) || t.Description.Contains(KeyString)));
            List<MTask> filterdList = new List<MTask>();
            foreach (MTask t in filterd)
            {
                if (!filterdList.Contains(t)) filterdList.Add(t);
            }
            ListOfTasks = filterdList;

        }

        internal void ChangeColLim()
        {
            try
            {
                controller.LimitColumn(user.Email, Board.Creator_email, Board.Name, SelectedColumn.ColumnOrdinal, SelectedColumn.MaxTasks);
                Message = "changed to column's Limit successfully";
                MessageColor = "Green";
            }
            catch(Exception e)
            {
                Message = e.Message;
                MessageColor = "Red";
            }
        }

        internal void removeColumn()
        {
            MColumn toRemove = SelectedColumn;
            try
            {
                if (toRemove.ColumnOrdinal < Board.Columns.Count-1) SelectedColumn = Board.Columns[toRemove.ColumnOrdinal + 1];
                else SelectedColumn =  Board.Columns[toRemove.ColumnOrdinal - 1]; 
                controller.RemoveColumn(user.Email, Board.Creator_email, Board.Name, toRemove.ColumnOrdinal);
                Board.RemoveColumn(toRemove);
                SelectedColumn.TasksList = new ObservableCollection<MTask> (controller.GetColumn(user.Email, Board.Creator_email, Board.Name, SelectedColumn.ColumnOrdinal));
                ListOfTasks = SelectedColumn.TasksList.ToList();
                Message = "Removes column successfully";
                MessageColor = "Green";
            }
            catch(Exception e)
            {
                Message = e.Message;
                MessageColor = "Red";
                SelectedColumn = toRemove;
            }
        }

        internal void MoveColumn()
        {
            try
            {
                controller.MoveColumn(user.Email, Board.Creator_email, Board.Name, SelectedColumn.ColumnOrdinal, Int32.Parse(MoveColumnOrdinal)- SelectedColumn.ColumnOrdinal);
                Board.MoveColumn(SelectedColumn, Int32.Parse(MoveColumnOrdinal));
                Message = "Moves column successfully";
                MessageColor = "Green";
            }
            catch (Exception e)
            {
                Message = e.Message;
                MessageColor = "Red";
            }
        }

        internal void addNewColumn()
        {
            try
            {
                controller.AddColumn(user.Email, Board.Creator_email, Board.Name, Int32.Parse(newColumnOrdinal), newColumnName);
                Board.addColumn(controller.GetColumn(Board.Creator_email, Board.Name, Int32.Parse(newColumnOrdinal)));
                Message = "Column added successfully";
                MessageColor = "Green";
            }
            catch (Exception e)
            {
                Message = e.Message;
                MessageColor = "Red";
            }
        }

        internal void advanceTask()
        {
            try
            {
                controller.AdvanceTask(user.Email, Board.Creator_email, Board.Name, SelectedColumn.ColumnOrdinal, SelectedTask.Id);
                Board.Columns[SelectedColumn.ColumnOrdinal + 1].TasksList.Add(SelectedTask);
                SelectedColumn.TasksList.Remove(SelectedTask);
                Message = "Task advances successfully";
                MessageColor = "Green";
            }
            catch (Exception e)
            {
                Message = e.Message;
                MessageColor = "Red";
            }
        }

        internal void Logout()
        {
            controller.Logout(user.Email);
        }
    }
}
