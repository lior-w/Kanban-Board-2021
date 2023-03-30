using IntroSE.Kanban.Backend.business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.ServiceLayer.Objects
{
    public struct SBoard
    {
        private string name;
        public string Name { get => name; }
        private string creator_email;
        public string Creator_email { get => creator_email; }
        private List<SColumn> columns;
        public List<SColumn> Columns { get => columns; }


        private int boardID;
        public int BoardID
        {
            get { return boardID; }
            set { boardID = value; }
        }

        private List<string> boardMembers;
        public List<string> BoardMembers { set => boardMembers = value; get => boardMembers; }

        internal SBoard(string name, string email, int ID, List<SColumn> columns, List<string> boardMembers)
        {
            this.name = name;
            this.creator_email = email;
            this.boardID = ID;
            this.columns = columns;
            this.boardMembers = boardMembers;
        }
        
    }
}
