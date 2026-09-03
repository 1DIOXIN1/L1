using System;

namespace _Project.Develop.Runtime.Utilities.InputManagement
{
    public sealed class GameplayBlocker : IGameplayBlocker
    {
        private readonly IInputService _input;

        public GameplayBlocker(IInputService input)
        {
            _input = input;
        }

        public IDisposable Block()
        {
            InputContext previous = _input.CurrentContext;
            _input.SetContext(InputContext.Cutscene);
            return new ContextRestore(_input, previous);
        }

        private sealed class ContextRestore : IDisposable
        {
            private readonly IInputService _input;
            private readonly InputContext _previous;
            private bool _disposed;

            public ContextRestore(IInputService input, InputContext previous)
            {
                _input = input;
                _previous = previous;
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;
                _input.SetContext(_previous);
            }
        }
    }
}
