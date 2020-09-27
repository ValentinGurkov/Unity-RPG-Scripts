using Core;
using Util;

namespace NPC
{
    public class QuestGiver : NPC
    {
        public override CursorType Cursor => gameManager.Enums.CursorTypes[Constants.CursorTypes.QuestGiver];
        public override string Type => Constants.CursorTypes.QuestGiver;
    }
}