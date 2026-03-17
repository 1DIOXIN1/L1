using System;
using System.Collections;

namespace _Project.Develop.Runtime.Utilities.DataManagement.DataProviders
{
    public interface IDataProvider
    {
        IEnumerator Load();
        IEnumerator Save();
        IEnumerator Exists(Action<bool> onExistsResult);
        void Reset();
    }
}