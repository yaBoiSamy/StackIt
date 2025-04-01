using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceObject : MonoBehaviour
{
    public bool placingObject = false;
    public int objectIndex;

    public float placementHeight;

    private Rigidbody rb;
    
    //rotate vars
    private Coroutine slerpCoroutine;
    private Quaternion storedTargetRotation;
    private float slerpSpeed = 1f;

    //smoothing vars
    private float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;
    
    private LevelStatus levelStatus;
    
    private Item_caroussel item_Caroussel;

    private void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();
        item_Caroussel = GameObject.FindWithTag("Carrousel").GetComponent<Item_caroussel>(); ;
        gameObject.SetActive(false);
        transform.position = new Vector3(-8, 0, 0);
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (placingObject)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (!item_Caroussel.isScrolling && !item_Caroussel.previouslyScrolling && EventSystem.current.currentSelectedGameObject == null)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Ended)
                    {
                        rb.useGravity = true;
                        rb.velocity = new Vector3(0, -0.2f, 0);
                        placingObject = false;
                    }
                    else
                    {
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        Vector3 targetPosition = new Vector3(ray.GetPoint(2.7f).x, transform.position.y, transform.position.z);
                        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                    }
                }
                else if ((Input.GetMouseButton(0) || Input.GetMouseButtonUp(0)))
                {
                    if (Input.GetMouseButtonUp(0))
                    {
                        rb.useGravity = true;
                        rb.velocity = new Vector3(0, -0.2f, 0);
                        placingObject = false;
                    }
                    else
                    {
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        Vector3 targetPosition = new Vector3(ray.GetPoint(2.7f).x, transform.position.y, transform.position.z);
                        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
                    }
                }
            }
            
        }
    }

    public void spawnObject()
    {
        if (!item_Caroussel.isScrolling && !item_Caroussel.isAnimating && !levelStatus.isPlacing)
        {
            gameObject.SetActive(true);
            item_Caroussel.DisappearThenSlide(objectIndex);
            transform.position = new Vector3(0, levelStatus.highestY + placementHeight, 0);
            placingObject = true;
            rb.useGravity = false;
        }
    }

    public void initiateRotation()
    {
        if (placingObject && slerpCoroutine == null)
        {
            Quaternion targetRotation = transform.rotation * Quaternion.AngleAxis(-90, Vector3.forward);

            slerpCoroutine = StartCoroutine(rotate(targetRotation));
            storedTargetRotation = targetRotation;
        }
        else if(placingObject && slerpCoroutine != null)
        {
            StopCoroutine(slerpCoroutine);
            Quaternion newTargetRotation = storedTargetRotation * Quaternion.AngleAxis(-90, Vector3.forward);
            slerpCoroutine = StartCoroutine(rotate(newTargetRotation));
            storedTargetRotation = newTargetRotation;
        }
    }

    IEnumerator rotate(Quaternion targetRotation)
    {
        float slerpAmount = 0.0f;
        while (slerpAmount < 1.0f)
        {
            slerpAmount += Time.deltaTime * slerpSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slerpAmount);
            yield return null;
        }

        transform.rotation = targetRotation;
        slerpCoroutine = null;
    }
}
