using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class ScaleWithMax : MonoBehaviour
{
    public Vector2 referenceResolution = new Vector2(1080, 1920);
    public float scaleAdjustment = 3.7f;
    private CanvasScaler scaler;

    void Awake()
    {
        scaler = GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;

        float screenRatio = (float)Screen.width / Screen.height;
        float refRatio = referenceResolution.x / referenceResolution.y;

        // We scale based on the bigger axis
        float scaleFactor;
        if (screenRatio < refRatio)
            scaleFactor = scaleAdjustment * Screen.height / referenceResolution.y;
        else
            scaleFactor = scaleAdjustment * Screen.width / referenceResolution.x;

        scaler.scaleFactor = scaleFactor;
    }

    void Update()
    {
        float screenRatio = (float)Screen.width / Screen.height;
        float refRatio = referenceResolution.x / referenceResolution.y;

        // We scale based on the bigger axis
        float scaleFactor;
        if (screenRatio < refRatio)
            scaleFactor = scaleAdjustment * Screen.height / referenceResolution.y;
        else
            scaleFactor = scaleAdjustment * Screen.width / referenceResolution.x;

        scaler.scaleFactor = scaleFactor;
    }
}
