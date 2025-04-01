using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotater : MonoBehaviour
{
    public float rotation;

    void Start()
    {
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}
