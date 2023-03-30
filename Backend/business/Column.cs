using log4net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace IntroSE.Kanban.Backend.business
{
    internal class Column
    {
        private int maxTasks;
        internal int MaxTasks
        {
            get { return maxTasks; }
            set { LimitColumn(value); }
        }
        private string name;
        internal string Name
        {
            get { return name; }
            set { name = value; }
        }
        private int columnId;
        internal int ColumnId
        {
            get { return columnId; }
        }

        private int columnOrdinal;
        internal int ColumnOrdinal
        {
            get { return columnOrdinal; }
            set { columnOrdinal = value; }
        }

        private Dictionary<int, Task> TasksList;

        private int boardId;
        internal int BoardId { get => boardId; }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        internal Column(int columnId, int boardId, int columnOrdinal , string name, int limit)
        {
            TasksList = new Dictionary<int, Task>();
            this.columnId = columnId;
            this.boardId = boardId;
            this.columnOrdinal = columnOrdinal;
            this.name = name;
            maxTasks = limit;
        }

        /// <summary>
        /// Limit the number of tasks in the column
        /// </summary>
        /// <param name="limit">The new limit value. A value of -1 indicates no limit.</param>
        ///<exception cref="Exception"> if limit<0, the limit should be a positive number </exception>
        ///<exception cref="Exception">if there is too many tasks in the column already </exception>
        internal void LimitColumn(int limit)
        {
            if (TasksList.Count > limit)
            {
                log.Error("Attempted limiting a Column that has more tasks than the limit");
                throw new Exception("you already have to many tasks in this column");
            }
            log.Debug("Limited the number of tasks in a column successfully");
            this.maxTasks = limit;
        }

        /// <summary>
        /// Add a new task to the column.
        /// </summary>
        /// <param name="taskId">the task that is supposed to be added.</param>
        ///<exception cref="Exception"> if the column is full </exception>
        internal void AddTask(Task task)
        {
            if (maxTasks!=-1 & TasksList.Count >= maxTasks)
            {
                log.Warn("Attempted adding a Task to a full Column");
                throw new Exception("you exceeded the limited number of tasks");
            }
            TasksList[task.Id] = task;
            log.Debug("Task added to a Column successfully");
        }

        /// <summary>
        /// removes a task from the column and returns it.
        /// </summary>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        /// <param name="taskId">the task that is supposed to be removed.</param>
        /// <exception cref="Exception"> thown if the user is not the assingee of the given task </exception>
        internal Task DeleteTask(string userEmail, int taskId)
        {
            if (TasksList[taskId].checkAssingee(userEmail))
            {
                Task task = TasksList[taskId];
                TasksList.Remove(taskId);
                log.Debug("Task removed from column successfully");
                return task;
            }
            else
            {
                log.Error("User is not the assingee of the task");
                throw new Exception("User is not the assingee of the task");
            }
        }

        /// <summary>
        /// checkes if a task is in the column
        /// </summary>
        /// <param name="taskId">the task that is supposed to be checked whether its in the column.</param>
        /// <returns>a boolean whether the task is in the column</returns>
        internal bool checkTaskExists(int taskId)
        {
            return TasksList.ContainsKey(taskId);
        }

        /// <summary>
        /// Returns whether the column is full.
        /// </summary>
        /// <returns>a boolean whether the column is full</returns>
        internal bool isColumnFull()
        {
            return (maxTasks!=-1 & TasksList.Count >= this.maxTasks);
        }

        /// <summary>
        /// Returns number of current tasks.
        /// </summary>
        internal int numOfTasks()
        {
            return TasksList.Count;
        }

        /// <summary>
        /// Creates a new Task Object and adding it to the list of Tasks.
        /// </summary>
        /// <param name="taskID">the ID of the task</param>
        /// <param name="title">Title of the new task</param>
        /// <param name="description">Description of the new task</param>
        /// <param name="dueDate">The due date if the new task</param>
        /// <param name="userEmail">Email of the current user. Must be logged in</param>
        internal Task CreateTask(int taskID, string title, string description, DateTime dueDate, string userEmail)
        {
            Task newTask = new Task(taskID, title, description, dueDate, userEmail);
            TasksList[taskID] = newTask;
            return newTask;
        }

        /// <summary>
        /// returns a list of all the taskIDs from the column.
        /// </summary>
        internal List<int> GetTaskIDs()
        {
            List<int> result = new List<int>();
            foreach (int key in TasksList.Keys)
            {
                result.Add(key);
            }
            return result;
        }

        /// <summary>
        /// returns a list of all the task from the column.
        /// </summary>
        internal List<Task> GetTasks()
        {
            List<Task> result = new List<Task>();
            foreach (Task t in TasksList.Values)
            {
                result.Add(t);
            }
            return result;
        }

        /// <summary>
        /// Update the due date of a task
        /// </summary>
        /// <param name="userEmail">Email of the user. Must be logged in</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDueDate">The new due date of the task</param>
        /// <exception cref="Exception"> thown if the user is not the assingee of the given task </exception>
        internal void UpdateTaskDueDate(string userEmail, int taskId, DateTime newDueDate)
        {
            if(TasksList[taskId].checkAssingee(userEmail))
                TasksList[taskId].DueDate = newDueDate;
            else
            {
                log.Error("User is not the assingee of the task");
                throw new Exception("User is not the assingee of the task");
            }
        }

        /// <summary>
        /// Update the Title of a task
        /// </summary>
        /// <param name="userEmail">Email of the user. Must be logged in</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newTitle">The new Title of the task</param>
        /// <exception cref="Exception"> thown if the user is not the assingee of the given task </exception>
        internal void UpdateTaskTitle(string userEmail, int taskId, string newTitle)
        {
            if (TasksList[taskId].checkAssingee(userEmail))
                TasksList[taskId].Title = newTitle;
            else
            {
                log.Error("User is not the assingee of the task");
                throw new Exception("User is not the assingee of the task");
            }
        }

        /// <summary>
        /// Update the Description of a task
        /// </summary>
        /// <param name="userEmail">Email of the user. Must be logged in</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="newDescription">The new Description of the task</param>
        /// <exception cref="Exception"> thown if the user is not the assingee of the given task </exception>
        internal void UpdateTaskDescription(string userEmail, int taskId, string newDescription)
        {
            if (TasksList[taskId].checkAssingee(userEmail))
                TasksList[taskId].Description = newDescription;
            else
            {
                log.Error("User is not the assingee of the task");
                throw new Exception("User is not the assingee of the task");
            }
        }

        /// <summary>
        /// Update the Assingee of a task
        /// </summary>
        /// <param name="userEmail">Email of the user. Must be logged in</param>
        /// <param name="taskId">The task to be updated identified task ID</param>
        /// <param name="emailAssignee">The new Sssingee of the task</param>
        /// <exception cref="Exception"> thown if the user is not the assingee of the given task </exception>
        internal void UpdateTaskAssingee(string userEmail, int taskId, string emailAssignee)
        {
            if (TasksList[taskId].checkAssingee(userEmail))
                TasksList[taskId].EmailAssignee = emailAssignee;
            else
            {
                log.Error("User is not the assingee of the task");
                throw new Exception("User is not the assingee of the task");
            }
        }

        /// <summary>
        /// return a Task object by it's ID
        /// </summary>
        /// <param name="taskId">The ID of the requested Task</param>
        /// <returns>A Task object with the given ID</returns>
        internal Task GetTaskByID(int taskId)
        {
            return TasksList[taskId];
        }
    }
}