using System.Collections.Generic;

namespace HololensPETS
{
    /**
     * ScrollingList is essentially a list that stores a
     * fixed number of elements such that once the list is full, it
     * starts to discard the oldest elements in the list.
     */
    public class ScrollingList<T>
    {
        private List<T> m_internalList;

        private int m_maxCapacity;
        public int MaxCapacity
        {
            get
            {
                return m_maxCapacity;
            }
        }

        public int Count
        {
            get
            {
                return m_internalList.Count;
            }
        }
        
        public ScrollingList() : this( 20 )
        {
        }

        public ScrollingList( int maxCapacity )
        {
            m_internalList = new List<T>();
            m_maxCapacity = maxCapacity;
        }

        public void Add( T element )
        {
            if( m_internalList.Count == m_maxCapacity )
            {
                m_internalList.RemoveAt(0);
            }

            m_internalList.Add(element);
        }

        public T GetElementAt( int index )
        {
            if( index < m_internalList.Count )
            {
                return m_internalList[index];
            }

            return default(T);
        }
    }
}
