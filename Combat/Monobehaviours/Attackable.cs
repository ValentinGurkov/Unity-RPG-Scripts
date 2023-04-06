using Attributes;
using Core;
using UnityEngine;
using Util;

namespace Combat
{
    [RequireComponent(typeof(HealthNew))]
    public class Attackable : Interactable
    {
        public override CursorType Cursor => gameManager.Enums.CursorTypes[Constants.CursorTypes.Combat];
        public override string Type => Constants.CursorTypes.Combat;

        public override bool HandleRaycast(GameObject callingObject)
        {
            var fighter = callingObject.GetComponent<FighterNew>();
            return fighter.CanAttack(gameObject);
        }
    }
}