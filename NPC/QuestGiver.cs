using Core;
using Util;

namespace NPC
{
    public class QuestGiver : NPC
    {
        public override CursorType Cursor => GameManager.CursorTypes[Constants.CursorTypes.QuestGiver] as CursorType;
    }
}