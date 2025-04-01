using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDetector : MonoBehaviour
{
    public GameObject[] levelItems;
    private Rigidbody[] rb;
    public bool isMoving;
    public bool previouslyMoving;
    private PlaceObject[] placeObjects;
    public bool isPlacing;

    [SerializeField]
    private float sensitivity;

    void Start()
    {
        rb = new Rigidbody[levelItems.Length]; 
        placeObjects = new PlaceObject[levelItems.Length];

        for (int i = 0; i < levelItems.Length; i++)
        {
            rb[i] = levelItems[i].GetComponent<Rigidbody>();
            placeObjects[i] = levelItems[i].GetComponent<PlaceObject>();
        }
    }
    
    void Update()
    {
        isMoving = false;
        isPlacing = false;

        for (int i = 0; i < levelItems.Length; i++)
        {
            if(placeObjects[i].placingObject)
            {
                isMoving = true;
                isPlacing = true;
            }
            if(rb[i].velocity.magnitude > sensitivity)
            {
                isMoving = true;
            }
        }
    }

    private void LateUpdate()
    {
        previouslyMoving = isMoving;
    }
}
