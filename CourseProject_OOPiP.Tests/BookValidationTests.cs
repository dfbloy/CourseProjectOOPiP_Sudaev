using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourseProject_OOPiP;

namespace CourseProject_OOPiP.Tests
{
    [TestClass]
    public class BookValidationTests
    {
        private Book _book;

        [TestInitialize]
        public void Setup()
        {
            _book = new Book();
        }

        [TestMethod]
        public void Book_Title_Empty_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Title", ""));
        }

        [TestMethod]
        public void Book_Title_Whitespace_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Title", "   "));
        }

        [TestMethod]
        public void Book_Title_Valid_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Title", "Война и мир"));
        }

        [TestMethod]
        public void Book_Year_Empty_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Year", ""));
        }

        [TestMethod]
        public void Book_Year_NotANumber_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Year", "abc"));
        }

        [TestMethod]
        public void Book_Year_TooOld_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Year", "1400"));
        }

        [TestMethod]
        public void Book_Year_TooFuture_ReturnsError()
        {
            int future = DateTime.Now.Year + 5;
            Assert.IsNotNull(_book.ValidateField("Year", future.ToString()));
        }

        [TestMethod]
        public void Book_Year_BorderLower_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Year", "1450"));
        }

        [TestMethod]
        public void Book_Year_BorderUpper_ReturnsNull()
        {
            int upper = DateTime.Now.Year + 1;
            Assert.IsNull(_book.ValidateField("Year", upper.ToString()));
        }

        [TestMethod]
        public void Book_Year_Valid_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Year", "2000"));
        }

        [TestMethod]
        public void Book_Price_Empty_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Price", ""));
        }

        [TestMethod]
        public void Book_Price_Negative_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Price", "-10"));
        }

        [TestMethod]
        public void Book_Price_TooBig_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Price", "2000000"));
        }

        [TestMethod]
        public void Book_Price_NotANumber_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Price", "abc"));
        }

        [TestMethod]
        public void Book_Price_DotSeparator_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Price", "99.99"));
        }

        [TestMethod]
        public void Book_Price_CommaSeparator_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Price", "99,99"));
        }

        [TestMethod]
        public void Book_Price_Zero_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Price", "0"));
        }

        [TestMethod]
        public void Book_Price_Max_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Price", "1000000"));
        }

        [TestMethod]
        public void Book_Author_Empty_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Author", ""));
        }

        [TestMethod]
        public void Book_Author_Valid_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Author", "Толстой"));
        }

        [TestMethod]
        public void Book_Isbn_Empty_ReturnsError()
        {
            Assert.IsNotNull(_book.ValidateField("Isbn", ""));
        }

        [TestMethod]
        public void Book_Isbn_Valid_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("Isbn", "978-3-16-148410-0"));
        }

        [TestMethod]
        public void Book_UnknownProperty_ReturnsNull()
        {
            Assert.IsNull(_book.ValidateField("UnknownProp", "value"));
        }
    }
}