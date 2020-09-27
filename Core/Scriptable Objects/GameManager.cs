using Sirenix.OdinInspector;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "GameManager", menuName = "Manager/Game Manager", order = 1)]
    public class GameManager : SerializedScriptableObject
    {
        [SerializeField] private Enums enums;

        public Enums Enums => enums;
    }
}