using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Model
{
    class MUser : NotifiableModelObject
    {
        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged("Email");
            }
        }

        private ObservableCollection<MBoard> memberBoards;
        public ObservableCollection<MBoard> MemberBoards { get; set; }

        private ObservableCollection<MBoard> unMemberBoards;
        public ObservableCollection<MBoard> UnMemberBoards { get; set; }

        public MUser(BackendController controller, string email) : base(controller)
        {
            this.Email = email;
            MemberBoards = new ObservableCollection<MBoard>(controller.GetBoardsOfUser(email));
            MemberBoards.CollectionChanged += HandleChangeMember;
            UnMemberBoards = new ObservableCollection<MBoard>(controller.GetBoardsOfUserNotMmber(email));
            UnMemberBoards.CollectionChanged += HandleChangeUnMember;
        }

        public void Join(MBoard b)
        {
            MemberBoards.Add(b);
            UnMemberBoards.Remove(b);
        }

        public void DeleteBoard(MBoard b)
        {
            MemberBoards.Remove(b);
        }

        private void HandleChangeMember(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (MBoard b in e.OldItems)
                    Controller.RemoveBoard(Email, b.Creator_email, b.Name);

        }
        private void HandleChangeUnMember(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
                foreach (MBoard b in e.OldItems)
                    Controller.JoinBoard(Email, b.Creator_email, b.Name);

        }
    }
}
