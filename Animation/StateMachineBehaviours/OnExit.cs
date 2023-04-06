using Events;
using UnityEngine;

namespace Animation.StateMachineBehaviours
{
    public class OnExit : StateMachineBehaviour
    {
        [SerializeField] private GameEvent onAnimationExit;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (onAnimationExit != null)
            {
                onAnimationExit.Raise();
            }
        }
    }
}