﻿using System.Collections;
using RPG.Saving;
using UnityEngine;

namespace RPG.SceneManagement
{
    /// <summary>
    /// Leverages the core saving functionality to match the game's designed way of saving.
    /// </summary>
    public class SavingWrapper : MonoBehaviour
    {
        [SerializeField] private float fadeOutTime = 0.5f;
        private const string defaultSaveFile = "save";
        private SavingSystem savingSystem;
        private Fader fader;

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
            savingSystem = GetComponent<SavingSystem>();
            yield return savingSystem.LoadLastScene(defaultSaveFile);
            fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeOutTime);
        }

        public void Save()
        {
            savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            savingSystem.Load(defaultSaveFile);
        }

        public void Delete()
        {
            SavingSystem.Delete(defaultSaveFile);
        }
    }
}