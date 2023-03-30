using IntroSE.Kanban.Backend.ServiceLayer;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    class MColumn: NotifiableModelObject
    {
        private int maxTasks;
        public int MaxTasks
        {
            get { return maxTasks; }
            set { maxTasks = value; RaisePropertyChanged("MaxTasks"); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; RaisePropertyChanged("Name"); }
        }
        public int ColumnId { private set; get; }

        private int columnOrdinal;
        public int ColumnOrdinal
        {
            get { return columnOrdinal; }
            set { columnOrdinal = value; RaisePropertyChanged("ColumnOrdinal"); }
        }

        private ObservableCollection<MTask> tasksList;
        public ObservableCollection<MTask> TasksList{ get; set; }

        internal MColumn(BackendController controller, int columnId, int columnOrdinal, string name, int maxTasks, List<STask> taskList) :base(controller)
        {
            this.ColumnId = columnId;
            this.ColumnOrdinal = columnOrdinal;
            this.Name = name;
            this.MaxTasks = maxTasks;
            this.TasksList = new ObservableCollection<MTask>(taskList.
                Select((t, i) => new MTask(controller, t)).ToList());
        }

        internal MColumn(BackendController controller, SColumn column) : base(controller)
        {
            this.ColumnId = column.ColumnId;
            this.ColumnOrdinal = column.ColumnOrdinal;
            this.Name = column.Name;
            this.MaxTasks = column.MaxTasks;
            this.TasksList = new ObservableCollection<MTask>(column.TaskList.
                    Select((t, i) => new MTask(controller, t)).ToList());
        }

    }
}
