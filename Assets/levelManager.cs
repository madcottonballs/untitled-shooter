using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting;

public class levelManager : MonoBehaviour
{
    static levelManager instance;

    public CinemachineVirtualCamera cam;
    [SerializeField] Fader fader;
    float zoomStart = 6.5f;
    [SerializeField] float zoomTarget = 3.5f;
    [SerializeField] float zoomDuration = 3.5f;
    public bool isTransitioning; // used by the player so the win condition isnt triggered repeatedly
    public int level = 1;

    public IEnumerator LoadNextLevel() // triggered by player hitting the win condition in movement.cs
    {
        isTransitioning = true;
        enabled = false;

        if (fader == null)
        {
            Debug.Log("fader reference is null in levelManager.cs");
        } 
        else
        {
            fader.FadeOut();
            yield return new WaitForSecondsRealtime(fader.FadeOutDuration);
        }

        level++;
        SceneManager.LoadScene("Level " + level);
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
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        fader = FindObjectOfType<Fader>();
        if (cam == null || fader == null)
        {
            Debug.Log("Camera or fader reference in levelManager.cs is not being found");
        }
        // zooms in
        cam.m_Lens.OrthographicSize = zoomStart;
        StartCoroutine(SmoothZoomIn());
        // fades in
        fader.FadeIn();
        
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
