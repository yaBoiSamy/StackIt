using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HightBar : MonoBehaviour
{
    private const int barHeight = 1500;
    private float alltimeHighestY = 0;
    
    private LevelStatus levelStatus;

    private RectTransform bronzeRT, silverRT, goldRT, bronzeBarRT, silverBarRT, goldBarRT;
    private Trophy bronze, silver, gold;
    private Image foreground;

    public Transform trophyTriggers;
    private Transform bronzeTrigger, silverTrigger, goldTrigger;

    //lerp vars
    private const float lerpDuration = 0.5f;
    private Coroutine lerpCoroutine;


    void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();

        bronzeTrigger = trophyTriggers.Find("Bronze");
        silverTrigger = trophyTriggers.Find("Silver");
        goldTrigger   = trophyTriggers.Find("Gold");

        Transform trophies = transform.Find("Trophies");
        bronzeRT = (RectTransform)trophies.Find("Bronze");
        silverRT = (RectTransform)trophies.Find("Silver");
        goldRT   = (RectTransform)trophies.Find("Gold");

        bronzeBarRT = (RectTransform)transform.Find("Background Bronze");
        silverBarRT = (RectTransform)transform.Find("Background Silver");
        goldBarRT   = (RectTransform)transform.Find("Background Gold");

        bronze = bronzeRT.GetComponent<Trophy>();
        silver = silverRT.GetComponent<Trophy>();
        gold   =   goldRT.GetComponent<Trophy>();

        foreground = transform.Find("Foreground").GetComponent<Image>();

        bronzeBarRT.sizeDelta = new Vector2(50, bronzeTrigger.position.y * barHeight / goldTrigger.position.y);
        silverBarRT.sizeDelta = new Vector2(50, silverTrigger.position.y * barHeight / goldTrigger.position.y - bronzeBarRT.sizeDelta.y);
        goldBarRT.sizeDelta   = new Vector2(50, barHeight - bronzeBarRT.sizeDelta.y - silverBarRT.sizeDelta.y);

        bronzeBarRT.localPosition = Vector3.up * (bronzeBarRT.sizeDelta.y / 2 - 750);
        silverBarRT.localPosition = Vector3.up * (bronzeBarRT.sizeDelta.y + silverBarRT.sizeDelta.y / 2 - 750);
        goldBarRT.localPosition   = Vector3.up * (bronzeBarRT.sizeDelta.y + silverBarRT.sizeDelta.y + goldBarRT.sizeDelta.y / 2 - 750);

        bronzeRT.localPosition = new Vector2(bronzeRT.localPosition.x, bronzeBarRT.sizeDelta.y);
        silverRT.localPosition = new Vector2(silverRT.localPosition.x, bronzeBarRT.sizeDelta.y + silverBarRT.sizeDelta.y);
        goldRT.localPosition   = new Vector2(goldRT.localPosition.x, 1500);
    }
    
    void Update()
    {
        if (levelStatus.previouslyMoving && !levelStatus.isMoving && levelStatus.highestY > alltimeHighestY)
        {
            initiateLerp(1 - levelStatus.highestY / goldTrigger.position.y);
            alltimeHighestY = levelStatus.highestY;
        }

        if (!bronze.Earned() && (barHeight - foreground.fillAmount * barHeight) > bronzeBarRT.sizeDelta.y)
            bronze.Earn();
        else if (!silver.Earned() && (barHeight - foreground.fillAmount * barHeight) > (bronzeBarRT.sizeDelta.y + silverBarRT.sizeDelta.y))
            silver.Earn();
        else if (!gold.Earned() && foreground.fillAmount == 0)
            gold.Earn();
    }

    public void ResetBar()
    {
        alltimeHighestY = 0;
        foreground.fillAmount = 1f;

        bronze.ResetTrophy();
        silver.ResetTrophy();
        gold.ResetTrophy();
    }

    public void initiateLerp(float targetBarHeight)
    {
        if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);
        lerpCoroutine = StartCoroutine(interpolateBarHeight(foreground.fillAmount, targetBarHeight));
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

    public Trophy CurrentTrophy()
    {
        if (gold.Earned())
            return gold;
        if (silver.Earned())
            return silver;
        if (bronze.Earned())
            return bronze;
        return null;
    }
}
