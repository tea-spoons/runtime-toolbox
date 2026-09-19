
namespace TeaSpoons.RuntimeToolbox
{
    using System;

    /// <summary>
    /// A wrapper for a value of type <typeparamref name="T"/> that has an event that is called after the value is updated.
    /// </summary>
    /// <remarks>
    /// This version has a public setter for the <see cref="Value"/> property.
    /// </remarks>
    public class PublicObservableValue<T>
    {
        private T value;
        private event Action<T> updated = delegate { };

        public T Value
        {
            get => value;
            set => SetValue(value);
        }

        public PublicObservableValue(T initialValue = default)
        {
            value = initialValue;
        }

        public void AddUpdatedResponse(Action<T> response)
        {
            updated += response;
        }

        public void AddAndInvokeUpdatedResponse(Action<T> response)
        {
            updated += response;
            response(value);
        }

        public void RemoveUpdatedResponse(Action<T> response)
        {
            updated -= response;
        }

        private void SetValue(T value)
        {
            if (Equals(value, this.value)) return;

            this.value = value;
            updated.Invoke(value);
        }
    }
}
