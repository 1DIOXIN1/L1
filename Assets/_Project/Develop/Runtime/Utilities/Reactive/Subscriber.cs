using System;

namespace _Project.Develop.Runtime.Utilities.Reactive
{
    public class Subscriber<T, K> : IDisposable
    {
        private Action<Subscriber<T, K>> _onDispose;
        private Action<T, K> _action;
        private IDisposable _disposableImplementation;

        public Subscriber(Action<T, K> action,  Action<Subscriber<T, K>> onDispose)
        {
            _action = action;
            _onDispose = onDispose;
        }
        
        public void Invoke(T arg, K arg2) => _action?.Invoke(arg, arg2);
        public void Dispose() => _onDispose?.Invoke(this);
    }
}