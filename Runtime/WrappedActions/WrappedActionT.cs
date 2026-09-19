namespace TeaSpoons.RuntimeToolbox
{
    using System;

    /// <summary>
    /// Just a little wrapper for an Action with input type T, allowing to add and invoke at the same time.
    /// Also allows subscription with a parameterless callback.
    /// </summary>
    public class WrappedAction<T>
    {
        private Action<T> action = delegate { };
        private Action actionNoParam = delegate { };

        public void Invoke(T value)
        {
            action?.Invoke(value);
            actionNoParam?.Invoke();
        }

        public void AddResponse(Action<T> response)
        {
            action += response;
        }

        public void AddResponseNoParam(Action response)
        {
            actionNoParam += response;
        }

        public void AddAndInvokeResponse(Action<T> response, T responseParam)
        {
            action += response;
            response?.Invoke(responseParam);
        }

        public void AddAndInvokeResponseNoParam(Action response)
        {
            actionNoParam += response;
            response?.Invoke();
        }

        public void RemoveResponse(Action<T> response)
        {
            action -= response;
        }

        public void RemoveResponseNoParam(Action response)
        {
            actionNoParam -= response;
        }
    }
}
