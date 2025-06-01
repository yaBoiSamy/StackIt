using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlaceObject : MonoBehaviour
{
    private bool placingObject = false;
    public float placementHeight;
    public Sprite buttonImage;


    private Rigidbody rb;
    
    //rotate vars
    private Coroutine slerpCoroutine;
    private Quaternion storedTargetRotation;
    private float slerpDuration = 0.4f;
    private float slerpSharpness = 3;

    //smoothing vars
    private float smoothTime = 0.1f;
    private Vector3 velocity = Vector3.zero;
    
    private LevelStatus levelStatus;
    
    private Item_caroussel item_Caroussel;

    private void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();
        item_Caroussel = levelStatus.itemCarrousel.GetComponent<Item_caroussel>();
        gameObject.SetActive(false);
        transform.position = new Vector3(-8, -8, 0);
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!placingObject) return;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        if (levelStatus.isRotating) return;

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

    public void spawnObject()
    {
        if (item_Caroussel.isScrolling || item_Caroussel.isAnimating || levelStatus.isPlacing) return;
        
        gameObject.SetActive(true);
        item_Caroussel.DisappearThenSlide(transform);
        transform.position = new Vector3(0, levelStatus.highestY + placementHeight, 0);
        placingObject = true;
        rb.useGravity = false;
    }

    public void DeactivateObject()
    {
        if (!isActiveAndEnabled) return;
        transform.position = levelStatus.inactivePos.position;
        transform.rotation = Quaternion.identity;
        item_Caroussel.AppearThenSlide(transform);
        gameObject.SetActive(false);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void initiateRotation()
    {
        levelStatus.isRotating = true;
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
        float Tanh(float x) => (MathF.Exp(x) - MathF.Exp(-x)) / (MathF.Exp(x) + MathF.Exp(-x));
        float Coth(float x) => (MathF.Exp(x) + MathF.Exp(-x)) / (MathF.Exp(x) - MathF.Exp(-x));
        Func<float, float> slerpFunc = x => (float) (Coth(slerpSharpness / 2) * Tanh(slerpSharpness * (x - 0.5f)) / 2 + 0.5f);
        Quaternion initialRotation = transform.rotation;
        float timer = 0.0f;
        float progress = 0.0f;
        while (progress < 0.99f)
        {
            timer += Time.deltaTime;
            progress = timer / slerpDuration;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, slerpFunc(progress));
            yield return null;
        }

        transform.rotation = targetRotation;
        slerpCoroutine = null;
        levelStatus.isRotating = false;
    }

    public bool PlacingObject()
    {
        return placingObject;
    }
}
