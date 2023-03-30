using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]
namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public struct SUser
    {
        public readonly string Email;

        internal SUser(string email)
        {
            this.Email = email;
        }
    }
}
