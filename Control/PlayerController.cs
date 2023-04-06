using System;
using RPG.Attributes;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [System.Serializable]
        public struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private CursorMapping[] mappings;
        [SerializeField] private float navMeshMaxProjectionDistance = 1f;
        [SerializeField] private float raycastRadius = 1f;

        private Mover m_Mover;
        private Health m_PlayerHealth;
        private Camera m_MainCamera;

        private void Awake()
        {
            m_Mover = GetComponent<Mover>();
            m_PlayerHealth = GetComponent<Health>();
            m_MainCamera = Camera.main;
        }

        private void Update()
        {
            /*  if (Input.GetKey("escape"))
              {
                  Application.Quit();
                  #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                  #endif
              }

              if (InteractWithUI())
              {
                  return;
              }

              if (m_PlayerHealth.IsDead)
              {
                  SetCursor(CursorType.None);
                  return;
              }

              if (InteractWithComponent())
              {
                  return;
              }

              if (InteractWithMovement())
              {
                  return;
              }*/

            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (!EventSystem.current.IsPointerOverGameObject()) return false;
            SetCursor(CursorType.UI);
            return true;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastableOld[] raycastables = hit.transform.GetComponents<IRaycastableOld>();
                foreach (IRaycastableOld raycastable in raycastables)
                {
                    if (!raycastable.HandleRaycast(gameObject)) continue;
                    SetCursor(raycastable.Cursor);
                    return true;
                }
            }

            return false;
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            var distances = new float[hits.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = hits[i].distance;
            }

            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement()
        {
            bool hasHit = RaycastNavMesh(out Vector3 target);

            if (!hasHit) return false;
            if (!m_Mover.CanMoveTo(target)) return false;

            /*  if (Input.GetMouseButton(0))
              {
                  m_Mover.StartMovement(target, 1f);
              }
            */
            SetCursor(CursorType.Movement);
            return true;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);
            if (!hasHit)
            {
                return false;
            }

            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit,
                navMeshMaxProjectionDistance,
                NavMesh.AllAreas);
            if (!hasCastToNavMesh)
            {
                return false;
            }

            target = navMeshHit.position;

            return true;
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            for (int i = 0; i < mappings.Length; i++)
            {
                if (mappings[i].type == type)
                {
                    return mappings[i];
                }
            }

            return mappings[0];
        }

        private Ray GetMouseRay()
        {
            return new Ray();
            //return m_MainCamera.ScreenPointToRay(Input.mousePosition);
        }

        public void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }
    }
}