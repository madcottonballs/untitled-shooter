using System.Collections;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class levelManager : MonoBehaviour
{
    static levelManager instance;

    public CinemachineVirtualCamera cam;
    [SerializeField] Fader fader;
    float zoomStart = 6.5f;
    
    [SerializeField] float zoomTarget = 3.5f;
    [SerializeField] float zoomDuration = 3.5f;
    [SerializeField] lungePromptFader LungePromptScript;
    Coroutine lungePromptRoutine;
    public bool isTransitioning; // used by the player so the win condition isnt triggered repeatedly
    public int level = 1;
    public string lungeMessage = "Press 'Q' to do a lunge attack.";

    private IEnumerator LoadLevel(int level)
    {
        isTransitioning = true;

        if (fader == null)
        {
            Debug.Log("fader reference is null in levelManager.cs");
        }
        else
        {
            // Fade out before loading the next scene so the transition is visible.
            fader.FadeOut();
            yield return new WaitForSecondsRealtime(fader.FadeOutDuration);
        }

        SceneManager.LoadScene("Level " + level);
    }

    public IEnumerator LoadNextLevel() // triggered by player hitting the win condition in movement.cs
    {
        level++;
        yield return LoadLevel(level);
    }
    public IEnumerator ReloadCurrentLevel() // triggered by player dying
    {
        yield return LoadLevel(level);
    }



    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void Start()
    {
        HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            instance = null;
        }
    }

    void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // These objects belong to the newly loaded scene, so reacquire them
        // instead of retaining references from the scene that was unloaded.
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        fader = FindObjectOfType<Fader>();
        LungePromptScript = FindObjectOfType<lungePromptFader>();

        if (cam == null)
        {
            Debug.Log("Camera reference is not being found in levelManager.cs");
        }
        else
        {
            cam.m_Lens.OrthographicSize = zoomStart;
            StartCoroutine(SmoothZoomIn());
        }

        if (fader == null)
        {
            Debug.Log("Fader reference is not being found in levelManager.cs");
        }
        else
        {
            fader.FadeIn();
        }

        if (lungePromptRoutine != null)
        {
            StopCoroutine(lungePromptRoutine);
            lungePromptRoutine = null;
        }

        // The initial Level 1 load also comes through this method via Start().
        if (scene.name == "Level 1" && LungePromptScript != null)
        {
            lungePromptRoutine = StartCoroutine(ShowLungePromptAfterFade());
        }

        isTransitioning = false;
    }

    IEnumerator ShowLungePromptAfterFade()
    {
        // Keep the prompt from being covered by the full-screen scene fader.
        if (fader != null)
        {
            yield return new WaitForSecondsRealtime(fader.FadeInDuration);
        }

        if (LungePromptScript == null)
        {
            yield break;
        }

        LungePromptScript.Show(lungeMessage);
        yield return new WaitForSecondsRealtime(3f);

        if (LungePromptScript != null)
        {
            LungePromptScript.Hide();
        }

        lungePromptRoutine = null;
    }

    IEnumerator SmoothZoomIn()
    {
        float startSize = cam.m_Lens.OrthographicSize;
        float elapsedTime = 0f;

        // Interpolate the orthographic size over time for a smooth zoom effect.
        // SmoothStep gives us an ease-out feel, so the zoom slows near the end.
        while (elapsedTime < zoomDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / zoomDuration);
            progress = Mathf.SmoothStep(0f, 1f, progress);
            cam.m_Lens.OrthographicSize = Mathf.Lerp(startSize, zoomTarget, progress);
            yield return null;
        }

        cam.m_Lens.OrthographicSize = zoomTarget;
    }
}
