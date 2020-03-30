namespace HololensPETS
{
    public class ObservableValue<T>
    {
        public delegate void ObservedValueChangedDelegate(T oldValue, T newValue);
        public ObservedValueChangedDelegate OnObservedValueChanged;

        private T m_value;
        public T Value
        {
            set
            {
                Set(value, true);
            }

            get
            {
                return m_value;
            }
        }

        public ObservableValue() : this( default( T ) )
        {
        }

        public ObservableValue( T val )
        {
            m_value = val;
        }

        public void Set( T newValue, bool notify = true )
        {
            T oldValue = m_value;
            m_value = newValue;

            if (notify && VerifyValueHasChanged(oldValue, newValue))
            {
                if (OnObservedValueChanged != null)
                {
                    OnObservedValueChanged(oldValue, newValue);
                }
            }
        }

        protected bool VerifyValueHasChanged(T oldValue, T newValue)
        {
            return !oldValue.Equals(newValue);
        }
    }
}
