using _Project.Develop.Runtime.Gameplay.Infrastructure;
using UnityEngine;

namespace _Project.Develop.Runtime.Gameplay.Main
{
    public class GameplaySequenceGeneratorService
    {
        public string Generate(int lenght, GameplayType gameplayType)
        {
            string result = "";
            
            switch (gameplayType)
            {
                case GameplayType.Numbers:
                {
                    for (int i = 0; i < lenght; i++)
                        result += Random.Range(0, 10);

                    break;
                }

                case GameplayType.Words:
                {
                    for (int i = 0; i < lenght; i++)
                        result += (char)Random.Range('A', 'Z' + 1);

                    break;
                }
            }
            Debug.Log(result);
            return result;
        }
    }
}