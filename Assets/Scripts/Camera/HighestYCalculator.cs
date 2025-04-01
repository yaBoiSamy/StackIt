using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighestYCalculator : MonoBehaviour
{
    public float highestY;

    private const float blockPlacementOffset = 1.5f;

    public GameObject[] levelItems;

    //camera vars
    private Vector3 cameraOffset;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        cameraOffset = transform.position;
    }

    void Update()
    {
        highestY = HighestY();

        Vector3 targetPosition = new Vector3(0, highestY, highestY / Mathf.Tan(80f / 2)) + cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    private float HighestY()
    {
        float highestYvalue = 0;

        for (int i = 0; i < levelItems.Length; i++)
        {
            Bounds itemBounds = levelItems[i].GetComponent<Renderer>().bounds;

            if (itemBounds.max.y > highestYvalue)
            {
                highestYvalue = itemBounds.max.y;
            }
        }
        return highestYvalue;
    }
}
