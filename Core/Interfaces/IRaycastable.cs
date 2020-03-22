using UnityEngine;

namespace Core
{
    public interface IRaycastable
    {
        CursorType Cursor { get; }

        bool HandleRaycast(GameObject callingObject);
    }
}