using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool[] requiresLoadingScreen;

    public GameObject loadingScreen;
    private LoadingAnimation loadingAnimation;

    public Image fadeImage;
    public float fadeTime;
    private Coroutine fadeCoroutine;

    private int currentScene = 0;

    public float minLoadTime;

    private void Awake()
    {
        instance = this;

        SceneManager.LoadScene(0, LoadSceneMode.Additive);
        fadeImage.gameObject.SetActive(false); 
    }

    private void Start()
    {
        loadingAnimation = loadingScreen.GetComponent<LoadingAnimation>();
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(0));
    }


    public void LoadScene(int sceneIndex)
    {
        loadingAnimation.ResetAnimation();
        bool withLoad = requiresLoadingScreen[currentScene] || requiresLoadingScreen[sceneIndex];
        StartCoroutine(LoadSceneAsynchro(sceneIndex, withLoad));
    }

    IEnumerator LoadSceneAsynchro(int sceneIndex, bool withLoad)
    {
        if (withLoad)
        {
            fadeImage.gameObject.SetActive(true);
            fadeImage.canvasRenderer.SetAlpha(0f);
            fadeCoroutine = StartCoroutine(Fade(1));
            while (fadeCoroutine != null)
                yield return null;
            loadingScreen.SetActive(true);
            fadeCoroutine = StartCoroutine(Fade(0));
            while (fadeCoroutine != null)
                yield return null;
        }

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentScene);
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        loadOperation.allowSceneActivation = false;
        currentScene = sceneIndex;

        float elapsedLoadTime = 0f;

        while (loadOperation.progress < 0.9f)
        {
            elapsedLoadTime += Time.deltaTime;
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            Debug.Log("Loading: " + (progress * 100f) + "%");
            yield return null;
        }
        Debug.Log("Loading: 90% complete, waiting for activation.");

        yield return null;
        loadOperation.allowSceneActivation = true;

        while (elapsedLoadTime <= minLoadTime || !loadOperation.isDone)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(currentScene));
        DynamicGI.UpdateEnvironment();

        if (withLoad)
        {
            fadeCoroutine = StartCoroutine(Fade(1));
            while (fadeCoroutine != null)
                yield return null;
            loadingScreen.SetActive(false);
        }

        fadeCoroutine = StartCoroutine(Fade(0));
        while (fadeCoroutine != null)
            yield return null;

        fadeImage.gameObject.SetActive(false);
    }

    private IEnumerator Fade(float target)
    {
        float startAlpha = fadeImage.canvasRenderer.GetAlpha();
        float startTime = Time.time;

        while (Time.time - startTime < fadeTime)
        {
            float normalizedTime = (Time.time - startTime) / fadeTime;
            float currentAlpha = Mathf.Lerp(startAlpha, target, normalizedTime);
            fadeImage.canvasRenderer.SetAlpha(currentAlpha);
            yield return null;
            fadeImage.canvasRenderer.SetAlpha(target);
            fadeCoroutine = null;
        }
    }
}


