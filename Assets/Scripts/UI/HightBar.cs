using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HightBar : MonoBehaviour
{
    private const int barHeight = 1500;

    private float previousUpdateHighestY;
    
    private LevelStatus levelStatus;

    [SerializeField]
    private RectTransform bronzeTrophy, silverTrophy, goldTrophy, bronzeBarT, silverBarT, goldBarT;

    [SerializeField]
    private ImageFader bronzeCheck, silverCheck, goldCheck;

    [SerializeField]
    private Image foreground;

    [SerializeField]
    private Transform bronzeTrigger, silverTrigger, goldTrigger;

    private bool bronzeEarned, silverEarned, goldEarned;

    //lerp vars
    public float lerpDuration = 1f;
    private Coroutine lerpCoroutine;



    void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();

        bronzeBarT.sizeDelta = new Vector2(50, bronzeTrigger.position.y * barHeight / goldTrigger.position.y);
        silverBarT.sizeDelta = new Vector2(50, silverTrigger.position.y * barHeight / goldTrigger.position.y - bronzeBarT.sizeDelta.y);
        goldBarT.sizeDelta = new Vector2(50, barHeight - bronzeBarT.sizeDelta.y - silverBarT.sizeDelta.y);

        bronzeBarT.localPosition =  Vector3.up * (bronzeBarT.sizeDelta.y / 2 - 750);
        silverBarT.localPosition = Vector3.up * (bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y / 2 - 750);
        goldBarT.localPosition =  Vector3.up * (bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y + goldBarT.sizeDelta.y / 2 - 750);

        bronzeTrophy.localPosition = new Vector2(bronzeTrophy.localPosition.x, bronzeBarT.sizeDelta.y);
        silverTrophy.localPosition = new Vector2(silverTrophy.localPosition.x, bronzeBarT.sizeDelta.y + silverBarT.sizeDelta.y);
        goldTrophy.localPosition = new Vector2(goldTrophy.localPosition.x, 1500);
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
