using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class Dboard: DTO
    {
        private int boardID;
        internal int BoardID { get => boardID; }

        private String name;
        internal String Name { get => name; }

        private String creator_email;
        internal string Creator_email { get => creator_email; }

        internal Dboard(int boardID, string name, String creator_email) : base(new BoardDalController())
        {
            this.boardID = boardID;
            this.name = name;
            this.creator_email = creator_email;
        }

    }
}
