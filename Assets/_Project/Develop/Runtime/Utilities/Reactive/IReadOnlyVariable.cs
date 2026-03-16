using System;

namespace _Project.Develop.Runtime.Utilities.Reactive
{
    public interface IReadOnlyVariable<T>
    {
        public IDisposable Subscribe(Action<T, T> action);
        public T Value{ get;}
    }


}