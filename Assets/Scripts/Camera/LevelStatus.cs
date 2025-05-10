using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class LevelStatus : MonoBehaviour
{
    public Transform itemsParent;
    public Transform itemCarrousel;

    private int itemCount;
    private Transform[] items;
    private Renderer[] meshes;
    private Rigidbody[] physics;
    private PlaceObject[] placeObjects;

    private float highestY;

    //camera vars
    private Vector3 cameraOffset;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    private bool isMoving;
    private bool previouslyMoving;
    private bool isPlacing;
    
    public float sensitivity;

    void Awake()
    {
        itemCount = itemsParent.childCount;
        items = new Transform[itemCount];
        meshes = new Renderer[itemCount];
        physics = new Rigidbody[itemCount];
        placeObjects = new PlaceObject[itemCount];
        for(int i = 0; i < itemCount; i++)
        {
            items[i] = itemsParent.GetChild(i);
            meshes[i] = findMesh(items[i]);
            physics[i] = items[i].GetComponent<Rigidbody>();
            placeObjects[i] = items[i].GetComponent<PlaceObject>();
        }

        cameraOffset = transform.position;
    }

    void Update()
    {
        highestY = ComputeHighestY();

        Vector3 targetPosition = new Vector3(0, highestY, highestY / Mathf.Tan(80f / 2)) + cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        isMoving = false;
        isPlacing = false;

        foreach(PlaceObject placeObject in placeObjects)
        {
            isMoving |= placeObject.PlacingObject();
            isPlacing |= placeObject.PlacingObject();
        }

        foreach(Rigidbody rb in physics)
        {
            isMoving |= rb.velocity.magnitude > sensitivity;
        }
    }


    private float ComputeHighestY()
    {
        float max = 0;

        foreach (Renderer mesh in meshes)
        {
            float highestVertex = mesh.bounds.max.y;

            if (highestVertex > max)
                max = highestVertex;
        }
        return max;
    }

    private void LateUpdate()
    {
        previouslyMoving = isMoving;
    }

    private Renderer findMesh(Transform root)
    {
        Renderer res = root.GetComponent<Renderer>();
        if (res != null) return res;


        foreach (Transform child in root)
        {
            res = findMesh(child);
            if (res != null) return res;
        }
        return null;
    }

    public int ItemCount()
    {
        return itemCount;
    }

    public Transform GetItem(int itemIndex)
    {
        return items[itemIndex];
    }

    public bool IsPlacing()
    {
        return isPlacing;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool PreviouslyMoving()
    {
        return previouslyMoving;
    }

    public float HighestY()
    {
        return highestY;
    }
}
