using UnityEngine;

namespace RPG.Core {

    public class FollowCamera : MonoBehaviour {
        [SerializeField] private Transform target;
        private Vector3 difference;

        // Start is called before the first frame update
        private void Start() {
            difference = transform.position - target.position;
        }

        // Update is called once per frame
        private void LateUpdate() {
            transform.position = target.position + difference;
        }
    }
}
