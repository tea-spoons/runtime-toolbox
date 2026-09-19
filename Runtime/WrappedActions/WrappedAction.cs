namespace TeaSpoons.RuntimeToolbox
{
    using System;

    /// <summary>
    /// Just a little wrapper for an Action, allowing to add and invoke at the same time
    /// </summary>
    public class WrappedAction
    {
        private Action action = delegate { };

        public void Invoke()
        {
            action?.Invoke();
        }

        public void AddResponse(Action response)
        {
            action += response;
        }
        public void AddAndInvokeResponse(Action response)
        {
            action += response;
            response();
        }

        public void RemoveResponse(Action response)
        {
            action -= response;
        }
    }
}
