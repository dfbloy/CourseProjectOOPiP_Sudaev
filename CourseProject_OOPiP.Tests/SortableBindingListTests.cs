using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourseProject_OOPiP;

namespace CourseProject_OOPiP.Tests
{
    [TestClass]
    public class SortableBindingListTests
    {
        private SortableBindingList<Book> _list;

        [TestInitialize]
        public void Setup()
        {
            _list = new SortableBindingList<Book>
            {
                new Book { Title = "B", Year = 2000 },
                new Book { Title = "A", Year = 1990 },
                new Book { Title = "C", Year = 2010 }
            };
        }

        [TestMethod]
        public void SupportsSorting_IsTrue()
        {
            Assert.IsTrue(((IBindingList)_list).SupportsSorting);
        }

        [TestMethod]
        public void Initially_NotSorted()
        {
            Assert.IsFalse(((IBindingList)_list).IsSorted);
        }

        [TestMethod]
        public void ApplySort_ByTitleAscending()
        {
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Title"];
            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Ascending);

            Assert.AreEqual("A", _list[0].Title);
            Assert.AreEqual("B", _list[1].Title);
            Assert.AreEqual("C", _list[2].Title);
            Assert.IsTrue(((IBindingList)_list).IsSorted);
        }

        [TestMethod]
        public void ApplySort_ByTitleDescending()
        {
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Title"];
            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Descending);

            Assert.AreEqual("C", _list[0].Title);
            Assert.AreEqual("B", _list[1].Title);
            Assert.AreEqual("A", _list[2].Title);
        }

        [TestMethod]
        public void ApplySort_ByYearAscending()
        {
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Year"];
            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Ascending);

            Assert.AreEqual(1990, _list[0].Year);
            Assert.AreEqual(2000, _list[1].Year);
            Assert.AreEqual(2010, _list[2].Year);
        }

        [TestMethod]
        public void ApplySort_ByYearDescending()
        {
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Year"];
            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Descending);

            Assert.AreEqual(2010, _list[0].Year);
            Assert.AreEqual(2000, _list[1].Year);
            Assert.AreEqual(1990, _list[2].Year);
        }

        [TestMethod]
        public void RemoveSort_ResetsIsSorted()
        {
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Title"];
            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Ascending);
            ((IBindingList)_list).RemoveSort();

            Assert.IsFalse(((IBindingList)_list).IsSorted);
        }

        [TestMethod]
        public void ApplySort_RaisesListChanged()
        {
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Title"];
            bool raised = false;

            _list.ListChanged += (s, e) =>
            {
                if (e.ListChangedType == ListChangedType.Reset) raised = true;
            };

            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Ascending);

            Assert.IsTrue(raised);
        }

        [TestMethod]
        public void ApplySort_CountUnchanged()
        {
            int before = _list.Count;
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Title"];
            ((IBindingList)_list).ApplySort(prop, ListSortDirection.Ascending);

            Assert.AreEqual(before, _list.Count);
        }

        [TestMethod]
        public void ApplySort_EmptyList_DoesNotThrow()
        {
            var empty = new SortableBindingList<Book>();
            var prop = TypeDescriptor.GetProperties(typeof(Book))["Title"];

            ((IBindingList)empty).ApplySort(prop, ListSortDirection.Ascending);

            Assert.AreEqual(0, empty.Count);
        }
    }
}