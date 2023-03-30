using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal abstract class DTO
    {
        protected DalController _controller;
        protected DTO(DalController controller)
        {
            _controller = controller;
        }

    }
}
