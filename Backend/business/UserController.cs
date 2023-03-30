using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IntroSE.Kanban.Backend.business
{
    /// <summary>
    /// The bussiness layer controller of User objects.
    /// Contains all the User's instances, methods for management of users.
    /// </summary>
    /// <remarks>
    /// This class can create a User, logged it in, check if the user is logged in and return a specific instance of a User.
    /// </remarks>
    internal class UserController
    {
        private Dictionary<string, User> users;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private DataAccessLayer.UserDalController dalController;

        internal UserController()
        {
            users = new Dictionary<string, User>();
            dalController = new DataAccessLayer.UserDalController();
        }

        /// <summary>
        /// Loads all the data that is related to User Controller - users, board members.
        /// </summary>
        internal void LoadAllUsers()
        {
            loadUsers();
            loadBoardMembers();
        }


        /// <summary>
        /// Loads all the data from users.
        /// </summary>
        private void loadUsers() {
            List<DataAccessLayer.Duser> dusers = dalController.SelectAllUsers();
            foreach (var duser in dusers)

            {
                User u = new User(duser.Email, duser.Password);
                users[u.Email] = u;
            }

        }

        /// <summary>
        /// Loads all the data from board_members.
        /// </summary>
        private void loadBoardMembers()
        {
            Dictionary<string, List<int>> members = dalController.SelectAllBoardMembers();
            foreach (var userBoards in members)
            {
                if (!users.ContainsKey(userBoards.Key))
                {
                    log.Error("recieved a user from the data base that doesnt exists");
                    throw new Exception("recieved a user from the data base that doesnt exists");
                }
                users[userBoards.Key].Boards = userBoards.Value;
            }
        }

        /// <summary>
        /// deletes all the data related to userController in the data base- users board_members.
        /// </summary>
        internal void DeleteAllData()
        {
            dalController.deleteAllTables();
        }


        /// <summary>
        /// Create a new user.
        /// </summary>
        /// <param name="email">new user's email adress</param>
        /// <param name="pass">new user's password</param>
        /// <exception cref="Exception">Thrown when email is null</exception>
        /// <exception cref="Exception">Thrown when password is null</exception>
        /// <exception cref="Exception">Thrown when email is already exists in the users list</exception>
        internal void Register(string email, string pass)
        {
            if (email == null)
            {
                log.Debug("UserController: User with null email attempted register");
                throw new Exception("must pass an email");
            }
            if (pass == null)
            {
                log.Debug("UserController: User with null password attempted register");
                throw new Exception("must pass a password");
            }
            if (users.ContainsKey(email))
            {
                log.Debug("UserController: User with this email alreasy exists, cannot register");
                throw new Exception("user already exist");
            }
            User u = new User(email, pass);
            log.Info("UserController: User created succesfully");
            users[email] = u;
            SaveUser(email, pass);
        }

        private void SaveUser(string email, string pass)
        {
            dalController.InsertUser(new DataAccessLayer.Duser(email, pass));
        }

        /// <summary>
        /// Log in an existing user.
        /// </summary>
        /// <param name="email">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>The User instance of the user that commits Login</returns>
        /// <exception cref="Exception">Thrown when the user is already logged in</exception>
        /// <exception cref="Exception">Thrown when password doesn't match the users's password</exception>
        /// <exception cref="Exception">Thrown when email does'nt exist in the users list</exception>
        internal string Login(string email, string password)
        {
            if (!users.ContainsKey(email))
            {
                log.Error("User attempted Login, not existing user");
                throw new Exception("user do not exist");
            }
            ValidateUserNOTLoggedIn(email);
            User u = GetUser(email);
            bool match = u.IsMatchPassword(password);
            if (match)
            {
                log.Debug("User Logeed in succesfully");
                u.Logged = true;
                return email;
            }
            else
            {
                log.Warn("User attempted Login with not matching password");
                throw new Exception("password do not match");
            } 
        }

        /// <summary>        
        /// Log out a logged in user. 
        /// </summary>
        /// <param name="email">The email of the user to log out</param>
        internal void LogoutUser(string email)
        {
            ValidateUserLoggedIn(email);
            GetUser(email).Logout();
        }

        /// <summary>
        /// removes a board from the user list of boards
        /// </summary>
        /// <param name="member">Email of the current user</param>
        /// <param name="boardId">Email of the board creator</param>
        internal void RemoveBoardFromUserList(string member, int boardId)
        {
            GetUser(member).RemoveBoardFromUserList(boardId);
        }

        /// <summary>        
        /// Returns list of board ID's the user is a member of.
        /// </summary>
        /// <param name="userEmail">Email of the current user</param>
        /// <returns>The list of boards</task></returns>
        internal List<int> GetBoardsOf(string userEmail)
        {
            return GetUser(userEmail).GetBoards();
        }

        /// <summary>
        /// Adds a board created by another user to the logged-in user. 
        /// </summary>
        /// <param name="userEmail">Email of the current user.</param>
        /// <param name="boardId">Email of the board creator</param>
        internal void AddBoardToUserList(int boardId, string userEmail)
        {
            GetUser(userEmail).AddBoardToUserList(boardId);
            dalController.InsertBoardToMembers(userEmail, boardId);
        }

        /// <summary>
        /// Returns a User instance by it's email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns>The User instance the matches the email</returns>
        /// <exception cref="Exception">Thrown when email is null</exception>
        /// <exception cref="Exception">Thrown when email doesn't exist in the users list</exception>
        internal User GetUser(string email)
        {
            if (email == null)
            {
                log.Error("try to GetUser with null email");
                throw new Exception("email can't be null");
            }
            if (users.ContainsKey(email))
            {
                return users[email];
            }
            else
            {
                log.Error("try to GetUser, user do not exist");
                throw new Exception("user do not exist");
            }
        }

        /// <summary>
        /// Validates the given user exists
        /// </summary>
        /// <param name="emailAssignee"></param>
        /// <exception cref="Exception">Thrown if the user does not exist</exception>
        internal void ValidateUserExists(string emailAssignee)
        {
            if (!users.ContainsKey(emailAssignee))
            {
                log.Warn("User doesnt exist");
                throw new Exception("the user does not exist");
            }
        }

        /// <summary>
        /// Validates the given user is logged in
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="Exception">Thrown if the user is not logged in</exception>
        internal void ValidateUserLoggedIn(string email)
        {
            if (!GetUser(email).Logged)
            {
                log.Warn("user isnt logged in");
                throw new Exception("user isnt logged in");
            }
        }

        /// <summary>
        /// Checks if a user is already logged in.
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="Exception">Thrown when the user is already logged in</exception>
        private void ValidateUserNOTLoggedIn(string email)
        {
            if (GetUser(email).Logged)
            {
                log.Warn("user already logged in");
                throw new Exception("user already logged in");
            }
        }
    }
}

