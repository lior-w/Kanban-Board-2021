using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    class MyBoardsVM : NotifiableObject
    {
        private Model.BackendController controller;
        public MUser User { get; private set; }
        
        private MBoard selectedBoard;
        public MBoard SelectedBoard
        {
            get => selectedBoard;
            set{ selectedBoard = value; EnableForward = value != null;  RaisePropertyChanged("SelectedBoard"); }
        }
        private bool enableForward = false;
        public bool EnableForward
        {
            get => enableForward;
            private set { enableForward = value; RaisePropertyChanged("EnableForward"); }
        }
        private string errorLable = "";
        public string ErrorLable
        {
            get => errorLable;
            set { errorLable = value; RaisePropertyChanged("ErrorLable"); }
        }

        public MyBoardsVM(MUser user)
        {
            this.controller = user.Controller;
            User = user;
        }

        internal void Logout()
        {
            controller.Logout(User.Email);
        }

        internal void deleteBoard()
        {
            try
            {
                User.DeleteBoard(SelectedBoard);
            }
            catch(Exception e)
            {
                ErrorLable = e.Message;
            }

        }
    }
}
