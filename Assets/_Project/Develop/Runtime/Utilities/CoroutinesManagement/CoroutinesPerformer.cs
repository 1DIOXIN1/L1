using System.Collections;
using UnityEngine;

namespace _Project.Develop.Runtime.Utilities
{
    public class CoroutinesPerformer : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    
        public Coroutine StartPerform(IEnumerator coroutineFunction) =>  StartCoroutine(coroutineFunction);
        public void StopPerform(Coroutine coroutine) =>  StopCoroutine(coroutine);
    }
}
