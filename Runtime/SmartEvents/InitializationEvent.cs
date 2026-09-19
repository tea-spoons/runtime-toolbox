
namespace TeaSpoons.RuntimeToolbox
{
    using System;

    /// <summary>
    /// An event that remembers having been invoked once.<br/>
    /// Responses that are added via <see cref="AddResponse"/> after the invocation will be invoked immediately in addition to being registered.
    /// </summary>
    /// <remarks>
    /// This type is meant to be used for events that are called when something was initialized.<br/>
    /// Future event subscribers can use it to immediately rely on the thing having been initialized if the event had been invoked before.
    /// </remarks>
    public sealed class InitializationEvent : SmartEvent
    {
        public bool WasInvoked => wasInvoked;

        private event Action evt = delegate { };
        private bool wasInvoked = false;

        /// <param name="trigger">The trigger to use to invoke this event.</param>
        public InitializationEvent(out Trigger trigger) : base(out trigger)
        {
        }

        public override void AddResponse(Action response)
        {
            if (wasInvoked)
            {
                response();
            }

            evt += response;
        }

        public override void RemoveResponse(Action response)
        {
            evt -= response;
        }

        protected override void Invoke()
        {
            evt();
            wasInvoked = true;
        }
    }
}
