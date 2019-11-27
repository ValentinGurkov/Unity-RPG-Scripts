using UnityEngine;

namespace RPG.Core {
    public interface IRaycastable {
        bool HandleRaycast(GameObject callingObject);

        CursorType Cursor {
            get;
        }
    }
}
