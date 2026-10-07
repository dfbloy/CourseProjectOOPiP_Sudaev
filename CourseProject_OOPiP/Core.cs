using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace CourseProject_OOPiP
{
    public class DataRepository<T> where T : class, ISearchable
    {
        private SortableBindingList<T> _items = new SortableBindingList<T>();
        private readonly string _filePath;
        private readonly XmlSerializer _serializer;

        public DataRepository(string filePath)
        {
            _filePath = filePath;
            _serializer = new XmlSerializer(typeof(List<T>), new Type[] { typeof(Book), typeof(Magazine) });
        }

        public BindingList<T> GetAll()
        {
            return _items;
        }

        public BindingList<T> Search(string text)
        {
            var filtered = _items.Where(item => item.ContainsText(text)).ToList();
            var result = new SortableBindingList<T>();
            foreach (var item in filtered)
            {
                result.Add(item);
            }
            return result;
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Remove(T item)
        {
            if (item != null && _items.Contains(item))
            {
                _items.Remove(item);
            }
        }

        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(_filePath))
            {
                _serializer.Serialize(writer, _items.ToList());
            }
        }

        public void Load()
        {
            if (!File.Exists(_filePath)) return;

            using (StreamReader reader = new StreamReader(_filePath))
            {
                var list = (List<T>)_serializer.Deserialize(reader);
                _items.Clear();
                foreach (var item in list)
                {
                    _items.Add(item);
                }
            }
        }

        public bool IsIsbnUnique(string isbn, Publication excludeItem = null)
        {
            if (string.IsNullOrWhiteSpace(isbn)) return true;

            string normalized = isbn.Trim();
            foreach (var item in _items)
            {
                if (item is Book book && !ReferenceEquals(book, excludeItem))
                {
                    if (string.Equals(book.Isbn?.Trim(), normalized, StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }
            return true;
        }

        public static bool IsEmpty(Publication item)
        {
            if (item == null) return true;

            if (item is Book book)
            {
                return string.IsNullOrWhiteSpace(book.Title)
                    && book.Year == null
                    && book.Price == null
                    && string.IsNullOrWhiteSpace(book.Author)
                    && string.IsNullOrWhiteSpace(book.Isbn);
            }

            if (item is Magazine mag)
            {
                return string.IsNullOrWhiteSpace(mag.Title)
                    && mag.Year == null
                    && mag.Price == null
                    && mag.IssueNumber == null
                    && string.IsNullOrWhiteSpace(mag.Periodicity);
            }

            return false;
        }
    }

    public class SortableBindingList<T> : BindingList<T>
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection;
        private PropertyDescriptor _sortProperty;

        protected override bool SupportsSortingCore => true;
        protected override bool IsSortedCore => _isSorted;
        protected override ListSortDirection SortDirectionCore => _sortDirection;
        protected override PropertyDescriptor SortPropertyCore => _sortProperty;

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortProperty = prop;
            _sortDirection = direction;

            var list = Items as List<T>;
            if (list == null) return;

            if (direction == ListSortDirection.Ascending)
            {
                list = list.OrderBy(x => prop.GetValue(x)).ToList();
            }
            else
            {
                list = list.OrderByDescending(x => prop.GetValue(x)).ToList();
            }

            Items.Clear();
            foreach (var item in list)
            {
                Items.Add(item);
            }

            _isSorted = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }
}
