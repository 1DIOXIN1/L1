using System;

namespace _Project.Develop.Runtime.Utilities.InputManagement
{
    public interface IGameplayBlocker
    {
        IDisposable Block();
    }
}