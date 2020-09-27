using Events;
using UnityEngine;
using RPG.Events;

namespace Animation.Behaviours
{
    public class OnExit : StateMachineBehaviour
    {
        [SerializeField] private GameEvent onAnimationExit;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            onAnimationExit?.Raise();
        }
    }
}