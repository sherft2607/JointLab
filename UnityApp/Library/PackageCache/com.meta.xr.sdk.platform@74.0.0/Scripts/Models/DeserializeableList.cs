namespace Oculus.Platform.Models
{
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Oculus.Platform.Models;
    /// DeserializableList is designed to be used with the Oculus Platform SDK and provides a way to deserialize JSON data into a list of objects.
    /// It provides a range of methods for adding, removing, and accessing elements in the list, as well as properties for checking if there are more pages of data available.
    public class DeserializableList<T> : IList<T>
    {
        //IList
        ///  The number of elements contained within this list. This is not equal to the total number of elements across multiple pages.
        public int Count
        {
            get { return _Data.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return ((IList<T>)_Data).IsReadOnly; }
        } //if you insist in getting it...
        /// This method returns the index of the specified object in the list, or -1 if it is not found.
        public int IndexOf(T obj)
        {
            return _Data.IndexOf(obj);
        }

        ///  Access the indexed element in this list.
        ///  \param index The index of the element to return.
        public T this[int index]
        {
            get { return _Data[index]; }
            set { _Data[index] = value; }
        }
        /// This method adds an item to the end of the list, increasing its size by one.
        public void Add(T item)
        {
            _Data.Add(item);
        }
        /// This method removes all items from the list, leaving it empty and with a count of zero.
        public void Clear()
        {
            _Data.Clear();
        }
        /// This method determines whether the list contains a specific value, returning true if it is found and false otherwise.
        public bool Contains(T item)
        {
            return _Data.Contains(item);
        }
        /// This method copies the elements of the list to an array, starting at a specified array index, allowing for efficient transfer of data.
        public void CopyTo(T[] array, int arrayIndex)
        {
            _Data.CopyTo(array, arrayIndex);
        }
        /// This method returns an enumerator that iterates through the list, providing a way to access each element in sequence.
        /// Taken from examples [here](https://msdn.microsoft.com/en-us/library/s793z9y2(v=vs.110).aspx)
        public IEnumerator<T> GetEnumerator()
        {
            return _Data.GetEnumerator();
        }
        /// This method inserts an item into the list at a specified index, shifting existing elements to make room.
        public void Insert(int index, T item)
        {
            _Data.Insert(index, item);
        }
        /// This method removes the first occurrence of a specific object from the list, decreasing its size by one.
        public bool Remove(T item)
        {
            return _Data.Remove(item);
        }
        /// This method removes the item at the specified index from the list, shifting existing elements to fill the gap.
        public void RemoveAt(int index)
        {
            _Data.RemoveAt(index);
        }

        // taken from examples here: https://msdn.microsoft.com/en-us/library/s793z9y2(v=vs.110).aspx
        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }
        /// returns an enumerator that iterates through the list, providing a way to access each element in sequence.

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

        // Internals and getters

        // Seems like Obsolete properties are broken in this version of Mono.
        // Anyway, don't use this.
        [System.Obsolete("Use IList interface on the DeserializableList object instead.", false)]
        public List<T> Data
        {
            get { return _Data; }
        }
        /// This is a protected field that stores the actual list of objects. It is used by the DeserializableList class to store the data that is deserialized from JSON.
        protected List<T> _Data;
        /// This is a protected field that store the URLs for the next pages of data. It allows you to retrieve additional data beyond the initial page.
        protected string _NextUrl;
        /// This is a protected field that store the URLs for the previous pages of data. It allows you to retrieve additional data beyond the initial page.
        protected string _PreviousUrl;

        /// This property returns a value indicating whether there is a next page of elements that can be retrieved, allowing for pagination of large datasets.
        public bool HasNextPage
        {
            get { return !System.String.IsNullOrEmpty(NextUrl); }
        }

        /// This property returns a value indicating whether there is a previous page of elements that can be retrieved, allowing for navigation through paginated data.
        public bool HasPreviousPage
        {
            get { return !System.String.IsNullOrEmpty(PreviousUrl); }
        }

        ///  This property returns the URL to request the next paginated list of elements, providing a way to retrieve additional data.
        public string NextUrl
        {
            get { return _NextUrl; }
        }

        ///  This property returns the URL to request the previous paginated list of elements, providing a way to navigate back through paginated data.
        public string PreviousUrl
        {
            get { return _PreviousUrl; }
        }
    }
}
