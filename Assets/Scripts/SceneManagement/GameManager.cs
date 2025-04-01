using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject loadingScreen;
    public Image loadingBar;

    public Image fadeImage;
    public float fadeTime;
    private Coroutine fadeCoroutine;

    private int currentScene = 0;

    public float minLoadTime;

    private void Awake()
    {
        instance = this;

        SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
        fadeImage.gameObject.SetActive(false);
    }


    public void LoadScene(int sceneIndex, bool withLoad)
    {
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
        loadOperation.allowSceneActivation = true;
        currentScene = sceneIndex;

        float elapsedLoadTime = 0f;

        while (!loadOperation.isDone)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
        Scene newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        SceneManager.SetActiveScene(newScene);
        DynamicGI.UpdateEnvironment();

        while (elapsedLoadTime <= minLoadTime)
        {
            elapsedLoadTime += Time.deltaTime;
            yield return null;
        }

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


