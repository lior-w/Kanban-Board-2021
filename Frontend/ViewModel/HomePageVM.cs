using Frontend.Model;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    class HomePageVM : NotifiableObject
    {
        private Model.BackendController controller;
        public MUser User { get; private set; }

        private string newBoardName = "";
        public string NewBoardName{
            get => newBoardName;
            set { newBoardName = value;  RaisePropertyChanged("NewBoardName"); }
        }
        private string creationErrorLable = "";
        public string CreationErrorLable
        {
            get => creationErrorLable;
            set { creationErrorLable = value; RaisePropertyChanged("CreationErrorLable"); }
        }

        private string creationErrorLableColor = "Red";
        public string CreationErrorLableColor
        {
            get => creationErrorLableColor;
            set { creationErrorLableColor = value; RaisePropertyChanged("CreationErrorLableColor"); }
        }

        private MBoard selectedBoard;
        public MBoard SelectedBoard
        {
            get => selectedBoard;
            set { selectedBoard = value; RaisePropertyChanged("SelectedBoard"); }
        }

        private string joiningErrorLable = "";
        public string JoiningErrorLable
        {
            get => joiningErrorLable;
            set { joiningErrorLable = value; RaisePropertyChanged("JoiningErrorLable"); }
        }

        private string joiningErrorLableColor = "Red";
        public string JoiningErrorLableColor
        {
            get => joiningErrorLableColor;
            set { joiningErrorLableColor = value; RaisePropertyChanged("JoiningErrorLableColor"); }
        }

        internal HomePageVM(MUser user)
        {
            this.controller = user.Controller;
            User = user;
        }


        internal void CreateBoard()
        {
            try
            {
                controller.AddBoard(User.Email, NewBoardName);
                User.MemberBoards.Add(controller.GetBoard(User.Email, NewBoardName));
                CreationErrorLable = "Board was created successfuly!";
                CreationErrorLableColor = "Green";
            }
            catch (Exception e)
            {
                CreationErrorLable = e.Message;
                CreationErrorLableColor = "Red";
            }
        }

        internal void joinBoard()
        {
            try
            {
                User.Join(SelectedBoard);
                JoiningErrorLable = "Joined board successfuly!";
                JoiningErrorLableColor = "Green";
            }
            catch(Exception e)
            {
                JoiningErrorLable = e.Message;
                JoiningErrorLableColor = "Red";
            }

        }

        internal void Logout()
        {
            controller.Logout(User.Email);
        }
    }
}
