using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trophy : MonoBehaviour
{
    private bool earned;
    private Image checkmark;
    public float fadeDuration = 1f;

    private void Start()
    {
        checkmark = transform.Find("Checkmark").GetComponent<Image>();
    }

    public void Earn()
    {
        earned = true;
        StartCoroutine(FadeInImage());
    }

    public void ResetTrophy()
    {
        earned = false;
        Color finalColor = checkmark.color;
        finalColor.a = 0f;
        checkmark.color = finalColor;
    }

    IEnumerator FadeInImage()
    {
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / fadeDuration);

            Color updatedColor = checkmark.color;
            updatedColor.a = Mathf.Lerp(0f, 1f, t);
            checkmark.color = updatedColor;

            yield return null;
        }

        Color finalColor = checkmark.color;
        finalColor.a = 1f;
        checkmark.color = finalColor;
    }

    public bool Earned() { return earned; }
}
