using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.Objects
{
    public struct SColumn
    {
        private int maxTasks;
        public int MaxTasks
        {
            get { return maxTasks; }
            set { maxTasks = value; }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private int columnId;
        public int ColumnId
        {
            get { return columnId; }
        }

        private int boardId;
        internal int BoardId { get => boardId; }

        private int columnOrdinal;
        public int ColumnOrdinal
        {
            get { return columnOrdinal; }
            set { columnOrdinal = value; }
        }

        private List<STask> tasksList;
        public List<STask> TaskList

        {
            get { return tasksList; }
            set { tasksList = value; }
        }

        internal SColumn(int columnId, int boardId, int columnOrdinal, string name, int maxTasks, List<STask> taskList)
        {
            this.columnId = columnId;
            this.boardId = boardId;
            this.columnOrdinal = columnOrdinal;
            this.name = name;
            this.maxTasks = maxTasks;
            this.tasksList = taskList;
        }
    }
}
