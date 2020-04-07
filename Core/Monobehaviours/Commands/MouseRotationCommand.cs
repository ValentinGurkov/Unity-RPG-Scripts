using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core
{
    public class MouseRotationCommand : Command
    {
        public float turnSmoothing;

        private IRotationInput m_Rotate;
        private IInteractInput m_Interact;
        private Rigidbody m_Rigidbody;
        private Coroutine m_Coroutine;

        private void Awake()
        {
            m_Rotate = GetComponent<IRotationInput>();
            m_Interact = GetComponent<IInteractInput>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }


        public override void Execute()
        {
            if (m_Coroutine == null) m_Coroutine = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            var time = 0.0f;
            while (m_Interact.IsPressingInteract)
            {
                Vector3 screenSize = new Vector3(Screen.width, 0, Screen.height) / 2;
                var mousePosition = new Vector3(Mouse.current.position.ReadValue().x, 0,
                    Mouse.current.position.ReadValue().y);

                m_Rotate.RotationDirection = (mousePosition - screenSize).normalized;

                if (turnSmoothing > 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(m_Rotate.RotationDirection, Vector3.up);
                    time += Time.fixedDeltaTime * turnSmoothing * m_Rotate.RotationDirection.magnitude;
                    Quaternion newRotation = Quaternion.Lerp(m_Rigidbody.rotation, targetRotation, time);

                    m_Rigidbody.MoveRotation(newRotation);
                }
                else
                {
                    m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_Rotate.RotationDirection, Vector3.up));
                }

                yield return null;
            }

            m_Coroutine = null;
        }
    }
}