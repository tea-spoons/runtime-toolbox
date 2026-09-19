
namespace TeaSpoons.RuntimeToolbox
{
    using System;

    /// <summary>
    /// Base class for different kinds of events.
    /// </summary>
    public abstract class SmartEvent
    {
        /// <summary>
        /// A trigger object that is returned from a <see cref="SmartEvent"/> constructor.
        /// </summary>
        /// <remarks>
        /// Used to seperate AddResponse logic from invocation logic to allow for making the latter <c>private</c>.
        /// </remarks>
        public readonly struct Trigger
        {
            private readonly SmartEvent target;

            internal Trigger(SmartEvent target)
            {
                this.target = target;
            }

            /// <summary>
            /// Invokes the associated event.
            /// </summary>
            public void Invoke()
            {
                target.Invoke();
            }
        }

        /// <param name="trigger">The trigger to use to invoke this event.</param>
        public SmartEvent(out Trigger trigger)
        {
            trigger = new Trigger(this);
        }

        public abstract void AddResponse(Action response);

        public abstract void RemoveResponse(Action response);

        protected abstract void Invoke();
    }
}
