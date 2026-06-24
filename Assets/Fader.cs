using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class Fader : MonoBehaviour
{
    [SerializeField] float fadeInDuration = 1.5f;
    [SerializeField] float fadeOutDuration = 1f;

    CanvasGroup canvasGroup;
    RectTransform rectTransform;
    Coroutine fadeRoutine;

    void Awake()
    {
        // Cache the CanvasGroup once so we can animate its alpha efficiently.
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        // Force the overlay to stretch across the whole canvas instead of
        // staying pinned to one corner from a bad prefab setup.
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

    }
    public float FadeOutDuration
    {
        get { return fadeOutDuration; }
    }

    public void FadeIn()
    {
        // The overlay starts fully visible, then fades out on scene load.
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = false;

        // FadeIn means "show the game", so alpha goes to 0.
        StartFade(0f, fadeInDuration);
    }

    public void FadeOut()
    {
        // FadeOut means "cover the game", so alpha goes to 1.
        StartFade(1f, fadeOutDuration);
    }

    void StartFade(float targetAlpha, float duration)
    {
        // If another fade is already running, stop it before starting a new one.
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }

        fadeRoutine = StartCoroutine(FadeCanvasGroup(targetAlpha, duration));
    }

    IEnumerator FadeCanvasGroup(float targetAlpha, float duration)
    {
        // Capture where the fade starts so we can interpolate smoothly.
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;

        // Use unscaled time so the fade still works if Time.timeScale is changed.
        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            // Convert elapsed time into a 0-to-1 progress value.
            float progress = Mathf.Clamp01(elapsedTime / duration);

            // Blend between the current alpha and the target alpha.
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
            yield return null;
        }

        // Snap exactly to the target so we do not leave the overlay slightly off.
        canvasGroup.alpha = targetAlpha;

        // A visible overlay should block clicks, an invisible one should not.
        canvasGroup.blocksRaycasts = targetAlpha > 0f;
        fadeRoutine = null;
    }
}
