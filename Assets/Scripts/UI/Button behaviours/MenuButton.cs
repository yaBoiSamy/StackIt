using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public float animationSpeed;

    private RectTransform menuRT;
    private RectTransform[] childButtonRTs;
    private int childButtonCount;
    private float[] deployedPositions;
    private float buttonSize;
    private bool isDeployed = false;
    private bool isAnimating = false;

    void Start()
    {
        menuRT = GetComponent<RectTransform>();
        Transform[] childButtonTs = new Transform[3];
        childButtonTs[0] = transform.Find("Settings button");
        childButtonTs[1] = transform.Find("Restart button");
        childButtonTs[2] = transform.Find("Exit button");
        childButtonCount = childButtonTs.Length;
        childButtonRTs = new RectTransform[childButtonCount];
        for (int i = 0; i < childButtonCount; i++)
            childButtonRTs[i] = childButtonTs[i].GetComponent<RectTransform>();

        buttonSize = childButtonRTs[0].sizeDelta.x * childButtonRTs[0].localScale.x;

        deployedPositions = new float[childButtonCount];
        for (int i = 0; i < childButtonCount; i++)
            deployedPositions[i] = (i + 1) * buttonSize;

        Button b = transform.Find("Button").GetComponent<Button>();
        b.onClick.AddListener(ToggleDeployment);
    }

    void ToggleDeployment()
    {
        if (isAnimating) return;
        isDeployed = !isDeployed;
        StartCoroutine(DeploymentAnimation());
    }

    IEnumerator DeploymentAnimation()
    {
        isAnimating = true;

        float animationProgress = 0;
        float animationVelocity = 0;

        if (isDeployed)
        {
            foreach (RectTransform rt in childButtonRTs)
                rt.gameObject.SetActive(true);
        }

        while (animationProgress < 0.99f)
        {
            float[] slideScaling = new float[childButtonCount];
            for (int i = 0; i < childButtonCount; i++)
            {
                slideScaling[i] = Mathf.Lerp(isDeployed ? 0 : deployedPositions[i], isDeployed ? deployedPositions[i] : 0, animationProgress);
                childButtonRTs[i].anchoredPosition = Vector2.right * slideScaling[i];
            }

            animationProgress = Mathf.SmoothDamp(animationProgress, 1f, ref animationVelocity, 1 / animationSpeed);
            yield return null;
        }

        for (int i = 0; i < childButtonCount; i++)
        {
            childButtonRTs[i].anchoredPosition = Vector2.right * (isDeployed ? deployedPositions[i] : 0);
        }

        if (!isDeployed)
        {
            foreach (RectTransform rt in childButtonRTs)
                rt.gameObject.SetActive(false);
        }

        isAnimating = false;
    }


    public bool IsDeployed()
    {
        return isDeployed;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }
}
