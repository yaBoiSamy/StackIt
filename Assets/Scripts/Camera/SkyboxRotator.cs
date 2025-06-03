using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("Rotation angle for the skybox in degrees.")]
    public float angle = 0f;

    [Tooltip("Enable this to apply the rotation every frame (for testing).")]
    public bool testMode = false;

    void Start()
    {
        ApplyRotation();
    }

    void Update()
    {
        if (testMode)
        {
            ApplyRotation();
        }
    }

    void ApplyRotation()
    {
        if (RenderSettings.skybox.HasProperty("_Rotation"))
        {
            RenderSettings.skybox.SetFloat("_Rotation", angle);
        }
        else
        {
            Debug.LogWarning("The current skybox material does not support _Rotation.");
        }
    }
}
