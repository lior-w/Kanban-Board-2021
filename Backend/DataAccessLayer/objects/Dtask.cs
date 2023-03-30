using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class Dtask: DTO
    {
        private int taskID;
        public int TaskID { get =>  taskID; }

        private string creationTime;
        public string CreationTime { get => creationTime; } 

        private string title;
        public string Title { get => title; }

        private string description;
        public string Description { get => description;  }

        private string dueDate;
        public string DueDate { get => dueDate; } 

        private string emailAssignee;
        public string EmailAssignee { get => emailAssignee; }

        private int columnID;
        public int ColumnID { get => columnID; }

        private int boardID;
        public int BoardID { get => boardID; }



        public Dtask(int taskID, string title, string description, string creationTime, string dueDate, string emailAssignee, int columnID, int boardID) : base(new BoardDalController())
        {
            this.taskID = taskID;
            this.creationTime = creationTime;
            this.title = title;
            this.description = description;
            this.dueDate = dueDate;
            this.emailAssignee = emailAssignee;
            this.columnID = columnID;
            this.boardID = boardID;
        }

    }
}
