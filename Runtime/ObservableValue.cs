
namespace TeaSpoons.RuntimeToolbox
{
    using System;

    /// <summary>
    /// A wrapper for a value of type <typeparamref name="T"/> that has an event that is called after the value is updated.
    /// </summary>
    public class ObservableValue<T>
    {
        /// <summary>
        /// A value update trigger object that is returned from an <see cref="ObservableValue{T}"/> constructor.
        /// </summary>
        /// <remarks>
        /// This version separates AddUpdatedResponse logic from value update logic to allow for making the latter <c>private</c>.
        /// </remarks>
        public readonly struct UpdateTrigger
        {
            private readonly ObservableValue<T> target;

            internal UpdateTrigger(ObservableValue<T> target)
            {
                this.target = target;
            }

            public void SetValue(T value)
            {
                target.SetValue(value);
            }
        }

        private T value;
        private event Action<T> updated = delegate { };

        public T Value => value;

        public ObservableValue(T initialValue, out UpdateTrigger updateTrigger)
        {
            value = initialValue;
            updateTrigger = new UpdateTrigger(this);
        }

        public ObservableValue(out UpdateTrigger updateTrigger) : this(default, out updateTrigger)
        {
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
