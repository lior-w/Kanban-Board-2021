using System.Collections.Generic;
using System;
using log4net;
using System.Text.RegularExpressions;
using System.Reflection;

namespace IntroSE.Kanban.Backend.business
{
    /// <summary>
    /// The User class.
    /// Represent an instance of a User object.
    /// Contain all the information of the user, and methods needed for the user's function.
    /// </summary>
    internal class User
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool logged;
        internal bool Logged
        {
            get { return logged; }
            set { logged = value; }
        }
        private readonly string email;
        public string Email { get => email;  }
        private string pass;
        private string Pass{ get => pass; set
            {
                if (value != null && IsValidPassword(value))
                    pass = value;
                else
                    throw new Exception("invalid password");
            }
        }
        private int MIN_PASS_LENGTH = 4;
        private int MAX_PASS_LENGTH= 20;
        private List<int> boards;
        public List<int> Boards { get => boards; set => boards = value; }
       

        internal User(string email, string password)
        
        {
            if (IsValidEmail(email))
            {
                this.email = email;
            }
            else
            {
                throw new Exception("invalid email");
            }
            Pass = password;
            boards = new List<int>();
        }

        /// <summary>        
        /// Log out a logged in user.
        /// </summary>
        internal void Logout()
        {
            if (logged)
            {
                log.Debug("Logout succesfully");
                this.logged = false;
            }
        }

        /// <summary>        
        /// Check if the password matches to the user's password.
        /// </summary>
        /// <param name="password">The password that compared to the user's password</param>
        /// <returns>True or false</returns>
        /// <exception cref="Exception">Thrown when password us null</exception>
        internal bool IsMatchPassword(string password)
        {
            if (password == null)
            {
                log.Warn("try to check isMatchPassword with null password");
                throw new Exception("must pass a password");
            }
            if (this.Pass != password)
            {
                log.Warn("Attempmt to log in with an incorrect password");
                return false;
            }
            log.Debug("Correct password");
            return true;
        }


        /// <summary>        
        /// Validate email adress.
        /// </summary>
        /// <param name="email">The email adress we check</param>
        /// <returns>True or false</returns>
        /// <exception cref="Exception">Thrown when email is null</exception>
        private bool IsValidEmail(string email)
        {
            if (email == null)
            {
                log.Warn("try to validate email, null email");
                throw new Exception("must pass an email");
            }
            string pattern = "^([0-9a-zA-Z]([-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";
            if (!Regex.IsMatch(email, pattern))
            {
                log.Warn("invalid email");
                return false;
            }
            log.Debug("validate email succesfully, valid email");
            return true;
        }

        /// <summary>        
        /// Validate password.
        /// </summary>
        /// <param name="pass">The password we check</param>
        /// <returns>True or false</returns>
        /// <exception cref="Exception">Thrown when pass is null</exception>
        internal bool IsValidPassword(string pass)
        {
            if (pass == null)
            {
                log.Warn("try to validate password, null password");
                throw new Exception("must pass a password");
            }
            if (pass.Length < MIN_PASS_LENGTH || pass.Length > MAX_PASS_LENGTH)
            {
                log.Warn("invalid password");
                return false;
            }
            bool up = false;
            bool low = false;
            bool num = false;
            for (int i = 0; i < pass.Length; i++)
            {
                char c = pass[i];
                if (char.IsLetterOrDigit(c))
                {
                    if (!num && char.IsDigit(c)) { num = true; }
                    if (!up && char.IsUpper(c)) { up = true; }
                    if (!low && char.IsLower(c)) { low = true; }
                }
            }
            if (!up || !low || !num)
            {
                log.Warn("invalid password");
                return false;
            }
            log.Debug("validate password succesfuly, valid password");
            return true;
        }

        /// <summary>        
        /// Returns list of board ID's the user is a member of.
        /// </summary>
        /// <returns>The list of boards</task></returns>
        internal List<int> GetBoards()
        {
            return this.Boards;
        }


        /// <summary>        
        /// Adds the id of the board to the list of boards.
        /// </summary>
        /// <param name="boardId">The Board object we want to add to the list</param>
        /// <exception cref="Exception">Thrown when the id already exists in the list</exception>
        internal void AddBoardToUserList(int boardId)
        {
            if (boards.Contains(boardId))
            {
                log.Warn("tried joining a borad while already being a board member");
                throw new Exception("user already board member");
            }
            boards.Add(boardId);
            log.Debug("board added to list succesfully");
        }

        /// <summary>        
        /// removes the id of the board from the list of boards.
        /// </summary>
        /// <param name="boardId">The Board object we want to remove to from list</param>
        internal void RemoveBoardFromUserList(int boardId)
        {
            boards.Remove(boardId);
            log.Debug("board removed from list succesfully");
        }
    }
}