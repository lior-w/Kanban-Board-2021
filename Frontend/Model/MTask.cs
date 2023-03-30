using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    class MTask : NotifiableModelObject
    {
        public int Id { private set; get; }

        private DateTime creationTime;
        public DateTime CreationTime
        {
            get => creationTime;
            set { this.creationTime = value; RaisePropertyChanged("CreationTime"); }
        }
        private string title;
        public string Title
        {
            get => title;
            set { this.title = value; RaisePropertyChanged("Title"); }
        }
        private string description;
        public string Description
        {
            get => description;
            set { this.description = value; RaisePropertyChanged("Description"); }
        }
        private DateTime dueDate;
        public DateTime DueDate
        {
            get => dueDate;
            set { this.dueDate = value; RaisePropertyChanged("DueDate"); }
        }
        private string emailAssignee;
        public string EmailAssignee
        {
            get => emailAssignee;
            set { this.emailAssignee = value; RaisePropertyChanged("EmailAssignee"); }
        }

        private string taskColor = "LightGray";
        public string TaskColor
        {
            get => taskColor;
            set { this.taskColor = value; RaisePropertyChanged("TaskColor"); }
        }
        private string isWorkedOn = "LightGray";
        public string IsWorkedOn
        {
            get => isWorkedOn;
            set { this.isWorkedOn = value; RaisePropertyChanged("IsWorkedOn"); }
        }
        



        public MTask(BackendController bc, DateTime creationTime, string title, string description, DateTime DueDate, string emailAssignee, int id) : base (bc)
        {
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.Id = id;
            this.EmailAssignee = emailAssignee;
            if (bc.ValidateUserLoggedIn(emailAssignee)) IsWorkedOn = "Blue";
            if (DateTime.Now.CompareTo(CreationTime + 0.75 * (DueDate - CreationTime)) > 0) TaskColor = "Orange";
            if (DateTime.Now.CompareTo(DueDate) > 0) TaskColor = "Red";
        }

        public MTask(BackendController controller, STask task) : base(controller)
        {
            this.CreationTime = task.CreationTime;
            this.Title = task.Title;
            this.Description = task.Description;
            this.DueDate = task.DueDate;
            this.Id = task.Id;
            this.EmailAssignee = task.emailAssignee;
            if (controller.ValidateUserLoggedIn(emailAssignee)) IsWorkedOn = "Blue";
            if (DateTime.Now.CompareTo(CreationTime + 0.75 * (DueDate - CreationTime)) > 0) TaskColor = "Orange";
            if (DateTime.Now.CompareTo(DueDate) > 0) TaskColor = "Red";
        }
    }
}
