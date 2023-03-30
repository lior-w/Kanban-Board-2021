using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{

    class TaskCreationVM : NotifiableObject
    {
        private Model.BackendController controller;
        private MUser user;
        private MBoard board;

        private string title = "";
        public string Title
        {
            get => title;
            set { title = value; RaisePropertyChanged("Title"); }
        }

        private string description = "";
        public string Description
        {
            get => description;
            set { description = value; RaisePropertyChanged("Description"); }
        }

        private DateTime dueDate;
        public DateTime DueDate
        {
            get => dueDate;
            set { dueDate = value; RaisePropertyChanged("DueDate"); }
        }

        private string message = "";
        public string Message
        {
            get => message;
            set { message = value; RaisePropertyChanged("Message"); }
        }

        internal TaskCreationVM(MUser user, MBoard board)
        {
            this.user = user;
            this.controller = user.Controller;
            this.board = board;
        }

        public bool CreateTask()
        {
            try
            {
                board.Columns[0].TasksList.Add(controller.AddTask(user.Email, board.Creator_email, board.Name, Title, Description, DueDate));
                return true;
            }
            catch (Exception e)
            {
                Message = e.Message;
                return false;
            }
        }

        public void Logout()
        {
            controller.Logout(user.Email);
        }
    }
}
