using System.Collections;
using Saving;
using UnityEngine;

namespace SceneManagement
{
    /// <summary>
    /// Leverages the core saving functionality to match the game's designed way of saving.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeOutTime = 0.5f;
        private const string DefaultSaveFile = "save";
        private SavingSystem _savingSystem;
        private Fader _fader;

        private void Awake()
        {
            StartCoroutine(LoadLastScene());
        }

        private void Update()
        {
            /* if (Input.GetKeyDown(KeyCode.S)) {
                 Save();
             }
             if (Input.GetKeyDown(KeyCode.L)) {
                 Load();
             }
             if (Input.GetKeyDown(KeyCode.Delete)) {
                 Delete();
             }*/
        }

        private IEnumerator LoadLastScene()
        {
            _savingSystem = GetComponent<SavingSystem>();
            yield return _savingSystem.LoadLastScene(DefaultSaveFile);
            //_fader = FindObjectOfType<Fader>();
            //_fader.FadeOutImmediate();
            //yield return _fader.FadeIn(fadeOutTime);
        }

        public void Save()
        {
            _savingSystem.Save(DefaultSaveFile);
        }

        public void Load()
        {
            _savingSystem.Load(DefaultSaveFile);
        }

        public void Delete()
        {
            _savingSystem.Delete(DefaultSaveFile);
        }
    }
}