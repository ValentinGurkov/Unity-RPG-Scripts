using UnityEngine;

namespace Control
{
    /// <summary>
    /// Used for calculating patrol path of NPCs and visualizing it with gizmos in the editor.
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        private const float WaypointRadius = 0.3f;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            GameObject active = UnityEditor.Selection.activeGameObject;
            if (active != null)
            {
                var AI = active.GetComponent<AIController>();
                if (AI != null && AI.PatrolPath == this)
                {
                    Gizmos.color = Color.yellow;
                }

                if (active == gameObject)
                {
                    Gizmos.color = new Color(0.4f, 1, 0.5f);
                }
            }

            for (var i = 0; i < transform.childCount; i++)
            {
                Gizmos.DrawSphere(GetWayPoint(i).position, WaypointRadius);
                Gizmos.DrawLine(GetWayPoint(i).position, GetWayPoint(i + 1).position);
            }
        }
#endif

        public int GetNextIndex(int i)
        {
            return (i + 1) % transform.childCount;
        }

        public Transform GetWayPoint(int i)
        {
            if (i >= transform.childCount)
            {
                i = 0;
            }

            return transform.GetChild(i);
        }
    }
}