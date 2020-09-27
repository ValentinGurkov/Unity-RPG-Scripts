using System.Collections;
using UnityEngine;

namespace Core
{
    public class CharacterBehaviour : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed = 5f;
        private Coroutine _lookRoutine;

        private IEnumerator LookRoutine(Transform targetTransform)
        {
            //is it worth to make a check if targets are not facing each other already or it wont really bring any performance benefit?
            Quaternion targetRotation = Quaternion.LookRotation(targetTransform.position - transform.position);
            Quaternion startRotation = transform.rotation;
            float t = 0;
            while (t < 1f)
            {
                t += rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }

            transform.rotation = targetRotation;
            _lookRoutine = null;
        }

        public void LookAtTarget(Transform targetTransform)
        {
            if (_lookRoutine == null) StartCoroutine(LookRoutine(targetTransform));
        }
    }
}