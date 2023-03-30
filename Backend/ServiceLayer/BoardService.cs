using IntroSE.Kanban.Backend.business;
using IntroSE.Kanban.Backend.ServiceLayer.Objects;
using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]

namespace IntroSE.Kanban.Backend.ServiceLayer
{
    internal class BoardService
    {
        private readonly BoardController boardController;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal BoardService()
        {
            boardController = new BoardController();
        }
        /// <summary>
        /// loads all the pressisted data from boards
        /// </summary>
        internal void loadAllBoards()
        {
            boardController.LoadAllBoards();
        }
        /// <summary>
        /// deletes all the pressisted data from boards
        /// </summary>
        internal void DeleteData()
        {
            boardController.DeleteAllData();
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="creatorEmail">Email of the creator of the boardn</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        ///<exception cref="Exception"> if the user isnt logged in </exception>
        internal void LimitColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            boardController.LimitColumnOfBoard(userEmail, creatorEmail, boardName, columnOrdinal, limit);
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        internal int GetColumnLimit(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            return boardController.GetColumnLimitOfBoard(userEmail, creatorEmail, boardName, columnOrdinal);
        }

        /// <summary>
        /// Get the name of a specific column given it's id
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        internal string GetColumnName(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            return boardController.getColumnNameOfBoard(userEmail, creatorEmail, boardName, columnOrdinal);
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary> 
        /// <param name="creatorEmail">Email of the board creator </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        internal void AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            boardController.AdvanceTask(userEmail, creatorEmail, boardName, columnOrdinal, taskId);
        }

        /// <summary>
        /// Returns a List of tasks from the column
        /// </summary>
        /// <param name="creatorEmail">Email of the creator of the board </param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A list of int containning the id's of the tasks in the column</returns>
        internal List<STask> GetColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            return toSTasks(boardController.GetListOfTaskFromColumn(userEmail, creatorEmail, boardName, columnOrdinal));
        }

        /// <summary>
        /// Adds a board to the specific user.
        /// </summary>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the new board</param>
        ///<exception cref="Exception"> if the user isnt logged in </exception>
        /// <returns>A response object. The response should contain a error message in case of an error</returns>
        internal void AddBoard(string creatorEmail, string boardName)
        {
            boardController.AddBoard(creatorEmail, boardName);
        }

        /// <summary>
        /// Removes a board to the specific user.
        /// </summary>
        /// <param name="userEmail">Email of the user.</param>
        /// <param name="creatorEmail">Email of the board creator.</param>
        /// <param name="boardName">The name of the board</param>
        internal void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            boardController.RemoveBoard(userEmail, creatorEmail, boardName);
        }

        /// <summary>
        /// Returns the ID of the given board by its creator eamail and name
        /// </summary>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>Returns the board ID of the given board by its creator eamail and name</returns>
        internal int GetBoardIdByEmailAndName(string creatorEmail, string boardName)
        {
            return boardController.GetBoardIdByNameAndEmail(creatorEmail, boardName);
        }

        /// <summary>
        /// Returns the name of the given board by its Id
        /// </summary>
        /// <param name="boardId">ID of the board</param>
        /// <returns>Returns the board ID of the given board by its creator eamail and name</returns>
        internal string GetBoardNameByID(int boardId)
        {
            return boardController.GetBoardNameByID(boardId);
        }

        /// <summary>        
        /// adds the user to the board members of the the given board
        /// </summary>
        /// <param name="userEmail">Email of the current user</param>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the board</param>
        internal void AddBoardMember(string userEmail, string creatorEmail, string boardName)
        {
            boardController.AddBoardMember(userEmail, creatorEmail, boardName);
        }

        /// <summary>        
        /// Returns the board members of this board
        /// </summary>
        /// <returns> Returns a list of string that contains the board members of this board </returns>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the board</param>
        internal List<string> GetBoardMembers(string creatorEmail, string boardName)
        {
            return boardController.GetBoardMembers(creatorEmail, boardName);
        }
        
        /// <summary>
        /// Validates the given user is a member of the given board
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">Name of the board</param>
        internal void ValidateUserIsBoardMember(string userEmail, string creatorEmail, string boardName)
        {
            boardController.ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
        }

        //===============================TASK===================================

        /// <summary>
        /// Add a new task.
        /// </summary>
		/// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <exception> Thrown when the user isn't logged in</exception>
        internal STask CreateTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            return new STask(boardController.CreateTask(userEmail, creatorEmail, boardName, title, description, dueDate));
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
        internal void UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime dueDate)
        {
            boardController.UpdateTaskDueDate(userEmail, creatorEmail, boardName, columnOrdinal, taskId, dueDate);
            log.Debug("Task dueDate updated successfully");
        }
        
        /// <summary>
        /// Update the Title of a task
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">The new Title of the task</param>
        internal void UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string title)
        {
            boardController.UpdateTaskTitle(userEmail, creatorEmail, boardName, columnOrdinal, taskId, title);
            log.Debug("Task title updated successfully");
        }

        /// <summary>
        /// Update the Description of a task
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">The new Description of the task</param>
        internal void UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string newDescription)
        {
            boardController.UpdateTaskDescription(userEmail, creatorEmail, boardName, columnOrdinal, taskId, newDescription);
            log.Debug("Task Description updated successfully");
        }

        /// <summary>
        /// Update the Assingee of a task
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="emailAssignee">The new Sssingee of the task</param>
        internal void UpdateTaskAssingee(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            boardController.UpdateTaskAssingee(userEmail, creatorEmail, boardName, columnOrdinal, taskId, emailAssignee);
            log.Debug("Task Assingee updated successfully");
        }

        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <param name="boards"> List of int that contains the board IDs </param>
        /// <returns>A list containing all the tasks</returns>
        internal IList<STask> InProgressTasks(List<int> boards, string userEmail)
        {
            List<Task> Btasks = boardController.InProgressTasks(boards, userEmail);
            IList<STask> result = new List<STask>();
            foreach (var Btask in Btasks)
                result.Add(new STask(Btask));
            return result;
        }

        /// <summary>
        /// Validates the given user is NOT member of the given board
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">Name of the board</param>
        internal void ValidateUserIsNOTBoardMember(string userEmail, string creatorEmail, string boardName)
        {
            boardController.ValidateUserIsNOTBoardMember(userEmail, creatorEmail, boardName);
        }

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param>        
        internal void RenameColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            boardController.RenameColumn(userEmail, creatorEmail, boardName, columnOrdinal, newColumnName);
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>        
        internal void AddColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        {
            boardController.AddColumn(userEmail, creatorEmail, boardName, columnOrdinal, columnName);
        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        internal void RemoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            boardController.RemoveColumn(userEmail, creatorEmail, boardName, columnOrdinal);
        }

        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
        internal void MoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int shiftSize)
        {
            boardController.MoveColumn(userEmail, creatorEmail, boardName, columnOrdinal, shiftSize);
        }

        /// <summary>
        /// returns a board given its name and its creators email.
        /// </summary>
        /// <param name="creatorEmail">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        internal SBoard GetBoard(string creatorEmail, string boardName)
        {
            Board b = boardController.getBoard(creatorEmail, boardName);
            return new SBoard(b.Name, b.Creator_email, b.BoardID, toSColumns(b.GetAllColumns()), b.BoardMembers);
        }

        /// <summary>
        /// return a Board object by it's ID
        /// </summary>
        /// <param name="boardID">The ID of the board</param>
        internal SBoard GetBoardByID(int boardID)
        {
            Board b = boardController.GetBoardByID(boardID);
            return new SBoard(b.Name, b.Creator_email, b.BoardID, toSColumns(b.GetAllColumns()), b.BoardMembers);
        }

        /// <summary>
        /// return a Column object by it's ID and the ID of th board.
        /// </summary>
        /// <param name="boardID">The ID of the board</param>
        /// <param name="columnID" > The ID of the column</param>
        internal SColumn GetColumnByID(int boardID, int columnID)
        {
            Column c = boardController.GetColumnByID(boardID, columnID);
            return new SColumn(c.ColumnId, c.BoardId, c.ColumnOrdinal, c.Name, c.MaxTasks, toSTasks(c.GetTasks()));
        }

        /// <summary>
        /// return a Task object by it's ID, the ID of the column and the ID of th board.
        /// </summary>
        /// <param name="boardId">The ID of the board</param>
        /// <param name="columnId" > The ID of the column</param>
        /// <param name="taskId">The ID of the requested Task</param>
        /// <returns>A STask object with the given ID</returns>
        internal STask GetTaskByID(int boardId, int columnId, int taskId)
        {
            return new STask(boardController.GetTaskByID(boardId, columnId, taskId));
        }

        /// <summary>
        /// return a list of all the board IDs.
        /// </summary>
        internal List<int> GetBoardsList()
        {
            return boardController.GetListOfBoardsID();
        }

        /// <summary>
        /// a helper function that creates a list of SColumns from a list of buisness Columns
        /// </summary>
        /// <param name="listOfSColumns">The list of the buisness Columns</param>
        private List<SColumn> toSColumns(List<Column> listOfSColumns)
        {
            List<SColumn> result = new List<SColumn>();
            foreach (Column c in listOfSColumns)
            {
                result.Add(new SColumn(c.ColumnId, c.BoardId, c.ColumnOrdinal, c.Name, c.MaxTasks, toSTasks(c.GetTasks())));
            }
            return result;
        }

        /// <summary>
        /// a helper function that creates a list of STasks from a list of buisness Tasks
        /// </summary>
        /// <param name="listOfBTasks">The list of the buisness Tasks</param>
        private List<STask> toSTasks(List<Task> listOfBTasks)
        {
            List<STask> result = new List<STask>();
            foreach (Task task in listOfBTasks)
            {
                result.Add(new STask(task));
            }
            return result;
        }
    }
}