using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GameManager", menuName = "Game Manager", order = 1)]
    public class GameManager : ScriptableObject
    {
        public static Dictionary<string, ScriptableObject> CursorTypes { get; } =
            new Dictionary<string, ScriptableObject>();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Init()
        {
            FillDictionaryEnum<CursorType>(CursorTypes, "Cursor Types");
        }

        private static void FillDictionaryEnum<T>(IDictionary<string, ScriptableObject> dictionary, string folder)
            where T : ScriptableObject
        {
            T[] types = Resources.LoadAll<T>(folder);

            foreach (T type in types)
            {
                dictionary.Add(type.name, type);
            }
        }
    }
}