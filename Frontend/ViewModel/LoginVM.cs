using Frontend.Model;
using Frontend.View;
using IntroSE.Kanban.Backend.ServiceLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.ViewModel
{
    class LoginVM: NotifiableObject
    {
        public BackendController Controller { get; private set; }

        public string email;
        public string Email {
            get => email;
            set { email = value;    RaisePropertyChanged("Email"); }
        }

        public string password = "";
        public string Password { 
            get => password;
            set { password = value; RaisePropertyChanged("Password"); }
        }

        private string errorLable = "";
        public string ErrorLable { 
            get=> errorLable; 
            set { errorLable = value; RaisePropertyChanged("ErrorLable"); } 
        }

        private string errorLableColor = "Red";
        public string ErrorLableColor
        {
            get => errorLableColor;
            set { errorLableColor = value; RaisePropertyChanged("ErrorLableColor"); }
        }

        public LoginVM()
        {
            this.Controller = new BackendController();
        }

        public MUser Login()
        {
            try
            {
                return Controller.Login(Email, Password);
            }
            catch (Exception e)
            {
                ErrorLable = e.Message;
                ErrorLableColor = "Red";
                return null;
            }
        }

        public void Register()
        {
            try
            {
                Controller.Register(Email, Password);
                ErrorLable = "Registered successfully";
                ErrorLableColor = "Green";
            }
            catch (Exception e)
            {
                ErrorLable = e.Message;
                ErrorLableColor = "Red";
            }
        }
    }
}
