using log4net;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Reflection;

namespace IntroSE.Kanban.Backend.DataAccessLayer
{
    internal abstract class DalController
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly string connectionString;

        public DalController()
        {
            string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "kanban.db"));
            this.connectionString = $"Data Source={path}; Version=3;";
        }

        /// <summary>updates a table where the new atribute is a string</summary>
        /// <param name="tableName">the table's name that is supposed to be updated</param>
        /// <param name="keyID">the table's primary key name</param>
        /// <param name="id">the value the table in which is supposed to be changed</param>
        /// <param name="attributeName">the table's column in wich it is supposed to be updated</param>
        /// <param name="attributeValue">the new value that is supposed to be inserted</param>
        /// <exception> if no entry in the table was changed</exception>
        internal void Update(string tableName, string keyID, int id, string attributeName, string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {tableName} set [{attributeName}]=@{attributeName} where {keyID}={id}"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("updated the DB at table "+tableName+" in entry "+attributeName+" where "+keyID+"="+id);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            if (res < 0)
            {
                log.Error("wasnt able to update the data base");
                throw new Exception("wasnt able to update the database");
            }
        }

        /// <summary>updates a table where the new atribute is an integer </summary>
        /// <param name="tableName">the table's name that is supposed to be updated</param>
        /// <param name="keyID">the table's primary key name</param>
        /// <param name="id">the value the table in which is supposed to be changed</param>
        /// <param name="attributeName">the table's column in wich it is supposed to be updated</param>
        /// <param name="attributeValue">the new value that is supposed to be inserted</param>
        /// <exception> if no entry in the table was changed</exception>
        internal void Update(string tableName, string keyID, int id, string attributeName, int attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {tableName} set [{attributeName}]=@{attributeName} where {keyID }={id}"
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("updated the DB at table " + tableName + " in entry " + attributeName + " where " + keyID + "=" + id);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

            }
            if (res < 0)
            {
                log.Error("wasnt able to update the data base at " + tableName + " in " + attributeName+ attributeValue);
                throw new Exception("wasnt able to update the data base at " + tableName + " in " + attributeName + attributeValue);
            }
        }

        /// <summary>updates a table where the new atribute is a string </summary>
        /// <param name="tableName">the table's name that is supposed to be updated</param>
        /// <param name="keyID1">the table's first primary key name</param>
        /// <param name="ID1">the value in the table in which its supposed to be updated</param>
        /// <param name="keyID2">the table's second primary key name</param>
        /// <param name="ID2">the value in the table in which its supposed to be updated</param>
        /// <param name="attributeName">the table's column in wich it is supposed to be updated</param>
        /// <param name="attributeValue">the new value that is supposed to be inserted</param>
        /// <exception> if no entry in the table was changed</exception>
        internal void UpdateByTwoKeys(string tableName, string keyID1, int id1, string keyID2, int id2, string attributeName, string attributeValue)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"update {tableName} set [{attributeName}]=@{attributeName} where {keyID1}={id1} and {keyID2}={id2} "
                };
                try
                {
                    command.Parameters.Add(new SQLiteParameter(attributeName, attributeValue));
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("updated the DB at table " + tableName + " in entry " + attributeName + " where " + keyID1 + "=" + id1 + "and" + keyID2 + "=" + id2);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();

                }

            }
            if (res < 0)
            {
                log.Error("wasnt able to update the data base at " + tableName + " in " + attributeName + attributeValue);
                throw new Exception("wasnt able to update the data base at " + tableName + " in " + attributeName + attributeValue);
            }
        }

        /// <summary>updates a table where the new atribute is an integer </summary>
        /// <param name="tableName">the table's name that is supposed to be updated</param>
        /// <param name="keyID1">the table's first primary key name</param>
        /// <param name="ID1">the value in the table in which its supposed to be updated</param>
        /// <param name="keyID2">the table's second primary key name</param>
        /// <param name="ID2">the value in the table in which its supposed to be updated</param>
        /// <param name="attributeName">the table's column in wich it is supposed to be updated</param>
        /// <param name="attributeValue">the new value that is supposed to be inserted</param>
        /// <exception> if no entry in the table was changed</exception>
        internal void UpdateByTwoKeys(string tableName, string keyID1, int id1, string keyID2, int id2, string attributeName, int attributeValue)
        {
            UpdateByTwoKeys(tableName, keyID1, id1, keyID2, id2, attributeName, attributeValue.ToString());
        }

        /// <summary>loads all the data from a table and conterts it into a DTO object</summary>
        /// <param name="tableName">the table's name that is supposed to be updated</param>
        /// <returns>a list of DTO objects</returns>
        internal List<DTO> Select(string tableName, string orderBy)
        {
            List<DTO> results = new List<DTO>();
            using (var connection = new SQLiteConnection(connectionString))
            {
                SQLiteCommand command = new SQLiteCommand(null, connection);
                command.CommandText = $"select * from {tableName} order by {orderBy};";
                SQLiteDataReader dataReader = null;
                try
                {
                    connection.Open();
                    dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        results.Add(ConvertReaderToObject(dataReader));
                    }
                }
                catch
                {
                    log.Error("loaded data from table: " + tableName);
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
            return results;
        }

        /// <summary>an abstract method, converts a SQLiteDataReader object into a DTO object</summary>
        /// <returns> a DTO object</returns>
        protected abstract DTO ConvertReaderToObject(SQLiteDataReader reader);

        /// <summary>deletes an entry or a number of entries from a table </summary>
        /// <param name="tableName">the table's name that the entries are supposed to be deleted from</param>
        /// <param name="keyID">the table's primary key name</param>
        /// <param name="ID">the value in the table in which its supposed to be deleted</param>
        /// <exception> if no entry in the table was deleted</exception>
        internal void Delete(string tableName, string keyID, int ID)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {tableName} where {keyID}={ID}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("delete the entries from " + tableName + "where " + keyID + "=" + ID);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            if (res < 0)
            {
                log.Error("wasnt able to delete the entries from " + tableName + "where " + keyID + "=" + ID);
                throw new Exception("wasnt able to delete the entries from " + tableName + "where " + keyID + "=" + ID);
            }
        }

        /// <summary>deletes an entry or a number of entries from a table based on two keys </summary>
        /// <param name="tableName">the table's name that the entries are supposed to be deleted from</param>
        /// <param name="keyID1">the table's first primary key name</param>
        /// <param name="ID1">the value in the table in which its supposed to be deleted</param>
        /// <param name="keyID2">the table's second primary key name</param>
        /// <param name="ID2">the value in the table in which its supposed to be deleted</param>
        /// <exception> if no entry in the table was deleted</exception>
        internal void DeleteByTwoKeys(string tableName, string keyID1, int ID1, string keyID2, int ID2)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {tableName} where {keyID1}={ID1} and {keyID2}={ID2}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("delete the entries from " + tableName + "where " + keyID1 + "=" + ID1 + "and" + keyID2 + "=" + ID2);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            if (res < 0)
            {
                log.Error("wasnt able to delete the entries from " + tableName + "where " + keyID1 + "=" + ID1 + "and" + keyID2 + "=" + ID2);
                throw new Exception("wasnt able to delete the entries from " + tableName + "where " + keyID1 + "=" + ID1 + "and" + keyID2 + "=" + ID2);
            }
        }

        /// <summary>deletes all the entries from a table </summary>
        /// <param name="tableName">the table's name that the entries are supposed to be deleted from</param>
        /// <exception> if no entry in the table was deleted</exception>
        internal void DeleteAllTable(string tableName)
        {
            int res = -1;
            using (var connection = new SQLiteConnection(connectionString))
            {
                var command = new SQLiteCommand
                {
                    Connection = connection,
                    CommandText = $"delete from {tableName}"
                };
                try
                {
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
                catch
                {
                    log.Error("deleted the table " + tableName);
                }
                finally
                {
                    command.Dispose();
                    connection.Close();
                }

            }
            if (res < 0)
            {
                log.Error("wasnt able to delete the table " + tableName);
                throw new Exception("wasnt able to delete the table " + tableName);
            }
        }
    }

}
