using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourseProject_OOPiP;

namespace CourseProject_OOPiP.Tests
{
    [TestClass]
    public class MagazineValidationTests
    {
        private Magazine _mag;

        [TestInitialize]
        public void Setup()
        {
            _mag = new Magazine();
        }

        [TestMethod]
        public void Magazine_Title_Empty_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Title", ""));
        }

        [TestMethod]
        public void Magazine_Title_Whitespace_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Title", "   "));
        }

        [TestMethod]
        public void Magazine_Title_Valid_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Title", "Мурзилка"));
        }

        [TestMethod]
        public void Magazine_Year_Empty_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Year", ""));
        }

        [TestMethod]
        public void Magazine_Year_NotANumber_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Year", "abc"));
        }

        [TestMethod]
        public void Magazine_Year_Before1800_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Year", "1700"));
        }

        [TestMethod]
        public void Magazine_Year_BorderLower_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Year", "1800"));
        }

        [TestMethod]
        public void Magazine_Year_TooFuture_ReturnsError()
        {
            int future = DateTime.Now.Year + 5;
            Assert.IsNotNull(_mag.ValidateField("Year", future.ToString()));
        }

        [TestMethod]
        public void Magazine_Year_BorderUpper_ReturnsNull()
        {
            int upper = DateTime.Now.Year + 1;
            Assert.IsNull(_mag.ValidateField("Year", upper.ToString()));
        }

        [TestMethod]
        public void Magazine_Year_Valid_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Year", "1950"));
        }

        [TestMethod]
        public void Magazine_Price_Empty_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Price", ""));
        }

        [TestMethod]
        public void Magazine_Price_Negative_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Price", "-5"));
        }

        [TestMethod]
        public void Magazine_Price_TooBig_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Price", "60000"));
        }

        [TestMethod]
        public void Magazine_Price_NotANumber_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Price", "abc"));
        }

        [TestMethod]
        public void Magazine_Price_DotSeparator_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Price", "150.50"));
        }

        [TestMethod]
        public void Magazine_Price_CommaSeparator_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Price", "150,50"));
        }

        [TestMethod]
        public void Magazine_Price_Max_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Price", "50000"));
        }

        [TestMethod]
        public void Magazine_IssueNumber_Empty_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("IssueNumber", ""));
        }

        [TestMethod]
        public void Magazine_IssueNumber_NotANumber_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("IssueNumber", "abc"));
        }

        [TestMethod]
        public void Magazine_IssueNumber_Zero_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("IssueNumber", "0"));
        }

        [TestMethod]
        public void Magazine_IssueNumber_Negative_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("IssueNumber", "-5"));
        }

        [TestMethod]
        public void Magazine_IssueNumber_Valid_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("IssueNumber", "12"));
        }

        [TestMethod]
        public void Magazine_Periodicity_Empty_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Periodicity", ""));
        }

        [TestMethod]
        public void Magazine_Periodicity_Whitespace_ReturnsError()
        {
            Assert.IsNotNull(_mag.ValidateField("Periodicity", "   "));
        }

        [TestMethod]
        public void Magazine_Periodicity_Valid_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("Periodicity", "Ежемесячно"));
        }

        [TestMethod]
        public void Magazine_UnknownProperty_ReturnsNull()
        {
            Assert.IsNull(_mag.ValidateField("UnknownProp", "value"));
        }
    }
}