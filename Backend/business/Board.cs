using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IntroSE.Kanban.Backend.business
{
    internal class Board
    {
        private string name;
        internal string Name { get => name; }
        private string creator_email;
        internal string Creator_email { get => creator_email; }

        private readonly Dictionary<int, Column> ColumnsList;

        //private List<Column> columns;
        //internal List<Column> Columns { get => columns; }

        private int boardID;
        internal int BoardID
        {
            get { return boardID; }
        }

        private List<string> boardMembers;
        internal List<string> BoardMembers {
            get => boardMembers;
            set => boardMembers = value;
        }

        private int MAX_COLUMN_ORDINAL = -1;
        private int MIN_COLUMN_ORDINAL = 0;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal Board(string name, string email, int ID)
        {
            if (name == null)
            {
                log.Warn("attemp to create a board without a name");
                throw new Exception("must pass a name");
            }
            if (email == null)
            {
                log.Warn("attemp to create a board without an email");
                throw new Exception("must pass an email");
            }
            this.name = name;
            this.creator_email = email;
            this.boardID = ID;
            ColumnsList = new Dictionary<int, Column>();
            boardMembers = new List<string>();
            boardMembers.Add(email);
            log.Info("new board created!");
        }

        /// <summary>
        ///  Returns the column's limit given it's ID
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>the column limit</returns>
        internal int GetColumnLimit(int columnOrdinal)
        {
            ValidateColumnOrdinal(columnOrdinal);
            int lim = ColumnsList[columnOrdinal].MaxTasks;
            if(lim == -1)
            {
                log.Error("Attempted getting column limit from a column that has not been limited");
                throw new Exception("not limited");
            }
            return lim;
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be advenced</param>
        ///<exception cref="Exception"> if attempted to advance a task from the "done" column </exception>
        internal void AdvanceTask(string userEmail, int columnOrdinal, int taskId)
        {
            ValidateColumnOrdinal(columnOrdinal);
            if (columnOrdinal == MAX_COLUMN_ORDINAL)
            {
                log.Warn("attempt to advance a done task");
                throw new Exception("cannot advance done tasks");
            }
            Task task = ColumnsList[columnOrdinal].DeleteTask(userEmail, taskId);
            ColumnsList[columnOrdinal + 1].AddTask(task);
            
            log.Debug("advanced a task");
        }

        /// <summary>
        /// Returns whether a column given it's ID is full or not
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>a boolean that aswers whether the column is full</returns>
        internal bool isColumnFull(int columnOrdinal)
        {
            ValidateColumnOrdinal(columnOrdinal);
            return ColumnsList[columnOrdinal].isColumnFull();
        }


        /// <summary>
        /// Validates the given user is a member of this board
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        /// <exception cref="Exception"> thrown if the user is not a member of this board</exception>
        internal void ValidateUserIsBoardMember(string userEmail)
        {
            if (!boardMembers.Contains(userEmail))
            {
                log.Error("The user isnt a board member");
                throw new Exception("The user isnt a board member");
            }
        }

        /// <summary>
        /// Validates the given user is NOT a member of this board
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        /// <exception cref="Exception"> thrown if the user IS a member of this board</exception>
        internal void ValidateUserIsNOTBoardMember(string userEmail)
        {
            if (boardMembers.Contains(userEmail))
            {
                log.Error("The user is already a board member");
                throw new Exception("The user is already a board member");
            }
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        public void LimitColumn(int columnOrdinal, int limit) {
            if (limit < 0)
            {
                log.Warn("Attempted limiting a Column with a negative number");
                throw new Exception("limit should be a positive number");
            }
            ColumnsList[columnOrdinal].LimitColumn(limit);
            log.Debug("Limited the number of tasks in a column successfully");
        }

        internal string getColumnName(int columnOrdinal)
        {
            ValidateColumnOrdinal(columnOrdinal);
            return ColumnsList[columnOrdinal].Name;
        }

        /// <summary>
        /// Returns a List of task IDs
        /// </summary>
        /// <param name="creatorEmail">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A list of int containning the id's of the tasks in the column</returns>
        internal List<Task> GetListOfTaskFromColumn(int columnOrdinal)
        {
            ValidateColumnOrdinal(columnOrdinal);
            return ColumnsList[columnOrdinal].GetTasks();
        }

        /// <summary>        
        /// adds the user to the board members of this board
        /// </summary>
        /// <param name="userEmail">Email of the current user</param>
        internal void AddBoardMember(string userEmail)
        {
            boardMembers.Add(userEmail);
        }

        /// <summary>        
        /// Returns the board members of this board
        /// </summary>
        /// <returns> Returns a list of string that contains the board members of this board </returns>
        internal List<string> GetBoardsMembers()
        {
            return this.boardMembers;
        }

        /// <summary>
        /// reinserts a task to its place after loarding
        /// </summary>
        /// <param name="TaskID"> the task's id</param>
        /// <param name="ColumnOrdinal">where in the board it should be reinserted</param>
        internal void insertTask(Task task, int ColumnOrdinal)
        {
            ColumnsList[ColumnOrdinal].AddTask(task);
        }

        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <returns>A list containing all the task ID's</returns>
        internal List<Task> getInProgressTasks(string userEmail)
        {
            List<Task> result = new List<Task>();
            for(int i=1; i<MAX_COLUMN_ORDINAL; i++)
            {
                foreach (Task t in ColumnsList[i].GetTasks())
                    if (t.checkAssingee(userEmail)) result.Add(t);
            }
            return result;
        }

        /// <summary>
        ///  Validates that the given column ordinal is within the valid range.
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        ///<exception cref="Exception"> throw if the column ordinal is invaild </exception>
        private void ValidateColumnOrdinal(int columnOrdinal)
        {
            if (columnOrdinal > MAX_COLUMN_ORDINAL | columnOrdinal < MIN_COLUMN_ORDINAL)
            {
                log.Error("attempt to get a nonexisting column");
                throw new Exception("no such column");
            }
        }

        /// <summary>
        /// Validates that the task ID exists in the given column in the given board
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task Id to be identified</param>
        /// <return> True if task exists, false otherwise</return>
        internal bool ValidateTaskExistsInColumn(int columnOrdinal, int taskId)
        {
            ValidateColumnOrdinal(columnOrdinal);
            return ColumnsList[columnOrdinal].checkTaskExists(taskId);
        }

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param> 
        internal void RenameColumn(int columnOrdinal, string newColumnName)
        {
            ValidateColumnOrdinal(columnOrdinal);
            ColumnsList[columnOrdinal].Name = newColumnName;
            log.Debug("column renamed");
        }

        /// <summary>
        /// Adds a new column
        /// </summary>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="columnName">The name for the new columns</param>
        internal Column AddNewColumn(int columnOrdinal, int coulmnId, string columnName)
        {
            Column c = new Column(coulmnId, BoardID, columnOrdinal, columnName, -1);
            AddColumn(columnOrdinal, c);
            return c;
        }

        internal List<Column> GetAllColumns()
        {
            List<Column> result = new List<Column>();
            foreach (Column col in ColumnsList.Values)
            {
                result.Add(col);
            }
            return result;
        }

        /// <summary>
        /// Adds a column to the board
        /// </summary>
        /// <param name="columnOrdinal">The location of the new column. Location for old columns with index>=columnOrdinal is increased by 1 (moved right). The first column is identified by 0, the location increases by 1 for each column.</param>
        /// <param name="column">The column to add</param>
        private void AddColumn(int columnOrdinal, Column column)
        {
            MAX_COLUMN_ORDINAL++;
            ValidateColumnOrdinal(columnOrdinal);
            for (int i = MAX_COLUMN_ORDINAL; i > columnOrdinal; i--)
            {
                ColumnsList[i] = ColumnsList[i - 1];
                ColumnsList[i].ColumnOrdinal += 1;
            }
            ColumnsList[columnOrdinal] = column;
            log.Debug("column added");
        }

        /// <summary>
        /// reinserts a column to its place after loading
        /// </summary>
        /// <param name="col">the ordinal of the column that should be reinserted</param>
        internal void InsertColumn(Column col)
        {
            MAX_COLUMN_ORDINAL++;
            ColumnsList[col.ColumnOrdinal] =  col;
            log.Debug("column reinserted");
        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        internal void RemoveColumn(int columnOrdinal)
        {
            for (int i = columnOrdinal+1; i <= MAX_COLUMN_ORDINAL; i++)
            {
                ColumnsList[i].ColumnOrdinal -= 1;
                ColumnsList[i-1] = ColumnsList[i];
            }
            ColumnsList.Remove(MAX_COLUMN_ORDINAL);
            MAX_COLUMN_ORDINAL--;
            log.Debug("column removed");
        }

        /// <summary>
        /// Moves a column shiftSize times to the right. If shiftSize is negative, the column moves to the left
        /// </summary>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="shiftSize">The number of times to move the column, relativly to its current location. Negative values are allowed</param>  
        internal void MoveColumn(int columnOrdinal, int shiftSize)
        {
            ValidateColumnOrdinal(columnOrdinal);
            if(ColumnsList[columnOrdinal].numOfTasks() != 0)
            {
                log.Warn("column is not empty and cant be moved");
                throw new Exception("attempted moving a column that is not empty");
            }
            if(columnOrdinal + shiftSize > MAX_COLUMN_ORDINAL | columnOrdinal + shiftSize < MIN_COLUMN_ORDINAL)
            {
                log.Warn("shift size out of bounds");
                throw new Exception("shift size out of bounds");
            }
            Column tmp = ColumnsList[columnOrdinal];
            RemoveColumn(columnOrdinal);
            AddColumn(columnOrdinal + shiftSize, tmp);
            log.Debug("column moved");
        }

        /// <summary>
        /// Moves all the tasks from one column ordinal to another
        /// </summary>
        /// <param name="from">the ordinal from which to remove the tasks</param>
        /// <param name="to">the ordinal to which add the tasks</param>  

        internal void MoveTasks(int from, int to)
        {
            List<Task> tasks = ColumnsList[from].GetTasks();
            Column col = ColumnsList[to];

            if(col.MaxTasks!=-1 &&(col.MaxTasks - col.numOfTasks() < tasks.Count))
            {
                log.Warn("cannot remove column, destination column is full");
                throw new Exception("cannot remove column, destination column is full");

            }
            foreach (Task t in tasks)
            {
                col.AddTask(t);
            }
            log.Debug("moved tasks from the column to be deleted");
        }

        /// <summary>
        /// returns the number of columns in the board.
        /// </summary>
        internal int GetColumnCount()
        {
            return ColumnsList.Count;
        }

        /// <summary>
        /// return the column ID of the column at the position of columnOrdinal
        /// </summary>
        /// <param name="columnOrdinal">the ordinal to find</param>
        internal int GetColumnID(int columnOrdinal)
        {
            return ColumnsList[columnOrdinal].ColumnId;
        }

        /// <summary>
        /// Creates a new Task Object and adding it to the list of Tasks.
        /// </summary>
        /// <param name="taskID">the ID of the task</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <returns>A Task object</returns>
        internal Task CreateTask(int taskID, string title, string description, DateTime dueDate, string userEmail)
        {
            return ColumnsList[0].CreateTask(taskID, title, description, dueDate, userEmail);
        }


        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDueDate">The new due date of the task</param>
        ///<exception cref="Exception"> thown if trying to update a task in the done column </exception>
        internal void UpdateTaskDueDate(string userEmail, int columnOrdinal, int taskId, DateTime newDueDate)
        {
            if (columnOrdinal == MAX_COLUMN_ORDINAL)
            {
                log.Warn("attempt to update the duedate of a done task");
                throw new Exception("cannot update done tasks");
            }
            ColumnsList[columnOrdinal].UpdateTaskDueDate(userEmail, taskId, newDueDate);
        }

        /// <summary>
        /// Update the Title of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">The new Title of the task</param>
        ///<exception cref="Exception"> thown if trying to update a task in the done column </exception>
        internal void UpdateTaskTitle(string userEmail, int columnOrdinal, int taskId, string newTitle)
        {
            if (columnOrdinal == MAX_COLUMN_ORDINAL)
            {
                log.Warn("attempt to update the title of a done task");
                throw new Exception("cannot update done tasks");
            }
            ColumnsList[columnOrdinal].UpdateTaskTitle(userEmail, taskId, newTitle);
        }

        /// <summary>
        /// Update the Description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">The new Description of the task</param>
        ///<exception cref="Exception"> thown if trying to update a task in the done column </exception>
        internal void UpdateTaskDescription(string userEmail, int columnOrdinal, int taskId, string newDescription)
        {
            if (columnOrdinal == MAX_COLUMN_ORDINAL)
            {
                log.Warn("attempt to update the description of a done task");
                throw new Exception("cannot update done tasks");
            }
            ColumnsList[columnOrdinal].UpdateTaskDescription(userEmail, taskId, newDescription);
        }

        /// <summary>
        /// Update the Assingee of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="emailAssignee">The new Sssingee of the task</param>
        /// <exception cref="Exception"> thown if trying to update a task in the done column </exception>
        internal void UpdateTaskAssingee(string userEmail, int columnOrdinal, int taskId, string emailAssignee)
        {
            if (columnOrdinal == MAX_COLUMN_ORDINAL)
            {
                log.Warn("Attempted updating task assingee of a done task");
                throw new Exception("cannot update done tasks");
            }
            ColumnsList[columnOrdinal].UpdateTaskAssingee(userEmail, taskId, emailAssignee);
        }

        /// <summary>
        /// returns a columns based on it's ID
        /// </summary>
        /// <param name="columnID">the column's ID</param>
        /// <exception cref="Exception"> thown if column doest exist </exception>
        internal Column GetColumnByID(int columnID)
        {
            foreach(Column col in ColumnsList.Values)
            {
                if (col.ColumnId == columnID) return col;
            }
            throw new Exception("column not found");
        }
    }
}