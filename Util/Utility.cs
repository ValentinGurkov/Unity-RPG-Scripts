using UnityEngine;

namespace RPG.Util {

    public static class Utility {
        /// <summary>
        /// More performant way of calculating distance than using Vector3.Distance().
        /// </summary>
        /// <param name="me"></param>First object of interest.
        /// <param name="target"></param>Second object of interest.
        /// <param name="distance"></param>Distance between objects to check.
        /// <returns>Whether the target is in range or not.</returns>
        public static bool IsTargetInRange(Transform me, Transform target, float distance) {
            return (me.position - target.position).sqrMagnitude < distance * distance;
        }
    }
}
