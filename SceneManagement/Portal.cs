using System.Collections;
using RPG.Control;
using RPG.Core;
using RPG.Questing;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private int sceneToLoad = -1;
        [SerializeField] private float fadeOutTime = 1f;
        [SerializeField] private float fadeInTime = .5f;
        [SerializeField] private float fadeWaitTime = .5f;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private DestinationIdentifier destination = DestinationIdentifier.A;

        private enum DestinationIdentifier
        {
            A,
            B,
            C,
            D,
            E
        }

        public Transform SpawnPoint => spawnPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition(other.GetComponent<PlayerController>()));
            }
        }

        private IEnumerator Transition(PlayerController playerController)
        {
            if (sceneToLoad < 0)
            {
                Debug.LogError("Scene to load is not set!");
                yield break;
            }

            playerController.enabled = false;
            playerController.SetCursor(CursorType.None);
            DontDestroyOnLoad(gameObject);

            var fader = FindObjectOfType<Fader>();
            var savingWrapper = FindObjectOfType<SavingWrapper>();
            yield return fader.FadeOut(fadeOutTime);

            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            var newPlayerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            newPlayerController.enabled = false;
            newPlayerController.SetCursor(CursorType.None);

            savingWrapper.Load();

            Portal destinationPortal = GetDestinationPortal();
            UpdatePlayer(destinationPortal);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);
            newPlayerController.enabled = true;
            newPlayerController.SetCursor(CursorType.Movement);
            Destroy(gameObject);
        }

        private Portal GetDestinationPortal()
        {
            foreach (Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this || portal.destination != destination)
                {
                    continue;
                }

                return portal;
            }

            return null;
        }

        private static void UpdatePlayer(Portal destinationPortal)
        {
            if (!destinationPortal) return;
            GameObject player = GameObject.FindWithTag("Player");
            var navMeshAgent = player.GetComponent<NavMeshAgent>();
            navMeshAgent.enabled = false;
            player.transform.position = destinationPortal.SpawnPoint.position;
            player.transform.rotation = destinationPortal.SpawnPoint.rotation;
            navMeshAgent.enabled = true;
        }
    }
}