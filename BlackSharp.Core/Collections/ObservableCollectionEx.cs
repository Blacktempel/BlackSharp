/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) 2025 Florian K.
 *
 */

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace BlackSharp.Core.Collections
{
    /// <summary>
    /// A <see cref="ObservableCollectionEx{T}"/> based on the Microsoft implementation of <see cref="ObservableCollection{T}"/>.
    /// <para/>
    /// This class also provides functionality which the normal <see cref="ObservableCollection{T}"/> does not (Find, Sort, ...).
    /// </summary>
    public class ObservableCollectionEx<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        #region Constructor

        /// <summary>
        /// Initializes the collection.
        /// </summary>
        public ObservableCollectionEx()
            : base()
        {
        }

        /// <summary>
        /// Initializes the collection with a specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements the collection can initially store.</param>
        public ObservableCollectionEx(int capacity)
            : base(new List<T>(capacity))
        {
        }

        /// <summary>
        /// Initializes the collection with the items of given <see cref="List{T}"/>.
        /// The elements are copied from the <see cref="List{T}"/> by enumerating it.
        /// </summary>
        /// <param name="list"><see cref="List{T}"/> whose elements should be copied.</param>
        public ObservableCollectionEx(List<T> list)
            : base(list != null ? new List<T>(list.Count) : list)
        {
            foreach (var item in list)
                Items.Add(item);
        }

        /// <summary>
        /// Initializes the collection with the items of given <see cref="IEnumerable{T}"/>.
        /// The elements are copied from the <see cref="IEnumerable{T}"/> by enumerating it.
        /// </summary>
        /// <param name="collection"><see cref="IEnumerable{T}"/> whose elements should be copied.</param>
        public ObservableCollectionEx(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
            foreach (var item in collection)
                Items.Add(item);
        }

        /// <summary>
        /// Initializes the collection with the items of given parameter objects.
        /// </summary>
        /// <param name="param">Elements for initialization.</param>
        public ObservableCollectionEx(params T[] param)
            : this((IEnumerable<T>)param)
        {
        }

        #endregion

        #region Fields

        /// <summary>
        /// Specifies invalid index. This is used by e.g. <see cref="FindIndex"/>.
        /// </summary>
        public const int InvalidIndex = -1;

        const string CountString = "Count";
        const string IndexerName = "Item[]";
        SimpleMonitor Monitor = new SimpleMonitor();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the value if the <see cref="CollectionChanged"/> and
        /// <see cref="PropertyChanged"/> events are fired on changes.
        /// Default is true.
        /// </summary>
        public bool AreNotifyEventsEnabled { get; set; } = true;

        #endregion

        #region Public

        /// <summary>
        /// Checks if the <see cref="ObservableCollectionEx{T}"/> is empty.
        /// </summary>
        /// <returns>Whether the collection is empty or not.</returns>
        public bool IsEmpty()
        {
            return Count == 0;
        }

        /// <summary>
        /// Determines whether an element is in the <see cref="ObservableCollectionEx{T}"/> by specified <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the element.</param>
        /// <returns>Whether an element in the <see cref="ObservableCollectionEx{T}"/> matches the specified conditions.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public bool Contains(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            foreach (var item in this)
                if (match(item))
                    return true;
            return false;
        }

        /// <summary>
        /// Performs the specified <see cref="Action{T}"/> on each element of <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action{T}"/> to perform on each element.</param>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public void ForEach(Action<T> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            foreach (var item in this)
                action.Invoke(item);
        }

        /// <summary>
        /// Determines whether all elements in <see cref="ObservableCollectionEx{T}"/>
        /// match the conditions of the specified <see cref="Predicate{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the element.</param>
        /// <returns>True if all elements in <see cref="ObservableCollectionEx{T}"/> match the conditions
        /// of the specified <see cref="Predicate{T}"/> or if the collection has no elements.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
                throw new ArgumentNullException(nameof(match));
            foreach (var item in this)
                if (!match(item))
                    return false;
            return true;
        }

        /// <summary>
        /// Move item at oldIndex to newIndex.
        /// </summary>
        /// <param name="oldIndex">The old index of the item.</param>
        /// <param name="newIndex">The new index of the item.</param>
        public void Move(int oldIndex, int newIndex)
        {
            MoveItem(oldIndex, newIndex);
        }

        /// <summary>
        /// Adds a range of elements to <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="items">Elements to add.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        public void AddRange(params T[] items)
        {
            AddRange((IEnumerable<T>)items);
        }

        /// <summary>
        /// Adds a range of elements to <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="items">Range of elements to add.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        public void AddRange(IEnumerable<T> items)
        {
            CheckReentrancy();
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            bool prev = AreNotifyEventsEnabled;
            AreNotifyEventsEnabled = false;

            foreach (var item in items)
                Add(item);

            AreNotifyEventsEnabled = prev;

            OnCollectionReset();
        }

        /// <summary>
        /// Adds a range of elements to <see cref="ObservableCollectionEx{T}"/>.
        /// Each given item will be checked for given <see cref="Predicate{T}"/>,
        /// and will only be added if it matches the condition.
        /// </summary>
        /// <param name="items">Range of elements to add.</param>
        /// <param name="condition">Condition an item must satisfy to be added.</param>
        /// <exception cref="ArgumentNullException">Parameter is null.</exception>
        public void AddRange(IEnumerable<T> items, Predicate<T> condition)
        {
            CheckReentrancy();
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            if (condition == null)
                throw new ArgumentNullException(nameof(condition));

            bool prev = AreNotifyEventsEnabled;
            AreNotifyEventsEnabled = false;

            foreach (var item in items)
                if (condition(item))
                    Add(item);

            AreNotifyEventsEnabled = prev;

            OnCollectionReset();
        }

        /// <summary>
        /// Removes a range of elements from <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index is invalid or count is less than 0.</exception>
        /// <exception cref="ArgumentException">Index and count do not denote a valid
        /// range of elements in <see cref="ObservableCollectionEx{T}"/>.</exception>
        public void RemoveRange(int index, int count)
        {
            CheckReentrancy();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (Count - index < count)
                throw new ArgumentException("Invalid offset");

            List<T> removed = new List<T>();

            bool prev = AreNotifyEventsEnabled;
            AreNotifyEventsEnabled = false;

            for (int i = index + count - 1, j = 0; j < count; --i, ++j)
            {
                var item = this[i];
                removed.Add(item);
                RemoveItem(i);
            }

            removed.Reverse();

            AreNotifyEventsEnabled = prev;

            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removed);
        }

        /// <summary>
        /// Searches through all items if the item matches the conditions of the specified <see cref="Predicate{T}"/>
        /// and removes it if so.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the elements to remove.</param>
        public void Remove(Predicate<T> match)
        {
            for (int i = 0; i < Count; ++i)
                if (match(this[i]))
                    Remove(this[i]);
            OnCollectionReset();
        }

        /// <summary>
        /// Searches for an element that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// and returns the first match within the <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the element to search for.</param>
        /// <returns>The first item that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// or the default value for type T if nothing was found.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public T Find(Predicate<T> match)
        {
            foreach (var item in this)
                if (match(item))
                    return item;
            return default(T);
        }

        /// <summary>
        /// Searches for an element that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// and returns the last match within the <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the element to search for.</param>
        /// <returns>The last item that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// or the default value for type T if nothing was found.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public T FindLast(Predicate<T> match)
        {
            for (int i = Count - 1; i >= 0; --i)
                if (match(this[i]))
                    return this[i];
            return default(T);
        }

        /// <summary>
        /// Searches for an element that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// and returns all matches within the <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the elements to search for.</param>
        /// <returns>All items that match the conditions of the specified <see cref="Predicate{T}"/>
        /// or an empty <see cref="List{T}"/> if nothing was found.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public List<T> FindAll(Predicate<T> match)
        {
            List<T> matches = new List<T>();
            foreach (var item in this)
                if (match(item))
                    matches.Add(item);
            return matches;
        }

        /// <summary>
        /// Searches for an element that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// and returns the first matches' index within the <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the elements to search for.</param>
        /// <returns>The first matches' index or <see cref="InvalidIndex"/> if nothing was found.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public int FindIndex(Predicate<T> match)
        {
            var count = Count;
            for (int i = 0; i < count; ++i)
                if (match(this[i]))
                    return i;
            return InvalidIndex;
        }

        /// <summary>
        /// Searches for an element that matches the conditions of the specified <see cref="Predicate{T}"/>
        /// and returns the last matches' index within the <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <param name="match">The <see cref="Predicate{T}"/> that defines the conditions of the elements to search for.</param>
        /// <returns>The last matches' index or <see cref="InvalidIndex"/> if nothing was found.</returns>
        /// <exception cref="ArgumentNullException">If the parameter is null.</exception>
        public int FindLastIndex(Predicate<T> match)
        {
            var count = Count;
            for (int i = count - 1; i >= 0; --i)
                if (match(this[i]))
                    return i;
            return InvalidIndex;
        }

        /// <summary>
        /// Returns the first element of <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <returns>The first element in <see cref="ObservableCollectionEx{T}"/>.</returns>
        public T First()
        {
            return this[0];
        }

        /// <summary>
        /// Returns the last element of <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        /// <returns>The last element in <see cref="ObservableCollectionEx{T}"/>.</returns>
        public T Last()
        {
            return this[Count - 1];
        }

        /// <summary>
        /// Sorts the elements in the <see cref="ObservableCollectionEx{T}"/> using the default comparer.
        /// </summary>
        public void Sort()
        {
            Sort(0, Count, null);
        }

        /// <summary>
        /// Sorts the elements in the <see cref="ObservableCollectionEx{T}"/>
        /// using the specified <see cref="Comparison{T}"/>.
        /// </summary>
        /// <param name="comparisation">The <see cref="Comparison{T}"/> to use when sorting the elements.</param>
        public void Sort(Comparison<T> comparisation)
        {
            Sort(0, Count, new FunctorComparer<T>(comparisation));
        }

        /// <summary>
        /// Sorts the elements in the <see cref="ObservableCollectionEx{T}"/>
        /// using the specified <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use when sorting the elements.</param>
        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        /// <summary>
        /// Sorts the elements in a range of elements in <see cref="ObservableCollectionEx{T}"/>
        /// using the specified <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range to sort.</param>
        /// <param name="count">The length of the range to sort.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use when comparing the elements or
        /// null to use the default comparer.</param>
        /// <exception cref="ArgumentOutOfRangeException">Index is invalid or count is less than 0.</exception>
        /// <exception cref="ArgumentException">Index and count do not denote a valid
        /// range of elements in <see cref="ObservableCollectionEx{T}"/>.</exception>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            CheckReentrancy();
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));
            if (Count - index < count)
                throw new ArgumentException("Invalid offset");
            ArrayList.Adapter((IList)Items).Sort(index, count, (IComparer)comparer);
            OnCollectionReset();
        }

        /// <summary>
        /// Reverses the order of the elements in the entire <see cref="ObservableCollectionEx{T}"/>.
        /// </summary>
        public void Reverse()
        {
            CheckReentrancy();
            ArrayList.Adapter((IList)Items).Reverse();
            OnCollectionReset();
        }

        /// <summary>
        /// Attempts to insert an item at the specified index in the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted.<br/>
        /// Must be greater than or equal to 0 and less than or equal to the current number of items.</param>
        /// <param name="item">The item to insert into the collection.</param>
        /// <returns>true if the item was successfully inserted; otherwise, false.</returns>
        /// <remarks>This will fail if given index is not in a valid range.</remarks>
        public bool TryInsert(int index, T item)
        {
            if (index < 0 || index > Count)
                return false;

            InsertItem(index, item);

            return true;
        }

        #endregion

        #region Protected

        /// <summary>
        /// Disallow reentrant attempts to change this collection. E.g. a event handler
        /// of the CollectionChanged event is not allowed to make changes to this collection.
        /// </summary>
        /// <returns></returns>
        protected IDisposable BlockReentrancy()
        {
            Monitor.Enter();
            return Monitor;
        }

        /// <summary>
        /// Check and assert for reentrant attempts to change this collection.
        /// </summary>
        /// <exception cref="InvalidOperationException">Raised when changing the collection
        /// while another collection change is still being notified to other listeners.</exception>
        protected void CheckReentrancy()
        {
            if (Monitor.Busy)
            {
                //We can allow changes if there's only one listener - the problem
                //only arises if reentrant changes make the original event args
                //invalid for later listeners. This keeps existing code working
                //(e.g. Selector.SelectedItems).
                if ((CollectionChanged != null) && (CollectionChanged.GetInvocationList().Length > 1))
                    throw new InvalidOperationException("Reentrancy not allowed.");
            }
        }

        /// <summary>
        /// Clears all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            CheckReentrancy();
            base.ClearItems();
            NotifyPropertyChanged(CountString);
            NotifyPropertyChanged(IndexerName);
            OnCollectionReset();
        }

        /// <summary>
        /// Removes an item by index from the collection.
        /// </summary>
        /// <param name="index">Index of item to remove.</param>
        protected override void RemoveItem(int index)
        {
            CheckReentrancy();
            T removedItem = this[index];

            base.RemoveItem(index);

            NotifyPropertyChanged(CountString);
            NotifyPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Remove, removedItem, index);
        }

        /// <summary>
        /// Inserts an item by index into the collection.
        /// </summary>
        /// <param name="index">Index where to add the new item.</param>
        /// <param name="item">Item to add.</param>
        protected override void InsertItem(int index, T item)
        {
            CheckReentrancy();
            base.InsertItem(index, item);

            NotifyPropertyChanged(CountString);
            NotifyPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
        }

        /// <summary>
        /// Called by base class <see cref="Collection{T}"/> when an item is set in list.
        /// Raises a <see cref="CollectionChanged"/> event to any listeners.
        /// </summary>
        /// <param name="index">Index of changing item.</param>
        /// <param name="item">Changing item.</param>
        protected override void SetItem(int index, T item)
        {
            CheckReentrancy();
            T originalItem = this[index];
            base.SetItem(index, item);

            NotifyPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        /// <summary>
        /// Called by <see cref="ObservableCollectionEx{T}"/> when an item is to be moved within the list.
        /// Raises a <see cref="CollectionChanged"/> event to any listeners.
        /// </summary>
        /// <param name="oldIndex">Old index of moving item.</param>
        /// <param name="newIndex">New index of moving item.</param>
        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            T removedItem = this[oldIndex];

            base.RemoveItem(oldIndex);
            base.InsertItem(newIndex, removedItem);

            NotifyPropertyChanged(IndexerName);
            OnCollectionChanged(NotifyCollectionChangedAction.Move, removedItem, newIndex, oldIndex);
        }

        #endregion

        #region Private

        #region SimpleMonitor

        /// <summary>
        /// Monitor class.
        /// </summary>
        class SimpleMonitor : IDisposable
        {
            public void Enter()
            {
                ++_BusyCount;
            }

            public void Dispose()
            {
                --_BusyCount;
            }

            public bool Busy { get { return _BusyCount > 0; } }

            int _BusyCount;
        }

        #endregion

        #endregion

        #region Events

        #region INotifyCollectionChanged

        /// <summary>
        /// Occurs when the collection changes, either by adding or removing an item.
        /// </summary>
        public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Raise CollectionChanged event to any listeners.<br/>
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <param name="e">The event.</param>
        /// <remarks>When overriding this method, either call its base implementation
        /// or call to guard against reentrant collection changes.</remarks>
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null && AreNotifyEventsEnabled)
            {
                using (BlockReentrancy())
                {
                    CollectionChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to
        /// <see cref="NotifyCollectionChangedAction.Reset"/>,
        /// <see cref="NotifyCollectionChangedAction.Add"/>,
        /// <see cref="NotifyCollectionChangedAction.Remove"/>.</param>
        /// <param name="changedItems">The items that are affected by the change.</param>
        void OnCollectionChanged(NotifyCollectionChangedAction action, IList changedItems)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, changedItems));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to
        /// <see cref="NotifyCollectionChangedAction.Reset"/>,
        /// <see cref="NotifyCollectionChangedAction.Add"/>,
        /// <see cref="NotifyCollectionChangedAction.Remove"/>.</param>
        /// <param name="item">The item that is affected by the change.</param>
        /// <param name="index">The index where the change occurred.</param>
        void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This can only be set to
        /// <see cref="NotifyCollectionChangedAction.Move"/>.</param>
        /// <param name="item">The item affected by the change.</param>
        /// <param name="index">The new index for the changed item.</param>
        /// <param name="oldIndex">The old index for the changed item.</param>
        void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action that caused the event. This can be set to
        /// <see cref="NotifyCollectionChangedAction.Replace"/>.</param>
        /// <param name="oldItem">The original item that is replaced.</param>
        /// <param name="newItem">The new item that is replacing the original item.</param>
        /// <param name="index">The index of the item being replaced.</param>
        void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyCollectionChangedEventArgs"/> class
        /// that describes a <see cref="NotifyCollectionChangedAction.Reset"/> change.
        /// </summary>
        void OnCollectionReset()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// Property changed event.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies that a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of property which has changed.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (AreNotifyEventsEnabled)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #endregion
    }

    #region FunctorComparer

    /// <summary>
    /// Comparisation helper class.
    /// </summary>
    public class FunctorComparer<T> : IComparer<T>, IComparer
    {
        #region Constructor
        /// <summary>
        /// Constructs a new object.
        /// </summary>
        /// <param name="comparisation">Comparison parameter.</param>
        /// <exception cref="ArgumentNullException">Throws if parameter is null.</exception>
        public FunctorComparer(Comparison<T> comparisation)
        {
            if (comparisation == null)
                throw new ArgumentNullException(nameof(comparisation));
            _Comp = comparisation;
        }
        #endregion

        #region Fields

        Comparison<T> _Comp;

        #endregion

        #region Compare

        /// <summary>
        /// Compare two objects of given type.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Whether objects are equal or not.</returns>
        public int Compare(T x, T y)
        {
            return _Comp(x, y);
        }

        /// <summary>
        /// Compare two objects.
        /// </summary>
        /// <param name="x">First object to compare.</param>
        /// <param name="y">Second object to compare.</param>
        /// <returns>Whether objects are equal or not.</returns>
        public int Compare(object x, object y)
        {
            return _Comp((T)x, (T)y);
        }

        #endregion
    }

    #endregion
}
