using Core;
using Util;

namespace NPC
{
    public class NPC : Interactable
    {
        public override CursorType Cursor => GameManager.CursorTypes[Constants.CursorTypes.NPC] as CursorType;
    }
}