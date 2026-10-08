using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourseProject_OOPiP;

namespace CourseProject_OOPiP.Tests
{
    [TestClass]
    public class ContainsTextTests
    {
        [TestMethod]
        public void ContainsText_EmptyQuery_ReturnsTrue()
        {
            var book = new Book { Title = "Муму" };
            Assert.IsTrue(book.ContainsText(""));
        }

        [TestMethod]
        public void ContainsText_NullQuery_ReturnsTrue()
        {
            var book = new Book { Title = "Муму" };
            Assert.IsTrue(book.ContainsText(null));
        }

        [TestMethod]
        public void ContainsText_Book_TitleMatches_ReturnsTrue()
        {
            var book = new Book { Title = "Преступление и наказание" };
            Assert.IsTrue(book.ContainsText("Преступление"));
        }

        [TestMethod]
        public void ContainsText_Book_CaseInsensitive_ReturnsTrue()
        {
            var book = new Book { Title = "War and Peace" };
            Assert.IsTrue(book.ContainsText("war"));
        }

        [TestMethod]
        public void ContainsText_Book_AuthorMatches_ReturnsTrue()
        {
            var book = new Book { Author = "Достоевский" };
            Assert.IsTrue(book.ContainsText("Досто"));
        }

        [TestMethod]
        public void ContainsText_Book_IsbnMatches_ReturnsTrue()
        {
            var book = new Book { Isbn = "978-5-17-118935-3" };
            Assert.IsTrue(book.ContainsText("118935"));
        }

        [TestMethod]
        public void ContainsText_Book_YearMatches_ReturnsTrue()
        {
            var book = new Book { Year = 1866 };
            Assert.IsTrue(book.ContainsText("1866"));
        }

        [TestMethod]
        public void ContainsText_Book_IgnoresPeriodicity()
        {
            var book = new Book { Periodicity = "Ежемесячно" };
            Assert.IsFalse(book.ContainsText("Ежемесячно"));
        }

        [TestMethod]
        public void ContainsText_Book_IgnoresIssueNumber()
        {
            var book = new Book { IssueNumber = 42 };
            Assert.IsFalse(book.ContainsText("42"));
        }

        [TestMethod]
        public void ContainsText_Magazine_TitleMatches_ReturnsTrue()
        {
            var mag = new Magazine { Title = "Наука и жизнь" };
            Assert.IsTrue(mag.ContainsText("Наука"));
        }

        [TestMethod]
        public void ContainsText_Magazine_IssueNumberMatches_ReturnsTrue()
        {
            var mag = new Magazine { IssueNumber = 42 };
            Assert.IsTrue(mag.ContainsText("42"));
        }

        [TestMethod]
        public void ContainsText_Magazine_PeriodicityMatches_ReturnsTrue()
        {
            var mag = new Magazine { Periodicity = "Еженедельно" };
            Assert.IsTrue(mag.ContainsText("Еженедельно"));
        }

        [TestMethod]
        public void ContainsText_Magazine_IgnoresAuthor()
        {
            var mag = new Magazine { Author = "Иванов" };
            Assert.IsFalse(mag.ContainsText("Иванов"));
        }

        [TestMethod]
        public void ContainsText_Magazine_IgnoresIsbn()
        {
            var mag = new Magazine { Isbn = "111-222" };
            Assert.IsFalse(mag.ContainsText("111-222"));
        }

        [TestMethod]
        public void ContainsText_NoMatch_ReturnsFalse()
        {
            var book = new Book { Title = "Книга", Author = "Автор" };
            Assert.IsFalse(book.ContainsText("xyz"));
        }

        [TestMethod]
        public void ContainsText_NullProperties_DoesNotThrow()
        {
            var book = new Book();
            Assert.IsFalse(book.ContainsText("anything"));
        }
    }
}