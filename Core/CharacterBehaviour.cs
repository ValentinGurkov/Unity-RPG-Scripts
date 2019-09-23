using UnityEngine;

namespace RPG.Core {

    public class CharacterBehaviour : MonoBehaviour {
        [SerializeField] private float rotationSpeed = 5;

        public void LookAtTarget(Transform targetTransform) {
            Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void AttackedBy(GameObject target) {
            LookAtTarget(target.transform);
        }
    }
}
