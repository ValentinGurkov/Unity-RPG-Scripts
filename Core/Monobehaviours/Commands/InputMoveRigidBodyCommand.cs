using System.Collections;
using UnityEngine;

namespace Core
{
    public class InputMoveRigidBodyCommand : Command
    {
        [SerializeField] private AnimationCurve speed;
        [SerializeField] private float speedModifier = 1;
        [SerializeField] private float turnSmoothing;

        private Rigidbody m_Rigidbody;
        private IMoveInput m_Move;
        private IRotationInput m_Rotate;
        private Coroutine m_MoveCoroutine;
        private Coroutine m_RotateCoroutine;
        private Transform m_Transform;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Move = GetComponent<IMoveInput>();
            m_Rotate = GetComponent<IRotationInput>();
            m_Transform = transform;
        }

        public override void Execute()
        {
            if (m_MoveCoroutine == null) m_MoveCoroutine = StartCoroutine(Move());
            if (m_RotateCoroutine == null) m_RotateCoroutine = StartCoroutine(Rotate());
        }

        private IEnumerator Move()
        {
            while (m_Move.MoveDirection != Vector3.zero)
            {
                float time = Time.fixedDeltaTime * speed.Evaluate(m_Move.MoveDirection.magnitude) * speedModifier;

                m_Rigidbody.MovePosition(m_Transform.position + m_Move.MoveDirection * time);
                yield return null;
            }

            m_MoveCoroutine = null;
        }

        private IEnumerator Rotate()
        {
            var time = 0.0f;

            while (m_Move.MoveDirection != Vector3.zero)
            {
                yield return new WaitUntil(() => m_Rotate.RotationDirection == Vector3.zero);

                if (m_Move.MoveDirection == Vector3.zero) continue;

                if (m_Move.MoveDirection.magnitude <= 0.5f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(m_Move.MoveDirection, Vector3.up);
                    time += Time.fixedDeltaTime * turnSmoothing * m_Move.MoveDirection.magnitude;
                    Quaternion newRotation = Quaternion.Lerp(m_Rigidbody.rotation, targetRotation, time);
                    m_Rigidbody.MoveRotation(newRotation);
                }
                else
                {
                    m_Rigidbody.MoveRotation(Quaternion.LookRotation(m_Move.MoveDirection, Vector3.up));
                }
            }

            m_RotateCoroutine = null;
        }
    }
}