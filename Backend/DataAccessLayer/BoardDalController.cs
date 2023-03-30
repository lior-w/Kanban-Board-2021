using IntroSE.Kanban.Backend.DataAccessLayer.objects;
using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class BoardDalController: DalController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        //"boards" table attributes
        internal string boards = "boards";
        internal string name = "name";
        internal string creatorEmail = "creator_email";
        internal string boardID = "boardID";

        //"tasks" table attributes
        internal string tasks = "tasks";
        internal string taskID = "taskID";
        internal string title = "title";
        internal string description = "description";
        internal string creationTime = "creation_time";
        internal string dueDate = "due_date";
        internal string assigneeEmail = "assignee_email";
        //boardID
        internal string columnID = "columnID";

        //"board_members" table attribute
        internal string boardsMembers = "board_members";
        internal string email = "email";
        //boardID

        //"columns" table attribute
        internal string columns = "columns";
        //coulmnID
        //boardID
        internal string colOrdinal = "columnOrdinal";
        internal string columnName = "name";
        internal string columnLimit = "columnLimit";


        /// <summary>
        /// loads all the data from the table "boards" (boardID, name, creators email, and the columns limits).
        /// </summary>
        /// <returns> a list of Dboard (board objects in the data layer)</returns>
        internal List<Dboard> SelectAllBoards()
        {
            List<Dboard> result = Select(boards, boardID).Cast<Dboard>().ToList();
            return result;
        }

        /// <summary>
        /// loads all the data from the table "tasks" (taskID, title, description, creationTime, dueDate, email assignee, 
        /// boardID of the board which the task belong to, column ordinal where the tasks is.).
        /// </summary>
        /// <returns> a list of Dtask (task objects in the data layer)</returns>
        internal List<Dtask> selectAllTasks()
        {
            List<Dtask> result = Select(tasks, taskID).Cast<Dtask>().ToList();
            return result;
        }

        /// <summary>
        /// loads all the data from the table "columns" (boardID, columnOrdinal, name, limit)
        /// </summary>
        /// <returns> a list of Dcolumns (task objects in the data layer)</returns>
        internal List<Dcolumn> selectAllColumns()
        {
            List<Dcolumn> result = Select(columns, colOrdinal).Cast<Dcolumn>().ToList();
            return result;
        }

        /// <summary>
        /// loads all the data from the table "board_members" (email, boardsID).
        /// </summary>
        /// <returns> a dictionary where the key is boardID and value is a list of all the email whome are the boards members</returns>
        internal Dictionary<int, List<string>> SelectAllBoardMembers()
        {
            Dictionary<int, List<string>> result = new Dictionary<int, List<string>>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {boardsMembers};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        if (!result.ContainsKey(dataReader.GetInt32(1))) result.Add(dataReader.GetInt32(1), new List<string>());
                        result[dataReader.GetInt32(1)].Add(dataReader.GetString(0));
                    }
                }
                finally
                {
                    if (dataReader != null)
                    {
                        dataReader.Close();
                    }

                    command.Dispose();
                    connection.Close();
                }
            }
            return result;
        }


        /// <summary>inserts an entry to the table "boards"</summary>
        /// <param name="board">the board that is supposed to be inserted</param>
        /// <exception>if didnt insert a single entry</exception>
        internal void InsertBoard(Dboard board)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {boards} ({name} ,{creatorEmail}, {boardID}) " +
                        $"VALUES (@nameVal,@creator_emailVal,@boardIDVal);";

                    SQLiteParameter nameParam = new SQLiteParameter(@"nameVal", board.Name);
                    SQLiteParameter creator_emailParam = new SQLiteParameter(@"creator_emailVal", board.Creator_email);
                    SQLiteParameter boardIDParam = new SQLiteParameter(@"boardIDVal", board.BoardID);
                    
                    command.Parameters.Add(nameParam);
                    command.Parameters.Add(creator_emailParam);
                    command.Parameters.Add(boardIDParam);

                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("inserted an entry to the DB in table "+board);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                if (res < 0)
                {
                    log.Error("wasnt able to insert an entries at boards");
                    throw new Exception("wasnt able to insert an entries at boards");
                }
            }
        }

        /// <summary>inserts an entry to the table "columns"</summary>
        /// <param name="column">the columns that is supposed to be inserted</param>
        /// <exception>if didnt insert a single entry</exception>
        internal void InsertNewColumn(Dcolumn column)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {columns} ({columnID}, {boardID}, {colOrdinal}, {columnName}, {columnLimit}) " +
                        $"VALUES (@columnIDVal,@columnBoardIDVal,@columnOrdinalVal,@columnNameVal,@columnLimitVal);";

                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnIDVal", column.ColumnID);
                    SQLiteParameter columnBoardIDParam = new SQLiteParameter(@"columnBoardIDVal", column.BoardID);
                    SQLiteParameter columnOrdinalParam = new SQLiteParameter(@"columnOrdinalVal", column.ColumnOrdinal);
                    SQLiteParameter columnNameParam = new SQLiteParameter(@"columnNameVal", column.Name);
                    SQLiteParameter columnLimitParam = new SQLiteParameter(@"columnLimitVal", column.Limit);

                    command.Parameters.Add(columnIDParam);
                    command.Parameters.Add(columnBoardIDParam);
                    command.Parameters.Add(columnOrdinalParam);
                    command.Parameters.Add(columnNameParam);
                    command.Parameters.Add(columnLimitParam);

                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("inserted an entries at columns");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                if (res < 0)
                {
                    log.Error("wasnt able to insert an entries at columns");
                    throw new Exception("wasnt able to insert an entries at columns");
                }
            }
        }

        /// <summary>inserts an entry to the table "tasks"</summary>
        /// <param name="task">the task that is supposed to be inserted</param>
        /// <exception>if didnt insert a single entry</exception>
        internal void InsertNewTask(Dtask task)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {tasks} ({taskID}, {title}, {description}, {creationTime}, {dueDate}, {assigneeEmail}, {columnID}, {boardID}) " +
                       $"VALUES (@taskIDVal,@titleVal, @descriptionVAL, @creation_timeVal, @due_dateVal, @assignee_emailVal, @columnIDVal, @boardIDVal);";

                    SQLiteParameter taskIDParam = new SQLiteParameter(@"taskIDVal", task.TaskID);
                    SQLiteParameter titleParam = new SQLiteParameter(@"titleVal", task.Title);
                    SQLiteParameter descriptionParam = new SQLiteParameter(@"descriptionVAL", task.Description);
                    SQLiteParameter creation_timeParam = new SQLiteParameter(@"creation_timeVal", task.CreationTime);
                    SQLiteParameter due_dateParam = new SQLiteParameter(@"due_dateVal", task.DueDate);
                    SQLiteParameter assignee_emailParam = new SQLiteParameter(@"assignee_emailVal", task.EmailAssignee);
                    SQLiteParameter columnIDParam = new SQLiteParameter(@"columnIDVal", task.ColumnID);
                    SQLiteParameter boardIDParam = new SQLiteParameter(@"boardIDVal", task.BoardID);

                    command.Parameters.Add(taskIDParam);
                    command.Parameters.Add(titleParam);
                    command.Parameters.Add(descriptionParam);
                    command.Parameters.Add(creation_timeParam);
                    command.Parameters.Add(due_dateParam);
                    command.Parameters.Add(assignee_emailParam);
                    command.Parameters.Add(columnIDParam);
                    command.Parameters.Add(boardIDParam);

                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("inserted an entries at tasks");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                if (res < 0)
                {
                    log.Error("wasnt able to insert an entries at tasks");
                    throw new Exception("wasnt able to insert an entries at tasks");
                }
            }
        }


        /// <summary>converts a SQLiteDataReader object to Dboard, Dcolumn or Dtask</summary>
        /// <param name="reader">the object that is supposed to be converted</param>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            if (reader.FieldCount == 3)
                return new Dboard(reader.GetInt32(2), reader.GetString(0), reader.GetString(1));
            else if (reader.FieldCount == 5)
                return new Dcolumn(reader.GetInt32(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetString(3), Int32.Parse(reader.GetString(4)));
            else return new Dtask(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetInt32(6), reader.GetInt32(7));
        }

        /// <summary>deletes all the tables that are under BoardDalController ("tasks", "columns", "boards")</summary>
        internal void deleteAllTables()
        {
            DeleteAllTable(tasks);
            DeleteAllTable(boards);
            DeleteAllTable(columns);
        }

        /// <summary>deletes a board from all the tables</summary>
        /// <param name="BoardID">the board's id that is supposed to be deleted</param>
        internal void deleteBoard(int BoardID)
        {
            Delete(boards, boardID, BoardID);
            Delete(columns, boardID, BoardID);
            Delete(boardsMembers, boardID, BoardID);
        }

        /// <summary>deletes a column from all the tables</summary>
        /// <param name="ColumnID">the board's id that is supposed to be deleted</param>
        internal void deleteColumn(int ColumnID)
        {
            Delete(columns, columnID, ColumnID);
            Delete(tasks, columnID, ColumnID);
        }

        /// <summary>deletes a task from the table "tasks"</summary>
        /// <param name="taskId">the task's id that is supposed to be deleted</param>
        internal void deleteTask(int taskId)
        {
            Delete(tasks, taskID, taskId);
        }
      

    }
}
