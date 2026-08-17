using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class lungePromptFader : MonoBehaviour
{
    [SerializeField] TMP_Text promptText;
    [SerializeField] float fadeDuration = 0.2f;

    CanvasGroup canvasGroup;
    Coroutine fadeRoutine;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void Show(string message)
    {
        if (promptText != null)
        {
            promptText.text = message;
        }

        StartFade(1f);
    }

    public void Hide()
    {
        StartFade(0f);
    }

    void StartFade(float targetAlpha)
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeTo(targetAlpha));
    }

    IEnumerator FadeTo(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        canvasGroup.blocksRaycasts = targetAlpha > 0f;
        fadeRoutine = null;
    }
}
