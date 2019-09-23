using UnityEngine;

namespace RPG.Utility {

    public static class Utility {

        public static bool IsTargetInRange(Transform me, Transform target, float distance) {
            return (me.position - target.position).sqrMagnitude < distance * distance;
        }
    }
}
