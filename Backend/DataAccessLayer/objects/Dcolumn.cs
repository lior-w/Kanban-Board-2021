using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer.objects
{
    class Dcolumn: DTO
    {
        private int boardID;
        public int BoardID { get => boardID; }

        private int columnID;
        public int ColumnID { get => columnID; }

        private int columnOrdinal;
        public int ColumnOrdinal { get => columnOrdinal; }

        private string name;
        public string Name { get => name; }

        private int limit;
        public int Limit { get => limit; }


        public Dcolumn(int columnID, int boardID, int columnOrdinal, string name, int limit) : base(new BoardDalController())
        {
            this.boardID = boardID;
            this.columnID = columnID;
            this.columnOrdinal = columnOrdinal;
            this.name = name;
            this.limit = limit;
        }
    }
}
