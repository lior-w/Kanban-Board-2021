using log4net;
using System;
using System.Reflection;

namespace IntroSE.Kanban.Backend.business
{
    internal class Task
    {
        private int id;
        internal int Id
        {
            get { return id; }
            set { id = value; }
        }
        private DateTime creationTime;
        internal DateTime CreationTime
        {
            get { return creationTime;}
            set { creationTime = value; }
        }
        private string title;
        internal string Title
        {
            get { return title; }
            set { title = UpdateTaskTitle(value); }
        }
        private int MAX_TITLE_LENGTH = 50;
        private string description;
        internal string Description
        {
            get { return description; }
            set { description = UpdateTaskDescription(value); }
        }
        private int MAX_DESCRIPTION_LENGTH = 300;
        private DateTime dueDate;
        internal DateTime DueDate
        {
            get { return dueDate; }
            set { dueDate = UpdateTaskDueDate(value); }
        }

        private string emailAssignee;
        internal string EmailAssignee
        {
            get { return emailAssignee; }
            set { emailAssignee = value; }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);


        internal Task(int id, string title, string description, DateTime DueDate, string emailAssignee)
        {
            validateTitle(title);
            validateDescription(description);
            validatedueDate(DueDate);
            this.Id = id;
            this.CreationTime = DateTime.Now;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.emailAssignee = emailAssignee;
        }
        internal Task(int id, string title, string description, DateTime DueDate, DateTime creationTime, string emailAssignee)
        {
            this.Id = id;
            this.CreationTime = creationTime;
            this.Title = title;
            this.Description = description;
            this.DueDate = DueDate;
            this.emailAssignee = emailAssignee;
        }


        /// <summary>
        /// Checks if the given title is vaild.
        /// </summary>
        /// <param name="title">The title to validate</param>
        /// <returns> True if the title given is valid, otherwise throws exception</returns>
        /// <exception> Thrown if given a null title</exception>
        /// <exception> Thrown if the title is empty or has a more than 50 characters </exception>
        private void validateTitle(string title)
        {
            if (title == null)
            {
                log.Warn("Task with null title attempted Creation");
                throw new Exception("missing title");
            }
            if (title.Length < 1 | title.Length > MAX_TITLE_LENGTH)
            {
                log.Warn("Task with invalid length title attempted Creation");
                throw new Exception("title should be between 1 to 50 charecters");
            }
        }

        /// <summary>
        /// Checks if the given description is valid.
        /// </summary>
        /// <param name="description">The description to validate</param>
        /// <returns> True if the description given is valid, otherwise throws exception</returns>
        /// <exception> Thrown if given a null description</exception>
        /// <exception> Thrown if the description has a more than 300 characters </exception>
        private void validateDescription(string description)
        {
            if (description == null)
            {
                log.Warn("Task with null description attempted Creation");
                throw new Exception("missing description");
            }
            if (description.Length > MAX_DESCRIPTION_LENGTH)
            {
                log.Warn("Task with too long description attempted Creation");
                throw new Exception("descreption should be under 500 charecters");
            }
        }

        /// <summary>
        /// Checks if the given due date is valid.
        /// </summary>
        /// <param name="DueDate">The due date to validate</param>
        /// <returns> True if the due date given is valid, otherwise throws exception</returns>
        /// <exception> Thrown if the due date is earlier than the creation time of the task </exception>
        private void validatedueDate(DateTime DueDate)
        {
            if (DateTime.Now.CompareTo(DueDate) >= 0)
            {
                log.Warn("Task with creation Time that's earlier than dueDate attempted Creation");
                throw new Exception("dueDate is supposed to be later than creationTime");
            }
        }

        /// <summary>
        /// Updates the Due Date of the task if the given Due Date is valid
        /// </summary>
        /// <param name="newDueDate">The updated due date</param>
        private DateTime UpdateTaskDueDate(DateTime newDueDate)
        {
            validatedueDate(newDueDate);
            return newDueDate;
        }

        /// <summary>
        /// Updates the Title of the task if the given Title is valid
        /// </summary>
        /// <param name="newTitle">The updated Title</param>
        private string UpdateTaskTitle(string newTitle)
        {
            validateTitle(newTitle);
            return newTitle;
        }

        /// <summary>
        /// Updates the Description of the task if the given Description is valid
        /// </summary>
        /// <param name="newDescription">The updated Description</param>
        private string UpdateTaskDescription(string newDescription)
        {
            validateDescription(newDescription);
            return newDescription;

        }

        /// <summary>
        /// checks if the given email is the assingee of the task
        /// </summary>
        /// <param name="userEmail">The updated Description</param>
        /// <returns>True is the given email is the assingee of the task False otherwise. </returns>
        internal bool checkAssingee(string userEmail)
        {
            return this.EmailAssignee == userEmail;
        }

    }
}
