using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using IntroSE.Kanban.Backend.ServiceLayer;
using System.Runtime.CompilerServices;
using STask = IntroSE.Kanban.Backend.ServiceLayer.STask;
using SUser = IntroSE.Kanban.Backend.ServiceLayer.SUser;

/*
 using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Tests")]
*/

namespace Tests
{
    [TestFixture]
    public class UnitTests
    {
        Service s;

        private readonly string user0 = "IamNotExist@gmail.com";
        private readonly string user1 = "user1@gmail.com";
        private readonly string user2 = "user2@walla.co.il";
        private readonly string user3 = "user3@post.bgu.ac.il";

        private readonly string pass1 = "123456Aa";
        private readonly string pass2 = "aAaA1234";
        private readonly string pass3 = "1a2b3c4D";

        private readonly string board1 = "board1";
        private readonly string board2 = "board2";
        private readonly string board3 = "board3";

        private readonly DateTime dueDate1 = DateTime.Today.AddDays(1);
        private readonly DateTime dueDate2 = DateTime.Today.AddDays(2);
        private readonly DateTime dueDate3 = DateTime.Today.AddDays(3);

        private STask task1;
        private STask task2;
        private STask task3;

        [SetUp]
        public void Init()
        {
            CleanUP();
            s = new Service();
            s.Register(user1, pass1);
            s.Register(user2, pass2);
            s.Register(user3, pass3);
            s.Login(user1, pass1);
            s.Login(user2, pass2);
            s.Login(user3, pass3);
            s.AddBoard(user1, board1);
            s.AddBoard(user2, board2);
            s.AddBoard(user3, board3);
            s.JoinBoard(user2, user1, board1);
            s.JoinBoard(user3, user1, board1);
            task1 = s.AddTask(user1, user1, board1, "task1", "desc1", dueDate1).Value;
            task2 = s.AddTask(user2, user2, board2, "task2", "desc2", dueDate2).Value;
            task3 = s.AddTask(user3, user3, board3, "task3", "desc3", dueDate3).Value;

            //user0 - not registerd
            //user1 - registerd, logged in, member of board1, assignee of task1, 0 task in progress
            //user2 - registerd, logged in, member of board1 and board2, assignee of task2, 0 task in progress
            //user3 - registerd, logged in, member of board1 and board3, assignee of task3, 0 task in progress
            //board1 - creator: user1. members: user1, user2, user3. column limits: none. tasks: backlog/task1
            //board2 - creator: user2. members: user2. column limits: none. tasks: backlog/task2
            //board3 - creator: user3. members: user3. column limits: none. tasks: backlog/task3
            //task1 - board: user1,board1. column: 0. title: task1. desc: desc1. due date: dueDate1. assignee: user1
            //task2 - board: user2,board2. column: 0. title: task2. desc: desc2. due date: dueDate2. assignee: user2
            //task3 - board: user3,board3. column: 0. title: task3. desc: desc3. due date: dueDate3. assignee: user3
        }

        [TearDown]
        public void CleanUP()
        {
            s = new Service();
            s.DeleteData();
        }

        [Test]
        public void Register()
        {
            Response r1 = s.Register("ali@gmail.com", "12345aaA");
            Response r2 = s.Register(user1, "606060bB");
            Response r3 = s.Register("yosi2@gmail.com", "1234");
            Response r4 = s.Register("a@@gmail.com", "11112222bB");
            Response r5 = s.Register(null, "aB123456");
            Response r6 = s.Register("a12@gmail.com", null);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("user already exist", r2.ErrorMessage);
            Assert.AreEqual("invalid password", r3.ErrorMessage);
            Assert.AreEqual("invalid email", r4.ErrorMessage);
            Assert.AreEqual("must pass an email", r5.ErrorMessage);
            Assert.AreEqual("must pass a password", r6.ErrorMessage);
        }

        [Test]
        public void Login()
        {
            s.Logout(user1);
            s.Logout(user2);
            s.Logout(user3);

            Response<SUser> r1 = s.Login(user1, pass1);
            Response<SUser> r2 = s.Login(user2, pass2);
            Response<SUser> r3 = s.Login(user0, pass1);
            Response<SUser> r4 = s.Login(user1, pass1);

            Assert.AreEqual(user1, r1.Value.Email);
            Assert.AreEqual(user2, r2.Value.Email);
            Assert.AreEqual("user do not exist", r3.ErrorMessage);
            Assert.AreEqual("user already logged in", r4.ErrorMessage);
        }

        [Test]
        public void Logout()
        {
            Response r1 = s.Logout(user1);
            Response r2 = s.Logout(user1);
            Response r3 = s.Logout(user0);
            Response r4 = s.Logout(user2);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r2.ErrorMessage);
            Assert.AreEqual("user do not exist", r3.ErrorMessage);
            Assert.IsNull(r4.ErrorMessage);
        }

        [Test]
        public void AddBoard()
        {
            s.Logout(user3);

            Response r1 = s.AddBoard(user3, "exp");
            Response r2 = s.AddBoard(user1, "b1");
            Response r3 = s.AddBoard(user1, "b2");
            Response r4 = s.AddBoard(user2, "b3");
            Response r5 = s.AddBoard(user2, "b1");
            Response r6 = s.AddBoard(user1, "b1");

            Assert.AreEqual("user isnt logged in", r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.IsNull(r3.ErrorMessage);
            Assert.IsNull(r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            Assert.AreEqual("this board already exists", r6.ErrorMessage);
        }

        [Test]
        public void JoinBoard()
        {
            s.Logout(user3);

            Response r1 = s.JoinBoard(user1, user3, board3);
            Response r2 = s.JoinBoard(user2, user1, board1);
            Response r3 = s.JoinBoard(user2, user3, board3);
            Response r4 = s.JoinBoard(user1, user3, "b1");
            Response r5 = s.JoinBoard(user1, user2, "b10");
            Response r6 = s.JoinBoard(user3, user2, board2);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("The user is already a board member", r2.ErrorMessage);
            Assert.IsNull(r3.ErrorMessage);
            Assert.AreEqual("board does not exists", r4.ErrorMessage);
            Assert.AreEqual("board does not exists", r5.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r6.ErrorMessage);
        }

        [Test]
        public void RemoveBoard()
        {
            s.Logout(user3);
            s.JoinBoard(user1, user3, board3);

            Response r1 = s.RemoveBoard(user2, user2, board2);
            Response r2 = s.RemoveBoard(user2, user1, board1);
            Response r3 = s.RemoveBoard(user1, user1, board1);
            Response r4 = s.RemoveBoard(user3, user3, board3);
            Response r5 = s.RemoveBoard(user2, user3, board3);
            Response r6 = s.RemoveBoard(user1, user3, board3);
            Response r7 = s.RemoveBoard(user1, user1, board1);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("the user isnt the board creator", r2.ErrorMessage);
            Assert.IsNull(r3.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r4.ErrorMessage);
            Assert.AreEqual("the user isnt the board creator", r5.ErrorMessage);//not implemented yet
            Assert.AreEqual("the user isnt the board creator", r6.ErrorMessage);
            Assert.AreEqual("board does not exists", r7.ErrorMessage);
        }

        [Test]
        public void LimitColumn()
        {
            s.JoinBoard(user2, user1, board1);

            Response r1 = s.LimitColumn(user1, user1, board1, 0, 10);
            Response r2 = s.LimitColumn(user2, user1, board1, 1, 15);
            Response r3 = s.LimitColumn(user2, user3, board3, 2, 20);
            Response r4 = s.LimitColumn(user1, user2, board1, 2, 20);
            Response r5 = s.LimitColumn(user1, user1, board1, 2, 20);

            s.Logout(user1);

            Response r6 = s.LimitColumn(user1, user1, board1, 0, 1);

            Assert.IsNull(r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r3.ErrorMessage);//not implemented yet
            Assert.AreEqual("board does not exists", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r6.ErrorMessage);
        }

        [Test]
        public void GetColumnLimit()
        {
            Response<int> r1 = s.GetColumnLimit(user1, user1, board1, 0);
            Response<int> r2 = s.GetColumnLimit(user2, user1, board1, 1);

            s.LimitColumn(user1, user1, board1, 2, 10);

            Response<int> r3 = s.GetColumnLimit(user3, user1, board1, 2);
            Response<int> r4 = s.GetColumnLimit(user1, user2, board2, 0);
            Response<int> r5 = s.GetColumnLimit(user1, user2, board1, 1);

            s.Logout(user1);

            Response<int> r6 = s.GetColumnLimit(user1, user1, board1, 2);

            //Assert.AreEqual(-1, r1.Value);
            //Assert.AreEqual(-1, r2.Value);
            Assert.AreEqual(10, r3.Value);
            Assert.AreEqual("The user isnt a board member", r4.ErrorMessage);
            Assert.AreEqual("board does not exists", r5.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r6.ErrorMessage);
        }

        [Test]
        public void GetColumnName()
        {
            Response<string> r1 = s.GetColumnName(user1, user1, board1, 0);
            Response<string> r2 = s.GetColumnName(user2, user1, board1, 1);
            Response<string> r3 = s.GetColumnName(user3, user1, board1, 2);
            Response<string> r4 = s.GetColumnName(user1, user3, board3, 0);
            Response<string> r5 = s.GetColumnName(user1, user1, board1, 3);

            Assert.AreEqual("backlog", r1.Value);
            Assert.AreEqual("in progress", r2.Value);
            Assert.AreEqual("done", r3.Value);
            Assert.AreEqual("The user isnt a board member", r4.ErrorMessage);
            Assert.AreEqual("no such column", r5.ErrorMessage);
        }

        [Test]
        public void AddTask()
        {
            Response<STask> r1 = s.AddTask(user1, user1, board1, "t1", "firstTask", dueDate1);
            Response<STask> r2 = s.AddTask(user2, user1, board1, "t2", "secondTask", dueDate2);

            s.LimitColumn(user1, user1, board1, 0, 3);

            Response<STask> r3 = s.AddTask(user3, user1, board1, "exp", "error", dueDate1);
            Response<STask> r4 = s.AddTask(user1, user2, board2, "exp", "error", dueDate1);

            s.Logout(user3);

            Response<STask> r5 = s.AddTask(user3, user3, board3, "exp", "logoutUser", dueDate3);

            Assert.AreEqual("t1", r1.Value.Title);
            Assert.AreEqual("firstTask", r1.Value.Description);
            Assert.AreEqual(dueDate1, r1.Value.DueDate);
            Assert.AreEqual("t2", r2.Value.Title);
            Assert.AreEqual("secondTask", r2.Value.Description);
            Assert.AreEqual(dueDate2, r2.Value.DueDate);
            Assert.AreEqual("Backlog is full", r3.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r4.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r5.ErrorMessage);
        }

        [Test]
        public void AssignTask()
        {
            s.JoinBoard(user1, user3, board3);

            Response r1 = s.AssignTask(user1, user1, board1, 0, task1.Id, user2);
            Response r2 = s.AssignTask(user2, user1, board1, 0, task1.Id, user2);
            Response r3 = s.AssignTask(user3, user2, board2, 0, task2.Id, user1);
            Response r4 = s.AssignTask(user2, user3, board3, 0, task3.Id, user1);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("the user is already the task's assignee", r2.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r3.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r4.ErrorMessage);
            CleanUP();
        }

        [Test]
        public void UpdateTaskDueDate()
        {
            s.JoinBoard(user1, user3, board3);
            s.AssignTask(user3, user3, board3, 0, task3.Id, user1);

            Response r1 = s.UpdateTaskDueDate(user1, user1, board1, 0, task1.Id, dueDate2);
            Response r2 = s.UpdateTaskDueDate(user2, user1, board1, 0, task1.Id, dueDate3);
            Response r3 = s.UpdateTaskDueDate(user3, user3, board3, 0, task3.Id, dueDate1);
            Response r4 = s.UpdateTaskDueDate(user2, user3, board3, 0, task3.Id, dueDate1);
            Response r5 = s.UpdateTaskDueDate(user1, user3, board3, 0, task3.Id, dueDate1);

            //STask newTask1 = s.GetTaskByID(task1.Id);
            //STask newTask3 = s.GetTaskByID(task3.Id);
            Assert.IsNull(r1.ErrorMessage);
            //Assert.AreEqual(dueDate2, s.GetTaskByID(task1.Id).DueDate);
            Assert.AreEqual("User is not the assingee of the task", r2.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r3.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            //Assert.AreEqual(dueDate1, s.GetTaskByID(task3.Id).DueDate);
            CleanUP();
        }

        [Test]
        public void UpdateTaskTitle()
        {
            s.JoinBoard(user1, user3, board3);
            s.AssignTask(user3, user3, board3, 0, task3.Id, user1);

            Response r1 = s.UpdateTaskTitle(user1, user1, board1, 0, task1.Id, "newTitle1");
            Response r2 = s.UpdateTaskTitle(user2, user1, board1, 0, task1.Id, "newTitle1");
            Response r3 = s.UpdateTaskTitle(user3, user3, board3, 0, task3.Id, "newTitle1");
            Response r4 = s.UpdateTaskTitle(user2, user3, board3, 0, task3.Id, "newTitle1");
            Response r5 = s.UpdateTaskTitle(user1, user3, board3, 0, task3.Id, "newTitle3");

            //STask newTask1 = s.GetTaskByID(task1.Id);
            //STask newTask3 = s.GetTaskByID(task3.Id);
            Assert.IsNull(r1.ErrorMessage);
            //Assert.AreEqual("newTitle1", newTask1.Title);
            Assert.AreEqual("User is not the assingee of the task", r2.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r3.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            //Assert.AreEqual("newTitle3", newTask3.Title);
            CleanUP();
        }

        [Test]
        public void UpdateTaskDescription()
        {
            s.JoinBoard(user1, user3, board3);
            s.AssignTask(user3, user3, board3, 0, task3.Id, user1);

            Response r1 = s.UpdateTaskDescription(user1, user1, board1, 0, task1.Id, "newDesc1");
            Response r2 = s.UpdateTaskDescription(user2, user1, board1, 0, task1.Id, "newDesc1");
            Response r3 = s.UpdateTaskDescription(user3, user3, board3, 0, task3.Id, "newDesc1");
            Response r4 = s.UpdateTaskDescription(user2, user3, board3, 0, task3.Id, "newDesc1");
            Response r5 = s.UpdateTaskDescription(user1, user3, board3, 0, task3.Id, "newDesc3");

            //STask newTask1 = s.GetTaskByID(task1.Id);
            //STask newTask3 = s.GetTaskByID(task3.Id);
            Assert.IsNull(r1.ErrorMessage);
            //Assert.AreEqual("newDesc1", newTask1.Description);
            Assert.AreEqual("User is not the assingee of the task", r2.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r3.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            //Assert.AreEqual("newDesc3", newTask3.Description);
            CleanUP();
        }

        [Test]
        public void AdvanceTask()
        {
            s.JoinBoard(user1, user3, board3);
            s.AssignTask(user3, user3, board3, 0, task3.Id, user1);

            Response r1 = s.AdvanceTask(user1, user1, board1, 0, task1.Id);
            Response r2 = s.AdvanceTask(user1, user1, board1, 0, task1.Id);
            Response r3 = s.AdvanceTask(user2, user1, board1, 1, task1.Id);
            Response r4 = s.AdvanceTask(user1, user1, board1, 1, task1.Id);
            Response r5 = s.AdvanceTask(user3, user3, board3, 0, task3.Id);
            Response r6 = s.AdvanceTask(user1, user3, board3, 0, task3.Id);
            Response r7 = s.AdvanceTask(user1, user1, board1, 2, task1.Id);
            Response r8 = s.AdvanceTask(user1, user2, board2, 0, task2.Id);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("Task could not be found", r2.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r3.ErrorMessage);
            Assert.IsNull(r4.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r5.ErrorMessage);
            Assert.IsNull(r6.ErrorMessage);
            Assert.AreEqual("cannot advance done tasks", r7.ErrorMessage);
            Assert.AreEqual("User is not the assingee of the task", r8.ErrorMessage);
        }

        [Test]
        public void GetColumn()
        {
            s.Logout(user3);
            STask t1 = s.AddTask(user1, user1, board1, "t1", "d1", dueDate1).Value;
            STask t2 = s.AddTask(user2, user1, board1, "t2", "d2", dueDate1).Value;
            STask t3 = s.AddTask(user2, user1, board1, "t3", "d3", dueDate1).Value;
            STask t4 = s.AddTask(user1, user1, board1, "t4", "d4", dueDate1).Value;
            STask t5 = s.AddTask(user1, user1, board1, "t5", "d5", dueDate1).Value;
            STask t6 = s.AddTask(user1, user1, board1, "t6", "d6", dueDate1).Value;
            STask t7 = s.AddTask(user1, user1, board1, "t7", "d7", dueDate1).Value;
            //Task t8 = s.AddTask(user2, user1, board1, "t8", "d8", dueDate1).Value;
            s.AdvanceTask(user1, user1, board1, 0, t1.Id);
            s.AdvanceTask(user1, user1, board1, 1, t1.Id);
            s.AdvanceTask(user2, user1, board1, 0, t2.Id);
            s.AdvanceTask(user2, user1, board1, 1, t2.Id);
            s.AdvanceTask(user2, user1, board1, 0, t3.Id);
            s.AdvanceTask(user2, user1, board1, 1, t3.Id);
            s.AdvanceTask(user1, user1, board1, 0, t4.Id);
            s.AdvanceTask(user1, user1, board1, 0, t5.Id);
            s.AdvanceTask(user1, user1, board1, 0, t6.Id);
            s.AdvanceTask(user1, user1, board1, 0, t7.Id);
            //user1,board1 - column 0: 2. column 1: 4. column 2: 3

            Response<IList<STask>> r1 = s.GetColumn(user1, user1, board1, 0);
            Response<IList<STask>> r2 = s.GetColumn(user1, user1, board1, 1);
            Response<IList<STask>> r3 = s.GetColumn(user1, user1, board1, 2);
            Response<IList<STask>> r4 = s.GetColumn(user3, user1, board1, 0);
            Response<IList<STask>> r5 = s.GetColumn(user1, user2, board2, 0);
            Response<IList<STask>> r6 = s.GetColumn(user1, user2, board1, 0);

            Assert.AreEqual(1, r1.Value.Count);
            Assert.AreEqual(4, r2.Value.Count);
            Assert.AreEqual(3, r3.Value.Count);
            Assert.AreEqual("user isnt logged in", r4.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r5.ErrorMessage);
            Assert.AreEqual("board does not exists", r6.ErrorMessage);
        }

        [Test]
        public void InProgressTasks()
        {
            s.JoinBoard(user1, user3, board3);
            STask t1 = s.AddTask(user1, user1, board1, "t1", "d1", dueDate1).Value;
            s.AdvanceTask(user1, user1, board1, 0, t1.Id);
            STask t2 = s.AddTask(user2, user1, board1, "t2", "d2", dueDate1).Value;
            s.AdvanceTask(user2, user1, board1, 0, t2.Id);
            STask t3 = s.AddTask(user3, user1, board1, "t3", "d3", dueDate1).Value;
            s.AdvanceTask(user3, user1, board1, 0, t3.Id);
            STask t4 = s.AddTask(user3, user3, board3, "t4", "d4", dueDate1).Value;
            s.AdvanceTask(user3, user3, board3, 0, t4.Id);
            STask t5 = s.AddTask(user3, user3, board3, "t5", "d5", dueDate1).Value;
            s.AssignTask(user3, user3, board3, 0, t5.Id, user1);
            s.AdvanceTask(user1, user3, board3, 0, t5.Id);
            STask t6 = s.AddTask(user3, user3, board3, "t6", "d6", dueDate1).Value;
            s.AdvanceTask(user3, user3, board3, 0, t6.Id);
            s.AssignTask(user3, user3, board3, 1, t6.Id, user1);

            Response<IList<STask>> r1 = s.InProgressTasks(user1);
            Response<IList<STask>> r2 = s.InProgressTasks(user2);
            Response<IList<STask>> r3 = s.InProgressTasks(user3);

            Assert.AreEqual(3, r1.Value.Count);
            Assert.AreEqual(1, r2.Value.Count);
            Assert.AreEqual(2, r3.Value.Count);
        }


        [Test]
        public void GetBoardNames()
        {
            s.Logout(user3);
            s.JoinBoard(user1, user2, board2);
            s.RemoveBoard(user2, user2, board2);
            s.AddBoard(user1, "b1");
            s.AddBoard(user2, "b2");
            s.AddBoard(user3, "b3");
            s.JoinBoard(user1, user2, "b2");
            s.JoinBoard(user1, user3, "b3");

            Response<IList<String>> r1 = s.GetBoardNames(user1);
            Response<IList<String>> r2 = s.GetBoardNames(user2);
            Response<IList<String>> r3 = s.GetBoardNames(user3);
            Response<IList<String>> r4 = s.GetBoardNames(user0);

            Assert.IsTrue(r1.Value.Contains(board1));
            Assert.IsTrue(r1.Value.Contains("b2"));
            Assert.IsFalse(r1.Value.Contains("b3"));
            Assert.IsFalse(r1.Value.Contains(board2));
            Assert.AreEqual(3, r1.Value.Count);
            Assert.IsFalse(r2.Value.Contains(board2));
            Assert.IsTrue(r2.Value.Contains("b2"));
            Assert.IsFalse(r2.Value.Contains("b3"));
            Assert.AreEqual(2, r2.Value.Count);
            Assert.AreEqual("user isnt logged in", r3.ErrorMessage);
            Assert.AreEqual("user do not exist", r4.ErrorMessage);
        }

        [Test]
        public void LoadData()
        {
            int t1Id = s.AddTask(user1, user1, board1, "t1", "d1", dueDate1).Value.Id;
            s.AdvanceTask(user1, user1, board1, 0, t1Id);
            int t2Id = s.AddTask(user2, user2, board2, "t2", "d2", dueDate1).Value.Id;
            s.AdvanceTask(user2, user2, board2, 0, t2Id);
            int t3Id = s.AddTask(user3, user3, board3, "t3", "d3", dueDate1).Value.Id;
            s.AdvanceTask(user3, user3, board3, 0, t3Id);
            int t4Id = s.AddTask(user2, user1, board1, "t4", "d4", dueDate1).Value.Id;
            s.AdvanceTask(user2, user1, board1, 0, t4Id);
            int t5Id = s.AddTask(user1, user1, board1, "t5", "d5", dueDate1).Value.Id;
            s.AssignTask(user1, user1, board1, 0, t5Id, user3);
            s.AdvanceTask(user3, user1, board1, 0, t5Id);
            s.AddColumn(user1, user1, board1, 0, "zero");
            s.AddColumn(user1, user1, board1, 1, "one");
            //tasks in progress:
            //user1: t1-board1
            //user2: t2-board2, t4-board1
            //user3: t3-board3, t5-board5
            //boards:
            //user1: board1
            //user2: board1, board2
            //user3: board1, board3
            Service s2 = new Service();

            Response<SUser> r1 = s2.Login(user1, pass1);
            Response<SUser> r2 = s2.Login(user2, pass2);
            Response<SUser> r3 = s2.Login(user3, pass3);
            Response<SUser> r4 = s2.Login(user0, pass1);
            Response<IList<String>> r5 = s2.GetBoardNames(user1);
            Response<IList<String>> r6 = s2.GetBoardNames(user2);
            Response<IList<String>> r7 = s2.GetBoardNames(user3);
            Response<IList<STask>> r8 = s2.InProgressTasks(user1);
            Response<IList<STask>> r9 = s2.InProgressTasks(user2);
            Response<IList<STask>> r10 = s2.InProgressTasks(user3);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual(user1, r1.Value.Email);
            Assert.IsNull(r2.ErrorMessage);
            Assert.AreEqual(user2, r2.Value.Email);
            Assert.IsNull(r3.ErrorMessage);
            Assert.AreEqual(user3, r3.Value.Email);
            Assert.AreEqual("user do not exist", r4.ErrorMessage);
            Assert.AreEqual(1, r5.Value.Count);
            Assert.IsTrue(r5.Value.Contains(board1));
            Assert.AreEqual(2, r6.Value.Count);
            Assert.IsTrue(r6.Value.Contains(board1));
            Assert.IsTrue(r6.Value.Contains(board2));
            Assert.AreEqual(2, r7.Value.Count);
            Assert.IsTrue(r7.Value.Contains(board1));
            Assert.IsTrue(r7.Value.Contains(board3));
            Assert.AreEqual(2, r8.Value.Count);
            //Assert.IsTrue(r8.Value.Contains(s2.GetTaskByID(t1Id)));
            Assert.AreEqual(2, r9.Value.Count);
            //Assert.IsTrue(r9.Value.Contains(s2.GetTaskByID(t2Id)));
            //Assert.IsTrue(r9.Value.Contains(s2.GetTaskByID(t4Id)));
            Assert.AreEqual(2, r10.Value.Count);
            //Assert.IsTrue(r10.Value.Contains(s2.GetTaskByID(t3Id)));
            //Assert.IsTrue(r10.Value.Contains(s2.GetTaskByID(t5Id)));
            CleanUP();
        }

        [Test]
        public void DeleteData()
        {
            s.DeleteData();
            s = new Service();
            s.LoadData();

            Response<SUser> r1 = s.Login(user1, pass1);
            Response r2 = s.Register(user1, pass1);
            Response<SUser> r3 = s.Login(user2, pass2);
            Response r4 = s.Register(user2, pass2);
            Response<SUser> r5 = s.Login(user3, pass3);
            Response r6 = s.Register(user3, pass3);
            Response<SUser> r7 = s.Login(user3, pass3);


            Assert.AreEqual("user do not exist", r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.AreEqual("user do not exist", r3.ErrorMessage);
            Assert.IsNull(r4.ErrorMessage);
            Assert.AreEqual("user do not exist", r5.ErrorMessage);
            Assert.IsNull(r6.ErrorMessage);
            Assert.IsNull(r7.ErrorMessage);
        }

        [Test]
        public void AddColumn()
        {
            s.Logout(user3);

            Response r1 = s.AddColumn(user1, user1, board1, 0, "newCol1");
            Response r2 = s.AddColumn(user2, user1, board1, 0, "newCol2");
            Response r3 = s.AddColumn(user1, user2, board2, 0, "exp");
            Response r4 = s.AddColumn(user3, user3, board3, 0, "exp");
            Response r5 = s.AddColumn(user2, user2, board2, 3, "newCol3");
            Response r6 = s.AddColumn(user2, user2, board2, 6, "exp");

            Assert.IsNull(r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r3.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            Assert.AreEqual("no such column", r6.ErrorMessage);



            //AddColumn(string userEmail, string creatorEmail, string boardName, int columnOrdinal, string columnName)
        }

        [Test]
        public void RemoveColumn()
        {
            s.Logout(user3);

            Response r1 = s.RemoveColumn(user2, user2, board2, 0);
            Response r2 = s.RemoveColumn(user2, user2, board2, 0);
            Response r3 = s.RemoveColumn(user1, user2, board2, 0);
            Response r4 = s.RemoveColumn(user3, user1, board1, 0);
            Response r5 = s.RemoveColumn(user2, user1, board1, 1);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("minimun column limit reached, cant remove column", r2.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r3.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
        }

        [Test]
        public void RenameColumn()
        {
            s.Logout(user3);

            Response r1 = s.RenameColumn(user1, user1, board1, 0, "Col1");
            Response r2 = s.RenameColumn(user2, user1, board1, 2, "Col2");
            Response r3 = s.RenameColumn(user1, user2, board2, 0, "exp");
            Response r4 = s.RenameColumn(user3, user3, board3, 0, "exp");
            Response r5 = s.RenameColumn(user2, user2, board2, 3, "exp");

            Assert.IsNull(r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r3.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r4.ErrorMessage);
            Assert.AreEqual("no such column", r5.ErrorMessage);
        }

        [Test]
        public void MoveColumn()
        {
            s.Logout(user3);

            Response r1 = s.MoveColumn(user1, user1, board1, 0, 2);
            Response r2 = s.MoveColumn(user1, user1, board1, 1, 0);
            Response r3 = s.MoveColumn(user2, user1, board1, 2, -2);
            Response r4 = s.MoveColumn(user1, user2, board2, 0, 1);
            Response r5 = s.MoveColumn(user3, user3, board3, 0, 1);
            Response r6 = s.MoveColumn(user2, user2, board2, 3, 1);

            Assert.AreEqual(r1.ErrorMessage, "attempted moving a column that is not empty");
            Assert.IsNull(r2.ErrorMessage);
            Assert.IsNull(r3.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r4.ErrorMessage);
            Assert.AreEqual("user isnt logged in", r5.ErrorMessage);
            Assert.AreEqual("no such column", r6.ErrorMessage);
        }

        [Test]
        public void AddColumn2()
        {
            //user0 - not registerd
            //user1 - registerd, logged in, member of board1, assignee of task1, 0 task in progress
            //user2 - registerd, logged in, member of board1 and board2, assignee of task2, 0 task in progress
            //user3 - registerd, logged in, member of board1 and board3, assignee of task3, 0 task in progress
            //board1 - creator: user1. members: user1, user2, user3. column limits: none. tasks: backlog/task1
            //board2 - creator: user2. members: user2. column limits: none. tasks: backlog/task2
            //board3 - creator: user3. members: user3. column limits: none. tasks: backlog/task3
            //task1 - board: user1,board1. column: 0. title: task1. desc: desc1. due date: dueDate1. assignee: user1
            //task2 - board: user2,board2. column: 0. title: task2. desc: desc2. due date: dueDate2. assignee: user2
            //task3 - board: user3,board3. column: 0. title: task3. desc: desc3. due date: dueDate3. assignee: user3

            Init();
            Response r1 = s.AddColumn(user1, user1, board1, 0, "zero");
            Response r2 = s.AddColumn(user1, user1, board1, 1, "one");
            Response r3 = s.AddColumn(user1, user1, board1, 6, "zero");
            Response r4 = s.AddColumn(user1, user1, board1, -1, "zero");
            Response r5 = s.AddColumn(user2, user3, board3, 1, "one");
            Response r6 = s.AddColumn(user2, user2, board2, 3, "three");

            Assert.IsNull(r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.AreEqual("no such column", r3.ErrorMessage);
            Assert.AreEqual("no such column", r4.ErrorMessage);
            Assert.AreEqual("The user isnt a board member", r5.ErrorMessage);
            Assert.IsNull(r6.ErrorMessage);
            CleanUP();
        }

        [Test]
        public void RemoveColumn2()
        {
            Init();
            s.AddColumn(user1, user1, board1, 0, "zero");
            s.AddColumn(user1, user1, board1, 1, "one");


            Response r1 = s.RemoveColumn(user1, user1, board1, 2);
            Response r2 = s.RemoveColumn(user1, user1, board1, 0);
            Response r3 = s.RemoveColumn(user1, user1, board1, 1);
            Response r4 = s.RemoveColumn(user1, user1, board1, 1);

            Assert.IsNull(r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.IsNull( r3.ErrorMessage);
            Assert.AreEqual("minimun column limit reached, cant remove column", r4.ErrorMessage);
            CleanUP();
        }


        [Test]
        public void MoveColumn2()
        {
            Init();
            s.AddColumn(user1, user1, board1, 0, "zero");
            s.AddColumn(user1, user1, board1, 1, "one");


            Response r1 = s.MoveColumn(user1, user1, board1, 0, 2);
            Response r2 = s.MoveColumn(user2, user2, board2, 0, 1);
            Response r3 = s.MoveColumn(user1, user1, board1, 4, -2);
            Response r4 = s.MoveColumn(user1, user1, board1, 0, 8);
            Response r5 = s.MoveColumn(user1, user1, board1, 0, 0);

            Assert.IsNull(r1.ErrorMessage);
            Assert.AreEqual("attempted moving a column that is not empty", r2.ErrorMessage);
            Assert.IsNull(r3.ErrorMessage);
            Assert.AreEqual("shift size out of bounds", r4.ErrorMessage);
            Assert.IsNull(r5.ErrorMessage);
            CleanUP();
        }

        [Test]
        public void RenameColumn2()
        {
            Init();
            s.AddColumn(user1, user1, board1, 0, "zero");
            s.AddColumn(user1, user1, board1, 1, "one");

            Response r1 = s.RenameColumn(user1, user1, board1, 0, "new zero");
            Response r2 = s.RenameColumn(user1, user1, board1, 1, "new one");
            Response r3 = s.RenameColumn(user1, user1, board1, 3, "");
            Response r4 = s.RenameColumn(user1, user1, board1, -5, "err");
            

            Assert.IsNull(r1.ErrorMessage);
            Assert.IsNull(r2.ErrorMessage);
            Assert.IsNull(r3.ErrorMessage);
            Assert.AreEqual("no such column", r4.ErrorMessage);
            CleanUP();
        }
    }
}
