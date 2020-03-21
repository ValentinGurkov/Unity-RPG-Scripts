using System.Collections;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        private CanvasGroup m_CanvasGroup;
        private Coroutine m_CurrentActiveFade;

        private void Awake()
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(m_CanvasGroup.alpha, target))
            {
                m_CanvasGroup.alpha = Mathf.MoveTowards(m_CanvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }

        public void FadeOutImmediate()
        {
            m_CanvasGroup.alpha = 1;
        }

        public Coroutine FadeOut(float time)
        {
            return Fade(1, time);
        }

        public Coroutine FadeIn(float time)
        {
            return Fade(0, time);
        }

        private Coroutine Fade(float target, float time)
        {
            if (m_CurrentActiveFade != null)
            {
                StopCoroutine(m_CurrentActiveFade);
            }

            m_CurrentActiveFade = StartCoroutine(FadeRoutine(target, time));
            return m_CurrentActiveFade;
        }
    }
}