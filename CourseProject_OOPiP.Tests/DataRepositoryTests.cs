using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourseProject_OOPiP;

namespace CourseProject_OOPiP.Tests
{
    [TestClass]
    public class DataRepositoryTests
    {
        private string _tempFile;
        private DataRepository<Publication> _repo;

        [TestInitialize]
        public void Setup()
        {
            _tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".xml");
            _repo = new DataRepository<Publication>(_tempFile);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile))
                File.Delete(_tempFile);
        }

        [TestMethod]
        public void Add_ThenGetAll_ContainsItem()
        {
            var book = new Book { Title = "Test", Year = 2000, Price = 100, Author = "A", Isbn = "1" };
            _repo.Add(book);

            Assert.AreEqual(1, _repo.GetAll().Count);
            Assert.AreSame(book, _repo.GetAll()[0]);
        }

        [TestMethod]
        public void Add_Multiple_KeepsOrder()
        {
            var b1 = new Book { Title = "1" };
            var b2 = new Book { Title = "2" };
            _repo.Add(b1);
            _repo.Add(b2);

            Assert.AreEqual("1", _repo.GetAll()[0].Title);
            Assert.AreEqual("2", _repo.GetAll()[1].Title);
        }

        [TestMethod]
        public void Remove_ExistingItem_RemovesIt()
        {
            var book = new Book { Title = "Test" };
            _repo.Add(book);
            _repo.Remove(book);

            Assert.AreEqual(0, _repo.GetAll().Count);
        }

        [TestMethod]
        public void Remove_NullItem_DoesNotThrow()
        {
            _repo.Remove(null);
            Assert.AreEqual(0, _repo.GetAll().Count);
        }

        [TestMethod]
        public void Remove_NotContainedItem_DoesNotThrow()
        {
            var b1 = new Book { Title = "1" };
            var b2 = new Book { Title = "2" };
            _repo.Add(b1);
            _repo.Remove(b2);

            Assert.AreEqual(1, _repo.GetAll().Count);
        }

        [TestMethod]
        public void Search_MatchesTitle_ReturnsFiltered()
        {
            _repo.Add(new Book { Title = "Анна Каренина", Year = 1877, Price = 100, Author = "Толстой", Isbn = "1" });
            _repo.Add(new Book { Title = "Война и мир", Year = 1869, Price = 100, Author = "Толстой", Isbn = "2" });

            var res = _repo.Search("Анна");
            Assert.AreEqual(1, res.Count);
            Assert.AreEqual("Анна Каренина", res[0].Title);
        }

        [TestMethod]
        public void Search_EmptyQuery_ReturnsAll()
        {
            _repo.Add(new Book { Title = "A" });
            _repo.Add(new Book { Title = "B" });

            var res = _repo.Search("");
            Assert.AreEqual(2, res.Count);
        }

        [TestMethod]
        public void Search_NoMatches_ReturnsEmpty()
        {
            _repo.Add(new Book { Title = "A" });
            var res = _repo.Search("zzz");
            Assert.AreEqual(0, res.Count);
        }

        [TestMethod]
        public void Search_DoesNotModifyOriginal()
        {
            _repo.Add(new Book { Title = "A" });
            _repo.Add(new Book { Title = "B" });

            _repo.Search("A");

            Assert.AreEqual(2, _repo.GetAll().Count);
        }

        [TestMethod]
        public void IsIsbnUnique_NoBooks_ReturnsTrue()
        {
            Assert.IsTrue(_repo.IsIsbnUnique("111-222"));
        }

        [TestMethod]
        public void IsIsbnUnique_Duplicate_ReturnsFalse()
        {
            _repo.Add(new Book { Isbn = "111-222" });
            Assert.IsFalse(_repo.IsIsbnUnique("111-222"));
        }

        [TestMethod]
        public void IsIsbnUnique_CaseInsensitive_ReturnsFalse()
        {
            _repo.Add(new Book { Isbn = "ABC" });
            Assert.IsFalse(_repo.IsIsbnUnique("abc"));
        }

        [TestMethod]
        public void IsIsbnUnique_WithWhitespace_ReturnsFalse()
        {
            _repo.Add(new Book { Isbn = "111-222" });
            Assert.IsFalse(_repo.IsIsbnUnique("  111-222  "));
        }

        [TestMethod]
        public void IsIsbnUnique_WithExcludeItem_ReturnsTrue()
        {
            var book = new Book { Isbn = "111-222" };
            _repo.Add(book);

            Assert.IsTrue(_repo.IsIsbnUnique("111-222", book));
        }

        [TestMethod]
        public void IsIsbnUnique_WithOtherExcludeItem_ReturnsFalse()
        {
            var book1 = new Book { Isbn = "111-222" };
            var book2 = new Book { Isbn = "333-444" };
            _repo.Add(book1);
            _repo.Add(book2);

            Assert.IsFalse(_repo.IsIsbnUnique("111-222", book2));
        }

        [TestMethod]
        public void IsIsbnUnique_Empty_ReturnsTrue()
        {
            Assert.IsTrue(_repo.IsIsbnUnique(""));
        }

        [TestMethod]
        public void IsIsbnUnique_Null_ReturnsTrue()
        {
            Assert.IsTrue(_repo.IsIsbnUnique(null));
        }

        [TestMethod]
        public void IsIsbnUnique_MagazineIsbn_Ignored()
        {
            _repo.Add(new Magazine { Isbn = "111-222" });
            Assert.IsTrue(_repo.IsIsbnUnique("111-222"));
        }

        [TestMethod]
        public void Save_ThenLoad_RestoresBooks()
        {
            _repo.Add(new Book { Title = "Книга1", Year = 2000, Price = 500, Author = "Автор", Isbn = "1" });
            _repo.Save();

            var repo2 = new DataRepository<Publication>(_tempFile);
            repo2.Load();

            Assert.AreEqual(1, repo2.GetAll().Count);
            Assert.AreEqual("Книга1", repo2.GetAll()[0].Title);
        }

        [TestMethod]
        public void Save_ThenLoad_RestoresMagazines()
        {
            _repo.Add(new Magazine { Title = "Журнал1", Year = 2010, Price = 100, IssueNumber = 5, Periodicity = "Месяц" });
            _repo.Save();

            var repo2 = new DataRepository<Publication>(_tempFile);
            repo2.Load();

            Assert.AreEqual(1, repo2.GetAll().Count);
            Assert.IsInstanceOfType(repo2.GetAll()[0], typeof(Magazine));
            Assert.AreEqual(5, ((Magazine)repo2.GetAll()[0]).IssueNumber);
        }

        [TestMethod]
        public void Save_ThenLoad_MixedTypes()
        {
            _repo.Add(new Book { Title = "Книга", Year = 2000, Price = 500, Author = "A", Isbn = "1" });
            _repo.Add(new Magazine { Title = "Журнал", Year = 2010, Price = 100, IssueNumber = 5, Periodicity = "Месяц" });
            _repo.Save();

            var repo2 = new DataRepository<Publication>(_tempFile);
            repo2.Load();

            Assert.AreEqual(2, repo2.GetAll().Count);
            Assert.IsInstanceOfType(repo2.GetAll()[0], typeof(Book));
            Assert.IsInstanceOfType(repo2.GetAll()[1], typeof(Magazine));
        }

        [TestMethod]
        public void Load_FileDoesNotExist_DoesNotThrow()
        {
            var repo = new DataRepository<Publication>("no_such_file_xyz.xml");
            repo.Load();
            Assert.AreEqual(0, repo.GetAll().Count);
        }

        [TestMethod]
        public void IsEmpty_Null_ReturnsTrue()
        {
            Assert.IsTrue(DataRepository<Publication>.IsEmpty(null));
        }

        [TestMethod]
        public void IsEmpty_EmptyBook_ReturnsTrue()
        {
            Assert.IsTrue(DataRepository<Publication>.IsEmpty(new Book()));
        }

        [TestMethod]
        public void IsEmpty_BookWithTitle_ReturnsFalse()
        {
            Assert.IsFalse(DataRepository<Publication>.IsEmpty(new Book { Title = "X" }));
        }

        [TestMethod]
        public void IsEmpty_BookWithYearOnly_ReturnsFalse()
        {
            Assert.IsFalse(DataRepository<Publication>.IsEmpty(new Book { Year = 2000 }));
        }

        [TestMethod]
        public void IsEmpty_EmptyMagazine_ReturnsTrue()
        {
            Assert.IsTrue(DataRepository<Publication>.IsEmpty(new Magazine()));
        }

        [TestMethod]
        public void IsEmpty_MagazineWithIssue_ReturnsFalse()
        {
            Assert.IsFalse(DataRepository<Publication>.IsEmpty(new Magazine { IssueNumber = 1 }));
        }

        [TestMethod]
        public void IsEmpty_MagazineWithPeriodicity_ReturnsFalse()
        {
            Assert.IsFalse(DataRepository<Publication>.IsEmpty(new Magazine { Periodicity = "Месяц" }));
        }
    }
}