using System;
using Movement;
using UnityEngine;

namespace Control
{
    [RequireComponent(typeof(Animator))]
    public class CharacterAnimationController : MonoBehaviour
    {
        private Animator m_Animator;
        private CharacterMover m_Mover;
        private static readonly int s_SpeedAnimKey = Animator.StringToHash("Speed");

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_Mover = GetComponent<CharacterMover>();
        }


        private void Update()
        {
            UpdateAnimator();
        }

        private void UpdateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(m_Mover.Velocity);
            float speed = Math.Abs(localVelocity.z);
            m_Animator.SetFloat(s_SpeedAnimKey, speed);
        }
    }
}