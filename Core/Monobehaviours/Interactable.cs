using UnityEngine;
using Util;

namespace Core
{
    [DisallowMultipleComponent]
    public abstract class Interactable : MonoBehaviour, IRaycastable
    {
        [SerializeField] protected GameManager gameManager;

        public virtual CursorType Cursor => gameManager.Enums.CursorTypes[Constants.CursorTypes.None];
        public virtual string Type => Constants.CursorTypes.None;

        public virtual bool HandleRaycast(GameObject callingObject)
        {
            return true;
        }
    }
}