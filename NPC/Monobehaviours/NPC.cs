using Core;
using Util;

namespace NPC
{
    public class NPC : Interactable
    {
        public override CursorType Cursor => gameManager.Enums.CursorTypes[Constants.CursorTypes.NPC];
        public override string Type => Constants.CursorTypes.NPC;
    }
}