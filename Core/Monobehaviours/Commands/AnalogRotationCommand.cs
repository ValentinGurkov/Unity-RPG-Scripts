using System.Collections;
using UnityEngine;

namespace Core
{
    public class AnalogRotationCommand : Command
    {
        public float turnSmoothing;

        private IRotationInput m_Input;
        private Rigidbody m_Rigidbody;
        private Coroutine m_Coroutine;

        private void Awake()
        {
            m_Input = GetComponent<IRotationInput>();
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        public override void Execute()
        {
            if (m_Coroutine == null) m_Coroutine = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            var time = 0.0f;
            while (m_Input.RotationDirection != Vector3.zero)
            {
                if (m_Input.RotationDirection.magnitude <= 0.5f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(m_Input.RotationDirection, Vector3.up);
                    time += Time.fixedDeltaTime * turnSmoothing * m_Input.RotationDirection.magnitude;
                    Quaternion newRotation = Quaternion.Lerp(m_Rigidbody.rotation, targetRotation, time);

                    m_Rigidbody.MoveRotation(newRotation);
                }
                else
                {
                    m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_Input.RotationDirection, Vector3.up));
                }

                yield return null;
            }

            m_Coroutine = null;
        }
    }
}