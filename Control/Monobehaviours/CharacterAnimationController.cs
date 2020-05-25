using System;
using Core;
using Movement;
using UnityEngine;

namespace Control
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationController : MonoBehaviour
    {
        [SerializeField] private bool useSingleAxis = true;
        private Transform m_Transform;
        private Animator m_Animator;
        private CharacterMoverNavMesh m_Velocity;
        private IMoveInput m_MoveInput;
        private static readonly int s_SpeedAnimKey = Animator.StringToHash("Speed");
        private static readonly int s_HorizontalAnimKey = Animator.StringToHash("Horizontal");
        private static readonly int s_VerticalAnimKey = Animator.StringToHash("Vertical");
        private static readonly int s_Dashing = Animator.StringToHash("Dash");


        private void Awake()
        {
            m_Transform = transform;
            m_Animator = GetComponent<Animator>();
            m_Velocity = GetComponent<CharacterMoverNavMesh>();
            m_MoveInput = GetComponent<IMoveInput>();
        }


        private void Update()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            if (useSingleAxis)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(m_Velocity.Velocity);
                float speed = Math.Abs(localVelocity.z);
                m_Animator.SetFloat(s_SpeedAnimKey, speed);
            }
            else
            {
                float verticalDot = Vector3.Dot(m_Transform.forward, m_MoveInput.MoveDirection);
                float horizontalDot = Vector3.Dot(m_Transform.right, m_MoveInput.MoveDirection);
                m_Animator.SetFloat(s_HorizontalAnimKey, horizontalDot);
                m_Animator.SetFloat(s_VerticalAnimKey, verticalDot);
            }
        }

        public void Dash()
        {
            m_Animator.SetTrigger(s_Dashing);
        }
    }
}