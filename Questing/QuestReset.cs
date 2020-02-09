using RPG.Util;
using UnityEngine;

namespace RPG.Questing {
    public class QuestReset : MonoBehaviour {
        [Tooltip("Used to debug quests when we do not need saving.")]
        [SerializeField] private bool resetQuests;
        private void OnDestroy() {
            if (resetQuests) {
                var quests = Resources.FindObjectsOfTypeAll<QuestS>();
                for (int i = 0; i < quests.Length; i++) {
                    var handler = quests[i] as IUnloadedSceneHandler;
                    if (handler != null) {
                        handler.OnSceneUnloaded();
                    }
                }
            }
        }
    }

}
