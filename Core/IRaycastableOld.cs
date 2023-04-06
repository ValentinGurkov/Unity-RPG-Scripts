using UnityEngine;

namespace RPG.Core
{
    public interface IRaycastableOld
    {
        bool HandleRaycast(GameObject callingObject);

        CursorType Cursor { get; }
    }
}