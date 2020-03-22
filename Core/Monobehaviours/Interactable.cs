using UnityEngine;
using Util;

namespace Core
{
    [DisallowMultipleComponent]
    public abstract class Interactable : MonoBehaviour, IRaycastable
    {
        public virtual CursorType Cursor => GameManager.CursorTypes[Constants.CursorTypes.None] as CursorType;

        public virtual bool HandleRaycast(GameObject callingObject)
        {
            return true;
        }
    }
}