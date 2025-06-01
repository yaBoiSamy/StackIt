using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class LevelStatus : MonoBehaviour
{
    public Transform itemsParent;
    public Transform itemCarrousel;
    public Transform heightBar;
    public Transform inactivePos;

    [HideInInspector] public int itemCount;
    [HideInInspector] public Transform[] items;
    private Renderer[] meshes;
    private Rigidbody[] physics;
    private PlaceObject[] placeObjects;

    [HideInInspector] public float highestY;

    //camera vars
    private Vector3 cameraOffset;
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public bool previouslyMoving;
    [HideInInspector] public bool isPlacing;
    [HideInInspector] public bool isRotating;
    
    public float sensitivity;

    void Awake()
    {
        itemCount = itemsParent.childCount;
        items = new Transform[itemCount];
        List<Renderer> meshList = new List<Renderer>();
        physics = new Rigidbody[itemCount];
        placeObjects = new PlaceObject[itemCount];
        for(int i = 0; i < itemCount; i++)
        {
            items[i] = itemsParent.GetChild(i);
            meshList.AddRange(findMesh(items[i]));
            physics[i] = items[i].GetComponent<Rigidbody>();
            placeObjects[i] = items[i].GetComponent<PlaceObject>();
        }
        meshes = meshList.ToArray();

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
            {
                max = highestVertex;
            }
        }
        return max;
    }

    private void LateUpdate()
    {
        previouslyMoving = isMoving;
    }

    private List<Renderer> findMesh(Transform parent)
    {
        List<Renderer> meshes = new List<Renderer>();
        foreach (Transform child in parent)
        {
            meshes.AddRange(findMesh(child));

            if (!child.gameObject.CompareTag("LevelItemMesh")) continue;
            Renderer r = child.GetComponent<Renderer>();
            if (r != null) meshes.Add(r);
        }
        return meshes;
    }
}
