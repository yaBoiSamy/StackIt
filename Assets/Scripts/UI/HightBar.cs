using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HightBar : MonoBehaviour
{
    private const int barHeight = 1500;

    private float previousUpdateHighestY;
    
    private LevelStatus levelStatus;

    private RectTransform bronzeTrophy, silverTrophy, goldTrophy, bronzeBarT, silverBarT, goldBarT;
    private ImageFader bronzeCheck, silverCheck, goldCheck;
    private Image foreground;

    public Transform bronzeTrigger, silverTrigger, goldTrigger;

    private bool bronzeEarned, silverEarned, goldEarned;

    //lerp vars
    public float lerpDuration = 1f;
    private Coroutine lerpCoroutine;



    void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();

        Transform trophies = transform.Find("Trophies");
        bronzeTrophy = (RectTransform)trophies.Find("Bronze");
        silverTrophy = (RectTransform)trophies.Find("Silver");
        goldTrophy   = (RectTransform)trophies.Find("Gold");

        bronzeBarT = (RectTransform)transform.Find("Background Bronze");
        silverBarT = (RectTransform)transform.Find("Background Silver");
        goldBarT   = (RectTransform)transform.Find("Background Gold");

        bronzeCheck = bronzeTrophy.Find("Checkmark").GetComponent<ImageFader>();
        silverCheck = silverTrophy.Find("Checkmark").GetComponent<ImageFader>();
        goldCheck   =   goldTrophy.Find("Checkmark").GetComponent<ImageFader>();

        foreground = transform.Find("Foreground").GetComponent<Image>();

        bronzeBarT.sizeDelta = new Vector2(50, bronzeTrigger.position.y * barHeight / goldTrigger.position.y);
        silverBarT.sizeDelta = new Vector2(50, silverTrigger.position.y * barHeight / goldTrigger.position.y - bronzeBarT.sizeDelta.y);
        goldBarT.sizeDelta   = new Vector2(50, barHeight - bronzeBarT.sizeDelta.y - silverBarT.sizeDelta.y);

        bronzeBarT.localPosition = Vector3.up * (bronzeBarT.sizeDelta.y / 2 - 750);
        silverBarT.localPosition = Vector3.up * (bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y / 2 - 750);
        goldBarT.localPosition   = Vector3.up * (bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y + goldBarT.sizeDelta.y / 2 - 750);

        bronzeTrophy.localPosition = new Vector2(bronzeTrophy.localPosition.x, bronzeBarT.sizeDelta.y);
        silverTrophy.localPosition = new Vector2(silverTrophy.localPosition.x, bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y);
        goldTrophy.localPosition   = new Vector2(goldTrophy.localPosition.x, 1500);
    }
    
    void Update()
    {
        if (levelStatus.previouslyMoving == true && levelStatus.isMoving == false && levelStatus.highestY > previousUpdateHighestY)
        {
            initiateLerp(1 - levelStatus.highestY / goldTrigger.position.y);
            previousUpdateHighestY = levelStatus.highestY;
        }
        if (!bronzeEarned && (barHeight - foreground.fillAmount * barHeight) > bronzeBarT.sizeDelta.y)
        {
            bronzeCheck.fadeImage(1f);
            bronzeEarned = true;
        }
        else if (!silverEarned && (barHeight - foreground.fillAmount * barHeight) > (bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y))
        {
            silverCheck.fadeImage(1f);
            silverEarned = true;
        }
        else if (!goldEarned && foreground.fillAmount == 0)
        {
            goldCheck.fadeImage(1f);
            goldEarned = true;
        }
    }

    public void initiateLerp(float targetBarHeight)
    {
        if (lerpCoroutine == null)
        {
            lerpCoroutine = StartCoroutine(interpolateBarHeight(foreground.fillAmount, targetBarHeight));
        }
        else if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
            lerpCoroutine = StartCoroutine(interpolateBarHeight(foreground.fillAmount, targetBarHeight));
        }
    }

    IEnumerator interpolateBarHeight(float startValue, float endValue)
    {
        float currentTime = 0f;

        while (currentTime < lerpDuration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentTime / lerpDuration);
            float interpolatedValue = Mathf.Lerp(startValue, endValue, t);

            foreground.fillAmount = interpolatedValue;

            yield return null;
        }

        foreground.fillAmount = endValue;
        lerpCoroutine = null;
    }
}
