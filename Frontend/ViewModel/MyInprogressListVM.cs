using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    class MyInprogressListVM : NotifiableObject
    {

        private Model.BackendController controller;
        private MUser user;
        private List<MTask> tasks;
        public List<MTask> Tasks
        {
            get => tasks;
            set { tasks = value;  RaisePropertyChanged("Tasks"); }
        }

        private MTask selectedTask;
        public MTask SelectedTask
        {
            get => selectedTask;
            set { selectedTask = value; EnableForward = value != null; RaisePropertyChanged("SelectedTask"); }
        }
        private bool enableForward = false;
        public bool EnableForward
        {
            get => enableForward;
            private set { enableForward = value; RaisePropertyChanged("EnableForward"); }
        }
        private string errorLable = "";
        public string ErrorLable
        {
            get => errorLable;
            set { errorLable = value; RaisePropertyChanged("ErrorLable"); }
        }

        internal MyInprogressListVM(MUser user)
        {
            this.user = user;
            this.controller = user.Controller;
            tasks = controller.InProgressTasks(user.Email);
            
        }

        internal void Logout()
        {
            controller.Logout(user.Email);
        }
    }
}
