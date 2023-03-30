using Frontend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frontend
{
    class innerTests
    {
        public static void main (string[] args)
        {
            BackendController bc = new BackendController();
            bc.Login("abc@gmail.com", "Aa123456");
        }
    }
}
