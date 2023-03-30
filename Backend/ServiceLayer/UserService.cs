using System;
using System.Collections.Generic;
using System.Reflection;
using IntroSE.Kanban.Backend.business;
using log4net;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    /// <summary>
    /// The service layer controller of User objects.
    /// Contains methods for management of users.
    /// </summary>
    /// <remarks>
    /// This class can create a user, logged it in and out, add a task to tasks_in_progress list of the user.
    /// </remarks>
    internal class UserService
    {
        private readonly UserController userController;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal UserService()
        {
            userController = new UserController(); //Holds all the users.
        }

        /// <summary>
        /// loads all the presistant data from users
        /// </summary>
        internal void loadAllUsers()
        {
            userController.LoadAllUsers();
        }

        /// <summary>
        /// deletes all the pressistant data from boards
        /// </summary>
        internal void DeleteData()
        {
            userController.DeleteAllData();
        } 

        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="email">new user's email adress.</param>
        /// <param name="password">new user's password.</param>
        /// <returns>A response object. The response should contain a error message in case of an error.</returns>
        internal Response Register(string email, string password)
        {
            try
            {
                userController.Register(email, password);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Log in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        internal Response<SUser> Login(string email, string password)
        {
            try
            {
                SUser su = new SUser(userController.Login(email, password));
                return Response<SUser>.FromValue(su);
            }
            catch (Exception e)
            {
                return Response<SUser>.FromError(e.Message);
            }
        }

        /// <summary>        
        /// Log out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        internal Response Logout(string email)
        {
            try
            {
                userController.LogoutUser(email);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user</param>
        /// <param name="boardId">Email of the board creator</param>
        internal void JoinBoard(int boardId, string userEmail)
        {
            userController.AddBoardToUserList(boardId, userEmail);
        }

        /// <summary>        
        /// Returns list of board ID's the user is a member of.
        /// </summary>
        /// <param name="userEmail">Email of the current user</param>
        /// <returns>The list of boards</task></returns>
        internal List<int> GetBoardsOf(string userEmail)
        {
            return userController.GetBoardsOf(userEmail);
        }

        /// <summary>
        /// removes a board from the user list of boards
        /// </summary>
        /// <param name="member">Email of the current user</param>
        /// <param name="boardId">Email of the board creator</param>
        internal void RemoveBoardFromUserList(string member, int boardId)
        {
            userController.RemoveBoardFromUserList(member, boardId);
        }

        /// <summary>
        /// Returns all the boards the user is a member of.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <returns>A list of all the boards the user is a member of</returns>
        internal List<int> getBoards(string userEmail)
        {
            return userController.GetBoardsOf(userEmail);
        }

        /// <summary>
        /// Validates the given user is logged in
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        internal void ValidateUserLoggedIn(string userEmail)
        {
            userController.ValidateUserLoggedIn(userEmail);
        }

        /// <summary>
        /// Validates the given user exists
        /// </summary>
        /// <param name="emailAssignee"></param>
        internal void ValidateUserExists(string emailAssignee)
        {
            userController.ValidateUserExists(emailAssignee);
        }

    }
}
