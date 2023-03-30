using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    class TaskPageVM : NotifiableObject
    {
        private Model.BackendController controller;
        private MUser user;
        private MBoard board;
        private int columnOrdinal;
        public MTask Task { get; set; }

        private List<String> members;
        public List<String> Members
        {
            get => members;
            set { members = value; RaisePropertyChanged("Members"); }
        }


        private string message = "";
        public string Message
        {
            get => message;
            set { message = value; RaisePropertyChanged("Message"); }
        }


        internal TaskPageVM(MUser user, MTask task, MBoard board, int columnOrdinal)
        {
            this.user = user;
            this.Task = task;
            this.board = board;
            this.columnOrdinal = columnOrdinal;
            this.controller = user.Controller;
            Members = board.BoardMembers;
        }

        internal void Logout()
        {
            controller.Logout(user.Email);
        }

        internal void commitChanges()
        {
            try
            {
                controller.UpdateTaskTitle(user.Email, board.Creator_email, board.Name, columnOrdinal, Task.Id, Task.Title);
                controller.UpdateTaskDescription(user.Email, board.Creator_email, board.Name, columnOrdinal, Task.Id, Task.Description);
                controller.UpdateTaskDueDate(user.Email, board.Creator_email, board.Name, columnOrdinal, Task.Id, Task.DueDate);
                controller.AssignTask(user.Email, board.Creator_email, board.Name, columnOrdinal, Task.Id, Task.EmailAssignee);
            }
            catch (Exception e)
            {
                Message = e.Message;
            }
        }
    }
}
