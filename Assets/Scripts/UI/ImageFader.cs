using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFader : MonoBehaviour
{
    private Image image;
    public float fadeDuration = 1f;

    private void Start()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void fadeImage(float targetFade)
    {
        StartCoroutine(FadeInImage());
    }

    IEnumerator FadeInImage()
    {
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / fadeDuration);
            
            Color updatedColor = image.color;
            updatedColor.a = Mathf.Lerp(0f, 1f, t);
            image.color = updatedColor;

            yield return null;
        }
        
        Color finalColor = image.color;
        finalColor.a = 1f;
        image.color = finalColor;
    }
}
