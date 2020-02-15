using System.IO;
using UnityEngine;

namespace RPG.Questing {
    public static class QuestSaver {

        public static void Save(Quest quest) {
            string json = JsonUtility.ToJson(quest);
            File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + quest.ID + ".json", json);
        }

        public static Quest Load(string questID) {
            Quest quest;
            if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + questID + ".json")) {
                quest = ScriptableObject.CreateInstance<Quest>();
                string json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + questID + ".json");
                JsonUtility.FromJsonOverwrite(json, quest);
                return quest;
            }
            quest = Resources.Load<Quest>(questID);

            return GameObject.Instantiate(quest);
        }
    }

}
