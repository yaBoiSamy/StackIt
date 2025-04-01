using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatus : MonoBehaviour
{
    public float highestY;

    private const float blockPlacementOffset = 1.5f;

    private GameObject[] levelMeshes;

    //camera vars
    private Vector3 cameraOffset;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    private Rigidbody[] rb;
    public bool isMoving;
    public bool previouslyMoving;
    private PlaceObject[] placeObjects;
    public bool isPlacing;
    
    public float sensitivity;

    void Awake()
    {
        levelMeshes = GameObject.FindGameObjectsWithTag("LevelItemMesh");

        cameraOffset = transform.position;

        rb = new Rigidbody[levelMeshes.Length];
        placeObjects = new PlaceObject[levelMeshes.Length];

        for (int i = 0; i < levelMeshes.Length; i++)
        {
            Transform parent = FindParent(levelMeshes[i]);
            rb[i] = parent.GetComponent<Rigidbody>();
            placeObjects[i] = parent.GetComponent<PlaceObject>();
        }
    }

    void Update()
    {
        highestY = HighestY();
        Debug.Log(highestY);

        Vector3 targetPosition = new Vector3(0, highestY, highestY / Mathf.Tan(80f / 2)) + cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        isMoving = false;
        isPlacing = false;

        for (int i = 0; i < levelMeshes.Length; i++)
        {
            if (placeObjects[i].placingObject)
            {
                isMoving = true;
                isPlacing = true;
            }
            if (rb[i].velocity.magnitude > sensitivity)
            {
                isMoving = true;
            }
        }
    }


    private float HighestY()
    {
        float highestYvalue = 0;

        for (int i = 0; i < levelMeshes.Length; i++)
        {
            Bounds itemBounds = levelMeshes[i].GetComponent<Renderer>().bounds;

            if (itemBounds.max.y > highestYvalue)
            {
                highestYvalue = itemBounds.max.y;
            }
        }
        return highestYvalue;
    }

    private void LateUpdate()
    {
        previouslyMoving = isMoving;
    }

    private Transform FindParent(GameObject childObject)
    {
        Transform currentTransform = childObject.transform;
        while (currentTransform != null)
        {
            PlaceObject placeObject = currentTransform.GetComponent<PlaceObject>();
            if (placeObject != null)
            {
                return currentTransform;
            }
            currentTransform = currentTransform.parent;
        }
        return null;
    }
}
