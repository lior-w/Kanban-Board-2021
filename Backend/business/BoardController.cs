using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IntroSE.Kanban.Backend.business
{
    internal class BoardController
    {
        private Dictionary<string, Dictionary<string, int>> IDBoardsList;
        private Dictionary<int, Board> BoardsList;
        private int BoardID;

        private int ColumnID;

        private int TaskID;

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        DataAccessLayer.BoardDalController dalController;

        private readonly string NAME_OF_COLUMN_0 = "backlog";
        private readonly string NAME_OF_COLUMN_1 = "in progress";
        private readonly string NAME_OF_COLUMN_2 = "done";

        internal BoardController()
        {
            IDBoardsList = new Dictionary<string, Dictionary<string, int>>();
            BoardsList = new Dictionary<int, Board>();
            dalController = new DataAccessLayer.BoardDalController();
            BoardID = 0;
            TaskID = 0;
            ColumnID = 0;
        }

        /// <summary>
        /// Loads all the data that is related to Board Controller - boards, tasks, board members.
        /// </summary>
        internal void LoadAllBoards()
        {
            loadBoards();
            loadBoardMembers();
            loadColumns();
            loadTasks();
        }

        /// <summary>
        /// Loads all the data from boards.
        /// </summary>
        private void loadBoards()
        {
            List<DataAccessLayer.Dboard> dboards = dalController.SelectAllBoards();
            int maxID = 0;
            foreach (var dboard in dboards)
            {
                Board b = new Board(dboard.Name, dboard.Creator_email, dboard.BoardID);
                if (IDBoardsList.ContainsKey(dboard.Creator_email))
                {
                    IDBoardsList[dboard.Creator_email].Add(dboard.Name, dboard.BoardID);
                    log.Debug("added board to dictionary");
                }
                else
                {
                    IDBoardsList.Add(dboard.Creator_email, new Dictionary<string, int>());
                    IDBoardsList[dboard.Creator_email].Add(dboard.Name, dboard.BoardID);
                }
                BoardsList[dboard.BoardID] = b;
                if (dboard.BoardID > maxID) maxID = dboard.BoardID;
            }
            BoardID = maxID+1;
        }

        /// <summary>
        /// Loads all the data from board_members.
        /// </summary>
        private void loadBoardMembers() { 
             Dictionary<int, List<string>> boardMembers = dalController.SelectAllBoardMembers();
             foreach (var boardMember in boardMembers)
             {
                 BoardsList[boardMember.Key].BoardMembers = boardMember.Value;
             }
        }

        /// <summary>
        /// Loads all the data from columns.
        /// </summary>
        private void loadColumns()
        {
            List<DataAccessLayer.objects.Dcolumn> dcolumns = dalController.selectAllColumns();
            int maxID = 0;
            foreach (var dcol in dcolumns)
            {
                Column c = new Column(dcol.ColumnID, dcol.BoardID, dcol.ColumnOrdinal, dcol.Name, dcol.Limit);
                BoardsList[dcol.BoardID].InsertColumn(c);
                if (dcol.ColumnID > maxID) maxID = dcol.ColumnID;
            }
            ColumnID = maxID+1;
        }

        /// <summary>
        /// Loads all the data from tasks.
        /// </summary>
        private void loadTasks()
        {
            List<DataAccessLayer.Dtask> dtasks = dalController.selectAllTasks();
            int maxID = 0;
            foreach (var dtask in dtasks)
            {
                Task t = new Task(dtask.TaskID, dtask.Title, dtask.Description, DateTime.Parse(dtask.DueDate), DateTime.Parse(dtask.CreationTime), dtask.EmailAssignee);
                BoardsList[dtask.BoardID].insertTask(t, BoardsList[dtask.BoardID].GetColumnByID(dtask.ColumnID).ColumnOrdinal);
                if (dtask.TaskID > maxID) maxID = dtask.TaskID;
            }
            TaskID = maxID+1;
        }

        /// <summary>
        /// deletes all the data that is related to board controller - tasks, boards -, from the database
        /// </summary>
        internal void DeleteAllData()
        {
            dalController.deleteAllTables();
        }

        /// <summary>
        /// Limit the number of tasks in a specific column
        /// </summary>
        /// <param name="email">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        ///<exception cref="Exception"> if the user isnt logged in </exception>
        internal void LimitColumnOfBoard(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int limit)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            Board b = getBoard(creatorEmail, boardName);
            b.LimitColumn(columnOrdinal, limit);
            dalController.UpdateByTwoKeys(dalController.columns, dalController.boardID, b.BoardID, dalController.colOrdinal, columnOrdinal, dalController.columnLimit, limit.ToString());
        }

        /// <summary>
        /// Get the limit of a specific column
        /// </summary>
        /// <param name="creatorEmail">The email address of the user, must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The limit of the column.</returns>
        internal int GetColumnLimitOfBoard(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            return getBoard(creatorEmail, boardName).GetColumnLimit(columnOrdinal);
        }

        /// <summary>
        /// Get the name of a specific column given it's id
        /// </summary>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>The name of the column.</returns>
        internal string getColumnNameOfBoard(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            return getBoard(creatorEmail, boardName).getColumnName(columnOrdinal);
        }

        /// <summary>
        /// finds a board given its name and its creators email.
        /// </summary>
        /// <param name="email">Email of user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        ///<exception cref="Exception"> if name or email are null </exception>
        ///<exception cref="Exception"> if the board is nonexsisting </exception>
        /// <returns>the board that has been found.</returns>
        internal Board getBoard(String email, string boardName)
        {
            if (boardName == null)
            {
                log.Warn("attemp to create a board without a name");
                throw new Exception("board name is null");
            }
            if (email == null)
            {
                log.Warn("attemp to create a board without a email");
                throw new Exception("email is null");
            }
            if (!IDBoardsList.ContainsKey(email) || !IDBoardsList[email].ContainsKey(boardName))
            {
                log.Error("attempt to reach a nonexisting board");
                throw new Exception("board does not exists");
            }
            else
            {
                return BoardsList[IDBoardsList[email][boardName]];
            }
        }

        /// <summary>
        /// creates and adds a board to the specific user.
        /// </summary>
        /// <param name="email">Email of the user. Must be logged in</param>
        /// <param name="name">The name of the new board</param>
        ///<exception cref="Exception"> if a board with those attributes already exists </exception>
        internal void AddBoard(string email, string name)
        {
            if (IDBoardsList.ContainsKey(email))
            {
                if (IDBoardsList[email].ContainsKey(name))
                {
                    log.Error("an ettempt to recreate a board");
                    throw new Exception("this board already exists");
                }
                else
                {
                    Board b = new Board(name, email, BoardID);
                    CreateDeafaultColumns(b);
                    saveBoard(b);
                    IDBoardsList[email].Add(name, BoardID);
                    BoardsList[BoardID]=b;
                    BoardID += 1;
                    log.Debug("added board to dictionary");
                }
            }
            else
            {
                Board b = new Board(name, email, BoardID);
                CreateDeafaultColumns(b);
                saveBoard(b);
                IDBoardsList.Add(email, new Dictionary<string, int>());
                IDBoardsList[email].Add(name, BoardID);
                BoardsList[BoardID]=b;
                BoardID += 1;
                log.Debug("added board to List");
            }
        }

        private void CreateDeafaultColumns(Board b)
        {
            b.AddNewColumn(0, ColumnID++, NAME_OF_COLUMN_0);
            b.AddNewColumn(1, ColumnID++, NAME_OF_COLUMN_1);
            b.AddNewColumn(2, ColumnID++, NAME_OF_COLUMN_2);
        }

        /// <summary>
        /// save the new board to the database
        /// </summary>
        private void saveBoard(Board b)
        {
            dalController.InsertBoard(new DataAccessLayer.Dboard(b.BoardID, b.Name, b.Creator_email));
            for (int i = 0; i <= 2; i++) {
                dalController.InsertNewColumn(new DataAccessLayer.objects.Dcolumn(b.GetColumnID(i), b.BoardID, i, b.getColumnName(i), -1));
            }
        }

        /// <summary>
        /// Removes a board to the specific user.
        /// </summary>
        /// <param name="userEmail">Email of the user. Must be logged in</param>
        /// <param name="creatorEmail">The name of the board</param>
        /// <param name="boardName">The name of the board</param>
        /// <exception cref="Exception"> if name or email are null; </exception>
        /// <exception cref="Exception"> if the user is not the creator of the board; </exception>
        /// <exception cref="Exception"> if a board with those attributes doesnt exists </exception>
        internal void RemoveBoard(string userEmail, string creatorEmail, string boardName)
        {
            if (boardName == null)
            {
                log.Warn("attemp to create a board without a name");
                throw new Exception("must pass a name");
            }
            if (creatorEmail == null)
            {
                log.Warn("attemp to create a board without an email");
                throw new Exception("must pass an email");
            }
            if (userEmail != creatorEmail)
            {
                log.Error("trying to remove board without being the creator of the board");
                throw new Exception("the user isnt the board creator");
            }
            if (!IDBoardsList.ContainsKey(creatorEmail) || !IDBoardsList[creatorEmail].ContainsKey(boardName))
            {
                log.Error("an ettempt to remove a nonexisting board");
                throw new Exception("cannot remove a nonexisting board");
            }
            List<Column> columnsToRemove = getBoard(creatorEmail, boardName).GetAllColumns();
            DeleteColumns(columnsToRemove);

            int BoardID = IDBoardsList[creatorEmail][boardName];
            BoardsList.Remove(BoardID);
            IDBoardsList[creatorEmail].Remove(boardName);
            if (IDBoardsList[creatorEmail].Count == 0)
                IDBoardsList.Remove(creatorEmail);
            log.Debug("The boared was removed");
            dalController.deleteBoard(BoardID);
        }
        
        /// <summary>
        /// Deletes the list of given columns from the DB.
        /// </summary>
        /// <param name="columnsToRemove">The ID's of the Tasks to delete</param>
        private void DeleteColumns(List<Column> columnsToRemove)
        {
            foreach (Column col in columnsToRemove)
            {
                List<int> tasksToDelete = col.GetTaskIDs();
                DeleteTasks(tasksToDelete);
                dalController.deleteColumn(col.ColumnId);
            }
        }

        /// <summary>
        /// Deletes the list of given task id from the DB.
        /// </summary>
        /// <param name="tasks">The ID's of the Tasks to delete</param>
        internal void DeleteTasks(List<int> tasks)
        {
            foreach (int id in tasks)
            {
                dalController.deleteTask(id);
            }
        }

        /// <summary>        
        /// adds the user to the board members of the the given board
        /// </summary>
        /// <param name="userEmail">Email of the current user</param>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the board</param>
        internal void AddBoardMember(string userEmail, string creatorEmail, string boardName)
        {
            getBoard(creatorEmail, boardName).AddBoardMember(userEmail);
        }

        /// <summary>        
        /// Returns the board members of this board
        /// </summary>
        /// <returns> Returns a list of string that contains the board members of this board </returns>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">The name of the board</param>
        internal List<string> GetBoardMembers(string creatorEmail, string boardName)
        {
            return getBoard(creatorEmail, boardName).GetBoardsMembers();
        }

        /// <summary>
        /// Validates that the task ID exists in the given column in the given board
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        ///<exception cref="Exception"> thown if the task was nor found </exception>
        private void ValidateTaskExistsInColumn(string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            if(!getBoard(creatorEmail, boardName).ValidateTaskExistsInColumn(columnOrdinal, taskId))
            {
                log.Error("Attempted to update a nonexisting task");
                throw new Exception("Task could not be found");
            }
        }

        /// <summary>
        /// Advance a task to the next column
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        internal void AdvanceTask(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId)
        {
            ValidateTaskExistsInColumn(creatorEmail, boardName, columnOrdinal, taskId);
            Board b = getBoard(creatorEmail, boardName);
            b.AdvanceTask(userEmail, columnOrdinal, taskId);
            dalController.Update(dalController.tasks, dalController.taskID, taskId, dalController.columnID, b.GetColumnID(columnOrdinal + 1));
        }

        /// <summary>
        /// Returns a List of task IDs
        /// </summary>
        /// <param name="creatorEmail">Email of the user. Must be logged in</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <returns>A list of int containning the id's of the tasks in the column</returns>
        internal List<Task> GetListOfTaskFromColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            return getBoard(creatorEmail, boardName).GetListOfTaskFromColumn(columnOrdinal);
        }


        /// <summary>
        /// Returns the ID of the given board by its creator eamail and name
        /// </summary>
        /// <param name="creatorEmail">Email of the user</param>
        /// <param name="boardName">The name of the board</param>
        /// <returns>Returns the board ID of the given board by its creator eamail and name</returns>
        internal int GetBoardIdByNameAndEmail(string creatorEmail, string boardName)
        {
            return getBoard(creatorEmail, boardName).BoardID;
        }

        internal List<int> GetListOfBoardsID()
        {
            List<int> result = new List<int>();
            foreach(int key in BoardsList.Keys)
            {
                result.Add(key);
            }
            return result;
        }

        /// <summary>
        /// Returns the name of the given board by its Id
        /// </summary>
        /// <param name="boardId">Email of the user. Must be logged in</param>
        /// <returns>Returns the board ID of the given board by its creator eamail and name</returns>

        internal string GetBoardNameByID(int boardId)
        {
            return BoardsList[boardId].Name;
        }

        /// <summary>
        /// Validates the given user is a member of the given board
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">Name of the board</param>
        internal void ValidateUserIsBoardMember(string userEmail, string creatorEmail, string boardName)
        {
            getBoard(creatorEmail, boardName).ValidateUserIsBoardMember(userEmail);
        }

        /// <summary>
        /// Validates the given user is NOT member of the given board
        /// </summary>
        /// <param name="userEmail">Email of the user to validate</param>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">Name of the board</param>
        internal void ValidateUserIsNOTBoardMember(string userEmail, string creatorEmail, string boardName)
        {
            getBoard(creatorEmail, boardName).ValidateUserIsNOTBoardMember(userEmail);
        }

        /// <summary>
        /// Validates the "Backlog" column of the given board is not full
        /// </summary>
        /// <param name="creatorEmail">Email of the creator of the board</param>
        /// <param name="boardName">Name of the board</param>
        ///<exception cref="Exception"> thrown if the "Backlog" column of the given board is full </exception>
        internal void ValidateBacklogNotFull(string creatorEmail, string boardName)
        {
            if (getBoard(creatorEmail, boardName).isColumnFull(0))
            {
                log.Debug("Attempted adding task to a full column");
                throw new Exception("Backlog is full");
            }
        }

        //==================================TASK=======================================

        /// <summary>
        /// Add a new task.
        /// </summary>
		/// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <returns>A Task object</returns>
        internal Task CreateTask(string userEmail, string creatorEmail, string boardName, string title, string description, DateTime dueDate)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            ValidateBacklogNotFull(creatorEmail, boardName);
            Board b = getBoard(creatorEmail, boardName);
            Task newTask = b.CreateTask(TaskID, title, description, dueDate, userEmail);
            int columnId = b.GetColumnID(0);
            dalController.InsertNewTask(convertToDtask(newTask, columnId, b.BoardID));

            log.Debug("Task created succsesfully");
            TaskID += 1;
            return newTask;
        }


        /// <summary>
        /// Returns all the in-progress tasks of the logged-in user is assigned to.
        /// </summary>
        /// <param name="userEmail">Email of the logged in user</param>
        /// <param name="boards"> List of int that contains the board IDs </param>
        /// <returns>A list containing all the tasks</returns>
        internal List<Task> InProgressTasks(List<int> boards, string userEmail)
        {
            List<Task> result = new List<Task>();
            foreach (int ID in boards)
            {
                List<Task> tasks = BoardsList[ID].getInProgressTasks(userEmail);
                result.AddRange(tasks);
            }
            return result;
        }

        /// <summary>
        /// creates an object of DataAccessLayer.Dtask
        /// </summary>
        /// <param name="task">the tasks with all its parameters to convert into DataAccessLayer.Dtask attributes</param>
        /// <param name="boardID">The ID of the boards whice holdes the task</param>
        /// <param name="coulmnOrdinal">the column ordinal which holdes the task</param>
        private DataAccessLayer.Dtask convertToDtask(Task task, int columnId, int boardId)
        {
            return new DataAccessLayer.Dtask(task.Id, task.Title, task.Description, task.CreationTime.ToString(), task.DueDate.ToString(), task.EmailAssignee, columnId, boardId);
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDueDate">The new due date of the task</param>
        internal void UpdateTaskDueDate(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, DateTime newDueDate)
        {
            ValidateTaskExistsInColumn(creatorEmail, boardName, columnOrdinal, taskId);
            getBoard(creatorEmail, boardName).UpdateTaskDueDate(userEmail, columnOrdinal, taskId, newDueDate);
            
            dalController.Update(dalController.tasks, dalController.taskID, taskId, dalController.dueDate, newDueDate.ToString());
        }

        /// <summary>
        /// Update the Title of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">The new Title of the task</param>
        internal void UpdateTaskTitle(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string newTitle)
        {
            ValidateTaskExistsInColumn(creatorEmail, boardName, columnOrdinal, taskId);
            getBoard(creatorEmail, boardName).UpdateTaskTitle(userEmail, columnOrdinal, taskId, newTitle);

            dalController.Update(dalController.tasks, dalController.taskID, taskId, dalController.title, newTitle);

        }

        /// <summary>
        /// Update the Description of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">The new Description of the task</param>
        internal void UpdateTaskDescription(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string newDescription)
        {
            ValidateTaskExistsInColumn(creatorEmail, boardName, columnOrdinal, taskId);
            getBoard(creatorEmail, boardName).UpdateTaskDescription(userEmail, columnOrdinal, taskId, newDescription);

            dalController.Update(dalController.tasks, dalController.taskID, taskId, dalController.description, newDescription);
        }

        /// <summary>
        /// Update the Assingee of a task
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column ID. The first column is identified by 0, the ID increases by 1 for each column</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="emailAssignee">The new Sssingee of the task</param>
        /// <exception cref="Exception"> thown if trying to assing a task to itself </exception>
        internal void UpdateTaskAssingee(string userEmail, string creatorEmail, string boardName, int columnOrdinal, int taskId, string emailAssignee)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            ValidateUserIsBoardMember(emailAssignee, creatorEmail, boardName);
            ValidateTaskExistsInColumn(creatorEmail, boardName, columnOrdinal, taskId);
            if (userEmail.Equals(emailAssignee))
            {
                log.Error("Attempted updating task assingee to the same assignee");
                throw new Exception("the user is already the task's assignee");
            }
            getBoard(creatorEmail, boardName).UpdateTaskAssingee(userEmail, columnOrdinal, taskId, emailAssignee);

            dalController.Update(dalController.tasks, dalController.taskID, taskId, dalController.assigneeEmail, emailAssignee);
        }

        //================COLUMN=======================================================================

        /// <summary>
        /// Renames a specific column
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <param name="newColumnName">The new column name</param>        
        internal void RenameColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string newColumnName)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            Board b = getBoard(creatorEmail, boardName); 
            b.RenameColumn(columnOrdinal, newColumnName);
            dalController.Update(dalController.columns, dalController.columnID ,b.GetColumnID(columnOrdinal), dalController.columnName, newColumnName);
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
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            Board b = getBoard(creatorEmail, boardName);
            Column c = b.AddNewColumn(columnOrdinal, ColumnID++, columnName);
            dalController.InsertNewColumn(convertToDcolumn(c, b.BoardID));
            for(int i = columnOrdinal; i < b.GetColumnCount(); i++)
            {
                dalController.Update(dalController.columns, dalController.columnID, b.GetColumnID(i), dalController.colOrdinal, i);
            }
        }

        /// <summary>
        /// creates an object of DataAccessLayer.Dcolumn
        /// </summary>
        /// <param name="col">the column with all its parameters to convert into DataAccessLayer.Dcolumn attributes</param>
        private DataAccessLayer.objects.Dcolumn convertToDcolumn(Column col, int boardId)
        {
            return new DataAccessLayer.objects.Dcolumn(col.ColumnId, boardId, col.ColumnOrdinal, col.Name, col.MaxTasks);
        }

        /// <summary>
        /// Removes a specific column
        /// </summary>
        /// <param name="creatorEmail">Email of the board creator</param>
        /// <param name="boardName">The name of the board</param>
        /// <param name="columnOrdinal">The column location. The first column location is identified by 0, the location increases by 1 for each column</param>
        /// <exception cref="Exception"> thown if number of column <=2 </exception>
        internal void RemoveColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal)
        {
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            Board b = getBoard(creatorEmail, boardName);
            if(b.GetColumnCount() <= 2)
            {
                log.Warn("minimun column limit reached, cant remove column");
                throw new Exception("minimun column limit reached, cant remove column");

            }

            int newColOrdinal;
            if (columnOrdinal != 0) newColOrdinal = columnOrdinal - 1;
            else newColOrdinal = 1;

            b.MoveTasks(columnOrdinal, newColOrdinal);
            int newColumnID = b.GetColumnID(newColOrdinal);

            //updating the DB of the moved tasks.
            foreach (Task t in b.GetListOfTaskFromColumn(columnOrdinal))
            {
                dalController.Update(dalController.tasks, dalController.taskID, t.Id, dalController.columnID, newColumnID);
            }

            int oldColumnID = b.GetColumnID(columnOrdinal);
            b.RemoveColumn(columnOrdinal);
            dalController.Delete(dalController.columns, dalController.columnID, oldColumnID);
            
            if(columnOrdinal == 0)
            {
                dalController.Update(dalController.columns, dalController.columnID, b.GetColumnID(0), dalController.colOrdinal, 0);
            }

            for(int i = newColOrdinal; i < b.GetColumnCount(); i++)
            {
                dalController.Update(dalController.columns, dalController.columnID, b.GetColumnID(i), dalController.colOrdinal, i);
            }
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
            ValidateUserIsBoardMember(userEmail, creatorEmail, boardName);
            Board b = getBoard(creatorEmail, boardName);
            b.MoveColumn(columnOrdinal, shiftSize);

            dalController.Update(dalController.columns, dalController.columnID, b.GetColumnID(columnOrdinal+shiftSize), dalController.colOrdinal, columnOrdinal+shiftSize);

            if (shiftSize > 0)
            {
                for (int i = columnOrdinal; i < columnOrdinal+shiftSize; i++)
                {
                    dalController.Update(dalController.columns, dalController.columnID, b.GetColumnID(i), dalController.colOrdinal, i);
                }
            }
            else
            {
                for (int i = columnOrdinal+shiftSize+1; i <= columnOrdinal; i++)
                {
                    dalController.Update(dalController.columns, dalController.columnID, b.GetColumnID(i), dalController.colOrdinal, i);
                }
            }
        }

        /// <summary>
        /// return a Board object by it's ID
        /// </summary>
        /// <param name="boardID">The ID of the board</param>
        internal Board GetBoardByID(int boardID)
        {
            return BoardsList[boardID];
        }

        /// <summary>
        /// return a Column object by it's ID and the ID of th board.
        /// </summary>
        /// <param name="boardID">The ID of the board</param>
        /// <param name="columnID" > The ID of the column</param>
        internal Column GetColumnByID(int boardID, int columnID)
        {
            return BoardsList[boardID].GetColumnByID(columnID);
        }


        /// <summary>
        /// return a Task object by it's ID, the ID of the column and the ID of th board.
        /// </summary>
        /// <param name="boardId">The ID of the board</param>
        /// <param name="columnId" > The ID of the column</param>
        /// <param name="taskId">The ID of the requested Task</param>
        /// <returns>A Task object with the given ID</returns>
        internal Task GetTaskByID(int boardId, int columnId, int taskId)
        {
            return BoardsList[boardId].GetColumnByID(columnId).GetTaskByID(taskId);
        }
    }
}