using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;


namespace Frontend.Model
{
    class MBoard : NotifiableModelObject
    {
        private string name;
        public string Name
        {
            get => name;
            set { name = value; RaisePropertyChanged("Name"); }
        }
        private string creator_email;
        public string Creator_email
        {
            get => creator_email;
            set { creator_email = value; RaisePropertyChanged("Creator_email"); }
        }
        public ObservableCollection<MColumn> Columns { get; set; }

        public List<string> BoardMembers;

        internal MBoard(BackendController controller, string name, string Creator_email, List<SColumn> columns, List<String> members) : base(controller)
        {
            this.Name = name;
            this.Creator_email = Creator_email;
            this.BoardMembers = members;
            this.Columns = new ObservableCollection<MColumn>(columns.
                Select((c, i) => new MColumn(controller, c) ).ToList());
        }

        internal MBoard(BackendController controller, SBoard board): base(controller)
        {
            this.name = board.Name;
            this.Creator_email = board.Creator_email;
            this.BoardMembers = board.BoardMembers;
            this.Columns = new ObservableCollection<MColumn>(board.Columns.
                Select((c, i) => new MColumn(controller, c)).ToList());

        }

        public void RemoveColumn(MColumn C)
        {
            for (int i = Columns.Count-1; i > C.ColumnOrdinal; i--)
            {
                Columns[i].ColumnOrdinal -= 1;
            }
            Columns.Remove(C);

        }

        internal void MoveColumn(MColumn selectedColumn, int newOrdinal)
        {
            RemoveColumn(selectedColumn);
            selectedColumn.ColumnOrdinal = newOrdinal;
            addColumn(selectedColumn);
        }

        public void addColumn(MColumn C)
        {
            Columns.Add(C);
            for (int i = Columns.Count-1; i > C.ColumnOrdinal; i--)
            {
                Columns[i] = Columns[i - 1];
                Columns[i].ColumnOrdinal += 1;
            }
            Columns[C.ColumnOrdinal] = C;
        }


    }
}
