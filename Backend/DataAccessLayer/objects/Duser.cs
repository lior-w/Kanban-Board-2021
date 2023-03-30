using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal class Duser : DTO
    {

        private string email;
        public string Email { get => email;  }
        private string password;
        public string Password { get => password; }


        public Duser(string email, string password) : base(new UserDalController())
        {
            this.email= email;
            this.password = password;
        }

    }
}
