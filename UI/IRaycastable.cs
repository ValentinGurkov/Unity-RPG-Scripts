using UnityEngine;

namespace RPG.UI {
    public interface IRaycastable {
        bool HandleRaycast(GameObject callingObject);

        CursorType Cursor {
            get;
        }
    }
}
