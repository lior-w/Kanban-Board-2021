using System.Collections.Generic;
using System;
using log4net;
using log4net.Config;
using IntroSE.Kanban.Backend.business;
using System.Reflection;
using System.IO;

using System.Runtime.CompilerServices;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;

[assembly: InternalsVisibleTo("Tests")]

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    public class Service
    {
        private readonly UserService userService;
        private readonly BoardService boardService;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public Service()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            log.Info("Starting log!");

            userService = new UserService();
            boardService = new BoardService();
            //LoadData();
        }
        ///<summary>This method loads the data from the persistance.
        ///         You should call this function when the program starts. </summary>
        public Response LoadData()
        {
            try
            {
                userService.loadAllUsers();
                boardService.loadAllBoards();
                return new Response();
            }
            catch(Exception e)
            {
                return new Response(e.Message);
            }
        }
        ///<summary>Removes all persistent data.</summary>
        public Response DeleteData()
        {
            try
            {
                userService.DeleteData();
                boardService.DeleteData();
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        public bool ValidateUserLoggedIn(string email)
        {
            try
            {
                userService.ValidateUserLoggedIn(email);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        /// <summary>
        /// Log in an existing user
        /// </summary>
        /// <param name="userEmail">The email address of the user to login</param>
        /// <param name="password">The password of the user to login</param>
        /// <returns>A response object with a value set to the user, instead the response should contain a error message in case of an error</returns>
        public Response<SUser> Login(string userEmail, string password)
        {
            return userService.Login(userEmail, password);
        }

        /// <summary>        
        /// Log out an logged-in user. 
        /// </summary>
        /// <param name="userEmail">The email of the user to log out</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response Logout(string userEmail)
        {
            return userService.Logout(userEmail);
        }
        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response LimitColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.LimitColumn(userEmail, creatorEmail, boardName, columnOrdinal, limit);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        public Response<int> GetColumnLimit(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                int limit = boardService.GetColumnLimit(userEmail, creatorEmail, boardName, columnOrdinal);
                return Response<int>.FromValue(limit);
            }
            catch (Exception e)
            {
                return Response<int>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Get the name of a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        public Response<string> GetColumnName(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                string name = boardService.GetColumnName(userEmail, creatorEmail, boardName, columnOrdinal);
                return Response<string>.FromValue(name);
            }
            catch (Exception e)
            {
                return Response<string>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Add a new task.
        /// </summary>
		/// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A response object with a value set to the Task, instead the response should contain a error message in case of an error</returns>
        public Response<STask> AddTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                STask result = boardService.CreateTask(userEmail, creatorEmail, boardName, title, description, dueDate);
                return Response<STask>.FromValue(result);
            }
            catch (Exception e)
            {
                return Response<STask>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="dueDate">The new due date of the column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.UpdateTaskDueDate(userEmail, creatorEmail, boardName, columnOrdinal, taskId, dueDate);
                return new Response();
            }
            catch (Exception e)
            {
                return Response<business.Task>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Update task title
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="title">New title for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.UpdateTaskTitle(userEmail, creatorEmail, boardName, columnOrdinal, taskId, title);
                return new Response();
            }
            catch (Exception e)
            {
                return Response<business.Task>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Update the description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="description">New description for the task</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string description)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.UpdateTaskDescription(userEmail, creatorEmail, boardName, columnOrdinal, taskId, description);
                return new Response();
            }
            catch (Exception e)
            {
                return Response<business.Task>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.AdvanceTask(userEmail, creatorEmail, boardName, columnOrdinal, taskId);
                return new Response();
            }
            catch (Exception e)
            {
                return Response<business.Task>.FromError(e.Message);
            }
        }
        /// <summary>
        /// Returns a column given it's column ordinal
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A response object with a value set to the Column, The response should contain a error message in case of an error</returns>
        public Response<IList<STask>> GetColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);

                List<STask> result = boardService.GetColumn(userEmail, creatorEmail, boardName, columnOrdinal);
                return Response<IList<STask>>.FromValue(result);
            }
            catch (Exception e)
            {
                return Response<IList<STask>>.FromError(e.Message);
            }
            
        }

        /// <summary>
        /// Creates a new board for the logged-in user.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddBoard(string userEmail, string boardName)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.AddBoard(userEmail, boardName);
                int boardId = boardService.GetBoardIdByEmailAndName(userEmail, boardName);
                userService.JoinBoard(boardId, userEmail);
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
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the new board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response JoinBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.ValidateUserIsNOTBoardMember(userEmail, creatorEmail, boardName);
                int boardId = boardService.GetBoardIdByEmailAndName(creatorEmail, boardName);
                userService.JoinBoard(boardId, userEmail);
                boardService.AddBoardMember(userEmail, creatorEmail, boardName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Removes a board.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                int boardId = boardService.GetBoardIdByEmailAndName(creatorEmail, boardName);
                List<String> membersToDelete = boardService.GetBoardMembers(creatorEmail, boardName);
                foreach(string member in membersToDelete)
                {
                    userService.RemoveBoardFromUserList(member, boardId);
                }

                boardService.RemoveBoard(userEmail, creatorEmail, boardName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }
        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <returns>A response object with a value set to the list of tasks, The response should contain a error message in case of an error</returns>
        public Response<IList<STask>> InProgressTasks(string userEmail)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                List<int> boards = userService.getBoards(userEmail);

                IList<STask> result = boardService.InProgressTasks(boards, userEmail);
                return Response<IList<STask>>.FromValue(result);
            }
            catch (Exception e)
            {
                return Response<IList<STask>>.FromError(e.Message);
            }
        }


        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userEmail">The email address of the user to register</param>
        /// <param name="password">The password of the user to register</param>
        /// <returns>A response object. The response should contain a error message in case of an error<returns>
        public Response Register(string userEmail, string password)
        {
            return userService.Register(userEmail, password);
        }


        /// <summary>
        /// Assigns a task to a user
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>        
        /// <param name="emailAssignee">Email of the user to assign to task to</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AssignTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                userService.ValidateUserExists(emailAssignee);
                boardService.UpdateTaskAssingee(userEmail ,creatorEmail, boardName, columnOrdinal, taskId, emailAssignee);
                
                return new Response();
            }
            catch (Exception e)
            {
                return Response<business.Task>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Returns the list of board of a user. The user must be logged-in. The function returns all the board names the user created or joined.
        /// </summary>
        /// <param name="userEmail">The email of the user. Must be logged-in.</param>
        /// <returns>A response object with a value set to the board, instead the response should contain a error message in case of an error</returns>
        public Response<IList<String>> GetBoardNames(string userEmail)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                List<int> boardIdList = userService.GetBoardsOf(userEmail);
                IList<String> result = new List<String>();
                foreach (int id in boardIdList)
                {
                    result.Add(boardService.GetBoardNameByID(id));
                }
                return Response<IList<String>>.FromValue(result);
            }
            catch (Exception e)
            {
                return Response<IList<String>>.FromError(e.Message);
            }
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>        
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response AddColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.AddColumn(userEmail, creatorEmail, boardName, columnOrdinal, columnName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message); ;
            }
        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RemoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.RemoveColumn(userEmail, creatorEmail, boardName, columnOrdinal);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param>        
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response RenameColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.RenameColumn(userEmail, creatorEmail, boardName, columnOrdinal, newColumnName);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        public Response MoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int shiftSize)
        {
            try
            {
                userService.ValidateUserLoggedIn(userEmail);
                boardService.MoveColumn(userEmail, creatorEmail, boardName, columnOrdinal, shiftSize);
                return new Response();
            }
            catch (Exception e)
            {
                return new Response(e.Message);
            }
        }

        //=========================FOR FRONTEND=====================================

        /// <summary>
        /// returns a board given its name and its creators email.
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        public SBoard GetBoard(string creatorEmail, string boardName)
        {
            return boardService.GetBoard(creatorEmail, boardName);
        }

        /// <summary>
        /// return a Board object by it's ID
        /// </summary>
        /// <param name="boardID">The ID of the board</param>
        public SBoard GetBoardByID(int boardID)
        {
            return boardService.GetBoardByID(boardID);
        }

        /// <summary>
        /// returns a column given its board name, its creators email and its ordinal.
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ordinal of the column</param>
        public SColumn GetColumn(string creatorEmail, string boardName, int columnOrdinal)
        {
            return boardService.GetBoard(creatorEmail, boardName).Columns[columnOrdinal];
        }

        /// <summary>
        /// return a Column object by it's ID and the ID of th board.
        /// </summary>
        /// <param name="boardID">The ID of the board</param>
        /// <param name="columnID" > The ID of the column</param>
        public SColumn GetColumnByID(int boardID, int columnID)
        {
            return boardService.GetColumnByID(boardID, columnID);
        }

        /// <summary>
        /// return a Task object by it's ID, the ID of the column and the ID of th board.
        /// </summary>
        /// <param name="boardId">The ID of the board</param>
        /// <param name="columnId" > The ID of the column</param>
        /// <param name="taskId">The ID of the requested Task</param>
        /// <returns>A STask object with the given ID</returns>
        public STask GetTaskByID(int boardId, int columnId, int taskId)
        {
            return boardService.GetTaskByID(boardId, columnId, taskId);
        }

        /// <summary>
        /// return a list of board ID's that the given user is a member of
        /// </summary>
        /// <param name="userEmail">the user who is a member of the boards/param>
        public List<int> GetBoardsOfUser(string userEmail)
        {
            return userService.getBoards(userEmail);
        }

        /// <summary>
        /// return a list of board ID's that the given user is NOT a member of
        /// </summary>
        /// <param name="userEmail">the user who is NOT a member of the boards/param>
        public List<int> GetBoardsOfUserNotMmber(string userEmail)
        {
            List<int> allBoards = boardService.GetBoardsList();
            List<int> memberBoards = userService.getBoards(userEmail);
            foreach(int id in memberBoards)
            {
                allBoards.Remove(id);
            }
            return allBoards;
        }
    }
}
