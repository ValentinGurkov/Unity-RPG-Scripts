using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "New Cursor Type", menuName = "Enum/Cursor Type", order = 0)]
    public class CursorType : ScriptableObject
    {
        [SerializeField] private Texture2D texture;
        [SerializeField] private Vector2 hotspot;

        public Texture2D Texture => texture;
        public Vector2 Hotspot => hotspot;
    }
}