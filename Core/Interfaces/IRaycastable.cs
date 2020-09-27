using UnityEngine;

namespace Core
{
    public interface IRaycastable
    {
        CursorType Cursor { get; }
        string Type { get; }

        bool HandleRaycast(GameObject callingObject);
    }
}