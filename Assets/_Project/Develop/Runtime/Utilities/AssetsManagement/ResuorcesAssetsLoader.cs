using UnityEngine;

namespace _Project.Develop.Runtime.Utilities.AssetsManagement
{
    public class ResuorcesAssetsLoader
    {
        public T Load<T>(string resourcePath) where T : Object 
            => Resources.Load<T>(resourcePath);
    }
}
