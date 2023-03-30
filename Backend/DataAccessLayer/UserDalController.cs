using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    class UserDalController : DalController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //"users" table attributes
        internal string users = "users";
        internal string email = "email";
        internal string password = "password";

        //"board_members" table attribute
        internal string boardsMembers = "board_members";
        //email
        internal string boardID = "boardID";


        /// <summary>
        /// loads all the data from the table "users" (email, password).
        /// </summary>
        /// <returns> a list of Dusers (user objects in the data layer)</returns>
        internal List<Duser> SelectAllUsers()
        {
            List<Duser> result = Select(users, email).Cast<Duser>().ToList();
            return result;
        }

        /// <summary>
        /// loads all the data from the table "board_members" (email, boardsID).
        /// </summary>
        /// <returns> a dictionary where the key is en emaial and value is a list of all the boardIDs whome are the user is a member of</returns>
        public Dictionary<string, List<int>> SelectAllBoardMembers()
        {
            Dictionary<string, List<int>> result = new Dictionary<string, List<int>>();
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
                        if (!result.ContainsKey(dataReader.GetString(0))) result.Add(dataReader.GetString(0), new List<int>());
                        result[dataReader.GetString(0)].Add(dataReader.GetInt32(1));
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

        /// <summary>inserts an entry to the table "users"</summary>
        /// <param name="user">the user that is supposed to be inserted</param>
        /// <exception>if didnt insert a single entry</exception>
        internal void InsertUser(Duser user)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {users} ({email} ,{password}) " +
                        $"VALUES (@emailVal,@passVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", user.Email);
                    SQLiteParameter passParam = new SQLiteParameter(@"passVal", user.Password);

                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(passParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("inserted an entry at table users");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                if (res < 0)
                {
                    log.Error("wasnt able to insert an entries at users");
                    throw new Exception("wasnt able to insert an entries at users");
                }
            }
        }

        /// <summary>inserts an entry to the table "board_members"</summary>
        /// <param name="userEmail">the user's email that is supposed to be inserted</param>
        /// <param name="BoardID">the board id that is supposed to be inserted</param>
        /// <exception>if didnt insert a single entry</exception>
        internal void InsertBoardToMembers(string userEmail, int BoardID)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                int res = -1;
                SQLiteCommand command = new SQLiteCommand(null, connection);
                try
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO {boardsMembers} ({email} ,{boardID}) " +
                        $"VALUES (@emailVal,@boardIDVal);";

                    SQLiteParameter emailParam = new SQLiteParameter(@"emailVal", userEmail);
                    SQLiteParameter boardIDParam = new SQLiteParameter(@"boardIDVal", BoardID);

                    command.Parameters.Add(emailParam);
                    command.Parameters.Add(boardIDParam);
                    command.Prepare();

                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("inserted an entries at board_members");
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }
                if (res < 0)
                {
                    log.Error("wasnt able to insert an entries at board_members");
                    throw new Exception("wasnt able to insert an entries at board_members");
                }
            }
        }

        /// <summary>convers a SQLiteDataReader object to Duser</summary>
        /// <param name="reader">the object that is supposed to be converted</param>
        protected override DTO ConvertReaderToObject(SQLiteDataReader reader)
        {
            Duser result = new Duser(reader.GetString(0), reader.GetString(1));
            return result;
        }

        /// <summary>deletes all the tables that are under UserDalController ("users", "board_members")</summary>
        internal void deleteAllTables()
        {
            DeleteAllTable(users);
            DeleteAllTable(boardsMembers);
        }

    }
            
    
}
