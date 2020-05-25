using Movement;
using UnityEngine;

namespace Core
{
    public class DashCommand : Command
    {
        private IMouseInput m_MouseInput;
        private CharacterMoverNavMesh m_Mover;
        private Camera m_Camera;

        private void Awake()
        {
            m_MouseInput = GetComponent<IMouseInput>();
            m_Mover = GetComponent<CharacterMoverNavMesh>();
            m_Camera = Camera.main;
        }

        public override void Execute()
        {
            Ray ray = GetMouseRay();
            if (Physics.Raycast(ray, out RaycastHit lookDirectionHit, 50f, LayerMask.GetMask("Ground")))
            {
                m_Mover.Dash(lookDirectionHit.point);
            }
        }

        private Ray GetMouseRay()
        {
            return m_Camera.ScreenPointToRay(m_MouseInput.MousePosition);
        }
    }
}