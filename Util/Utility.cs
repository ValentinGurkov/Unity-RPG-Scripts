using UnityEngine;

namespace RPG.Util
{
    public static class Utility
    {
        /// <summary>
        /// More performant way of calculating distance than using Vector3.Distance().
        /// </summary>
        /// <param name="me">First object of interest.</param>
        /// <param name="target">Second object of interest.</param>
        /// <param name="distance">Distance between objects to check.</param>
        /// <returns>Whether the target is in range or not.</returns>
        public static bool IsTargetInRange(Transform me, Transform target, float distance)
        {
            return (me.position - target.position).sqrMagnitude < distance * distance;
        }
    }
}