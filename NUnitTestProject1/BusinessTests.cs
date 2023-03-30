using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using IntroSE.Kanban.Backend.business;
using System.Runtime.CompilerServices;
using Moq;
using User = IntroSE.Kanban.Backend.business.User;
//using IBoard = IntroSE.Kanban.Backend.business.IBoard;
//using ImprovedBoardController = IntroSE.Kanban.Backend.business.ImprovedBoardController;

namespace Tests
{
    class BusinessTests
    {
        User u;
        string email = "user@gmail.com";
        string valid_password = "Ab12345";
        string password;
        int boardID_1 = 1;
        int boardID_2 = 2;
        int boardID;
        List<int> boardsList;
        

        [SetUp]
        public void SetUp()
        {
            u = new User(email, valid_password);
            boardsList = new List<int>();
            boardsList.Add(boardID_1);
            u.Boards = boardsList;
        }

        [Test]
        public void AddBoardToUserList_exist_boardID_throws()
        {
            boardID = boardID_1;

            try
            {
                u.AddBoardToUserList(boardID_1);
            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.Not.Null.Or.Empty);
                Assert.That(e.Message, Is.EqualTo("user already board member"));
            }

            Assert.That(boardsList.Count == 1);
        }

        [Test]
        public void AddBoardToUserList_new_board_success()
        {
            boardID = boardID_2;

            u.AddBoardToUserList(boardID_2);

            Assert.That(u.Boards.Count == 2);
            Assert.That(u.Boards.Contains(boardID_2));
        }

        [Test]
        public void IsValidPassword_null_password_throws()
        {
            password = null;
            bool valid = false;

            try
            {
                valid = u.IsValidPassword(password);
            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.Not.Null.Or.Empty);
                Assert.That(e.Message, Is.EqualTo("must pass a password"));
                Assert.IsFalse(valid);
                //Assert.AreEqual("must pass a password", e.Message);
            }
        }

        [Test]
        public void IsValidPassword_too_short_fail()
        {
            password = "Ab1";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_too_long_fail()
        {
            password = "Abcdefghijklmnopqrstuvwxyz1234567";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_only_lower_fail()
        {
            password = "abcdef";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_only_upper_fail()
        {
            password = "ABCDEF";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_only_digit_fail()
        {
            password = "123456";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_no_digit_fail()
        {
            password = "Abcdefghi";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_no_upper_fail()
        {
            password = "abcd1234";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_no_lower_fail()
        {
            password = "ABCD1234";

            bool valid = u.IsValidPassword(password);

            Assert.IsFalse(valid);
        }

        [Test]
        public void IsValidPassword_right_length_comtain_digit_lower_upper_success()
        {
            password = valid_password;

            bool valid = u.IsValidPassword(password);

            Assert.IsTrue(valid);
        }

        [Test]
        public void IsMatchPassword_null_throws()
        {
            password = null;
            bool match = false;
            //var exception = Assert.Throws<Exception>(() => u.IsMatchPassword(password));
            
            
            try
            {
                match = u.IsMatchPassword(password);
            }
            catch (Exception e)
            {
                Assert.That(e.Message, Is.Not.Null.Or.Empty);
                Assert.That(e.Message, Is.EqualTo("must pass a password"));
                Assert.IsFalse(match);
                //Assert.AreEqual("must pass a password", e.Message);
            }
            
        }

        [Test]
        public void IsMatchPassword_wrong_password_fail()
        {
            password = valid_password + "123";

            bool match = u.IsMatchPassword(password);

            Assert.IsFalse(match);
        }

        [Test]
        public void IsMatchPassword_rihgt_password_success()
        {
            password = valid_password;

            bool match = u.IsMatchPassword(password);

            Assert.IsTrue(match);
        }
    }
}
