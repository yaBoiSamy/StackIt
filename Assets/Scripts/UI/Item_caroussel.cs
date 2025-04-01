using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item_caroussel : MonoBehaviour
{
    public RectTransform[] carrouselButtons;
    private RectTransform carrouselTransform;

    public  float animationTime;

    public Camera canvasCamera;
    private float clickOffset;

    private int carrouselButtonCount;
    private float carrouselBoundLeft = 0f;
    private float carrouselBoundRight = 0f;
    public float halfSquare;
    public float boundReajustTime;

    private List<int> appearAnimationQueue = new List<int>();

    private Vector3 emptyVector = Vector3.zero;

    private Vector2 previousmousePosition;
    public bool isScrolling;
    public bool previouslyScrolling;

    public bool isAnimating;

    void Start()
    {
        carrouselTransform = gameObject.GetComponent<RectTransform>();
        carrouselButtonCount = carrouselButtons.Length;
        if(carrouselButtonCount > 5f)
        {
            float initialBoundOffset = (carrouselButtonCount - 5f) * 180f;
            carrouselBoundLeft = -initialBoundOffset;
            carrouselBoundRight = initialBoundOffset;
        }
    }
    
    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Ray ray = canvasCamera.ScreenPointToRay(touch.position);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 10f))
            {
                
                GameObject hitObject = hitInfo.collider.gameObject;
                if (hitObject.CompareTag("Carrousel"))
                {
                    if (touch.phase == TouchPhase.Began)
                    {
                        clickOffset = transform.position.x - ray.origin.x;
                    }
                    else
                    {
                        transform.position = new Vector3(ray.GetPoint(10f).x + clickOffset, transform.position.y, transform.position.z);
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = canvasCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;


            if (Physics.Raycast(ray, out hitInfo, 10f))
            {
                GameObject hitObject = hitInfo.collider.gameObject;
                if (hitObject.CompareTag("Carrousel"))
                {
                    if (previousmousePosition.x != Input.mousePosition.x)
                    {
                        isScrolling = true;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        clickOffset = transform.position.x - ray.origin.x;
                    }
                    else if (isScrolling)
                    {
                        transform.position = new Vector3(ray.GetPoint(10f).x + clickOffset, transform.position.y, transform.position.z);
                    } 
                }
                else { isScrolling = false;}
            }
            else{isScrolling = false;}
                
        }
        else{isScrolling = false;}

        if (!isScrolling)
        {
            if (transform.localPosition.x > carrouselBoundRight)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(carrouselBoundRight, transform.localPosition.y, transform.localPosition.z), ref emptyVector, boundReajustTime);
            }
            else if (transform.localPosition.x < carrouselBoundLeft)
            {
                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(carrouselBoundLeft, transform.localPosition.y, transform.localPosition.z), ref emptyVector, boundReajustTime);
            }
        }
    }

    private void LateUpdate()
    {
        previousmousePosition = Input.mousePosition;
        previouslyScrolling = isScrolling;
    }

    public void DisappearThenSlide(int clickedButtonIndex)
    {
        StartCoroutine(disappearAnimation(clickedButtonIndex));
        StartCoroutine(slideIntoPlace(clickedButtonIndex, true));
    }

    public void AppearThenSlide(int clickedButtonIndex)
    {
        if (appearAnimationQueue.Count == 0)
        {
            StartCoroutine(appearAnimation(clickedButtonIndex));
            StartCoroutine(slideIntoPlace(clickedButtonIndex, false));
        }

        appearAnimationQueue.Add(clickedButtonIndex);
    }

    IEnumerator disappearAnimation(int clickedButtonIndex)
    {
        isAnimating = true;
        while (carrouselButtons[clickedButtonIndex].localScale.x > 0.01f)
        {
            float lerpUpdate = Mathf.Lerp(carrouselButtons[clickedButtonIndex].localScale.x, 0, animationTime);
            carrouselButtons[clickedButtonIndex].localScale = new Vector3(lerpUpdate, lerpUpdate, 1f);
            yield return null;
        }

        carrouselButtons[clickedButtonIndex].gameObject.SetActive(false);
        carrouselButtons[clickedButtonIndex].localScale = new Vector3(0f,0f,1f);

        if(carrouselButtonCount > 5)
        {
            carrouselBoundLeft += halfSquare;
            carrouselBoundRight -= halfSquare;
        }
        carrouselButtonCount -= 1;
        isAnimating = false;
    }

    IEnumerator appearAnimation(int clickedButtonIndex)
    {
        isAnimating = true;
        carrouselButtons[clickedButtonIndex].gameObject.SetActive(true);

        while (carrouselButtons[clickedButtonIndex].localScale.x <= 0.99f)
        {
            float lerpUpdate = Mathf.Lerp(carrouselButtons[clickedButtonIndex].localScale.x, 1, animationTime);
            carrouselButtons[clickedButtonIndex].localScale = new Vector3(lerpUpdate, lerpUpdate, animationTime);
            yield return null;
        }

        carrouselButtons[clickedButtonIndex].localScale = Vector3.one;
        carrouselButtonCount += 1;

        if (carrouselButtonCount > 5)
        {
            carrouselBoundLeft -= halfSquare;
            carrouselBoundRight += halfSquare;
        }
    }

    IEnumerator slideIntoPlace(int clickedButtonIndex, bool isDisappearing)
    {
        bool slidingComplete = false;
        List<float> targetPosition = new List<float>();
        if (isDisappearing)
        {
            for (int i=0; i < carrouselButtons.Length; i++)
            {
                if (i < clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].transform.localPosition.x + halfSquare);
                }
                else if ( i > clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].transform.localPosition.x - halfSquare);
                }
                else
                {
                    targetPosition.Add(carrouselButtons[i].transform.localPosition.x);
                }
            }
        }
        else
        {
            for (int i = 0; i < carrouselButtons.Length; i++)
            {
                if (i < clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].transform.localPosition.x - halfSquare);
                }
                else if (i > clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].transform.localPosition.x + halfSquare);
                }
                else if (i == clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].transform.localPosition.x);
                }
            }
        }

        

        while (!slidingComplete)
        {
            slidingComplete = true;
            
            for (int i = 0; i < carrouselButtons.Length; i++)
            {
                carrouselButtons[i].transform.localPosition = new Vector3(Mathf.Lerp(carrouselButtons[i].transform.localPosition.x, targetPosition[i], animationTime), carrouselButtons[i].transform.localPosition.y, carrouselButtons[i].transform.localPosition.z);
                
                if (!(carrouselButtons[i].transform.localPosition.x <= targetPosition[i] + 1f && carrouselButtons[i].transform.localPosition.x >= targetPosition[i] - 1f))
                {
                    slidingComplete = false;
                }
                
            }
            
            yield return null;
        }

        for (int i=0; i < carrouselButtons.Length; i++)
        {
            carrouselButtons[i].transform.localPosition = new Vector3(targetPosition[i], carrouselButtons[i].transform.localPosition.y, carrouselButtons[i].transform.localPosition.z);
        }

        if (appearAnimationQueue.Count <= 1)
        {
            isAnimating = false;
        }
        else
        {
            StartCoroutine(appearAnimation(appearAnimationQueue[1]));
            StartCoroutine(slideIntoPlace(appearAnimationQueue[1], false));
        }

        if(!isDisappearing)
        {
            appearAnimationQueue.RemoveAt(0);
        }
    }
}