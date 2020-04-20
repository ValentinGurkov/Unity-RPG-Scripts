using Core;
using UnityEngine;
using Util;

namespace Combat
{
    public class Attackable : Interactable
    {
        public override CursorType Cursor => GameManager.CursorTypes[Constants.CursorTypes.Combat] as CursorType;

        public override bool HandleRaycast(GameObject callingObject)
        {
            return true;
        }
    }
}