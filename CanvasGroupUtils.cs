using System.Collections;
using System.Collections.Generic;
using GoodbyeReality.Tweening;
using UnityEngine;

namespace GoodbyeReality.Utility
{
    public class CanvasGroupUtils : Singleton<CanvasGroupUtils>
    {
        private Dictionary<CanvasGroup, Coroutine> _levelSetters = new();
        private Easer _easerAlpha;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// </summary>
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(this);

            _easerAlpha = new(Easings.Function.Linear, 1f);
        }

        public void SetAlpha(CanvasGroup canvasGroup, float targetAlpha)
        {
            canvasGroup.alpha = Mathf.Abs(targetAlpha);
            SetCanvasGroupBlockRaycast(canvasGroup, targetAlpha > 0.01f);
        }

        public void SetAlphaOverTime(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            if (_levelSetters.ContainsKey(canvasGroup))
            {
                if (_levelSetters[canvasGroup] != null)
                {
                    StopCoroutine(_levelSetters[canvasGroup]);
                }

                _levelSetters[canvasGroup] = StartCoroutine(LerpAlphaOverTime(canvasGroup, targetAlpha, duration));
            }
            else
            {
                _levelSetters.Add(canvasGroup, StartCoroutine(LerpAlphaOverTime(canvasGroup, targetAlpha, duration)));
            }
        }

        private IEnumerator LerpAlphaOverTime(CanvasGroup canvasGroup, float targetAlpha, float duration)
        {
            _easerAlpha.Duration = duration;

            _easerAlpha.Reset();
            while (_easerAlpha.InterpolatedValue < 1f)
            {
                _easerAlpha.Update(Time.unscaledDeltaTime);
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, _easerAlpha.InterpolatedValue);
                yield return null;
            }

            canvasGroup.alpha = Mathf.Abs(canvasGroup.alpha);
            SetCanvasGroupBlockRaycast(canvasGroup, canvasGroup.alpha > 0.01f);

            _levelSetters.Remove(canvasGroup);
        }

        public void SetCanvasGroupBlockRaycast(CanvasGroup canvasGroup, bool value)
        {
            canvasGroup.blocksRaycasts = value;
        }

        public void SetCanvasGroupInteractable(CanvasGroup canvasGroup, bool value)
        {
            canvasGroup.interactable = value;
        }
    }
}