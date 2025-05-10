using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Item_caroussel : MonoBehaviour
{
    private LevelStatus levelStatus;
    private Transform[] carrouselButtons;
    public GameObject carrousselButtonPrefab;

    public float scrollSensitivity;

    private RectTransform carrouselTransform;

    public  float animationTime;

    private Camera canvasCamera;
    private float clickOffset;

    private int carrouselButtonCount;
    private float carrouselBoundLeft = 0f;
    private float carrouselBoundRight = 0f;
    public float halfSquare;
    public float boundReajustTime;

    private List<int> appearAnimationQueue = new List<int>();

    private Vector3 emptyVector = Vector3.zero;
    private Vector3 scrollStartPosition = Vector3.zero;
    private bool isScrolling;
    private bool previouslyScrolling;
    private bool isAnimating;

    void Start()
    {
        Camera[] cameras = new Camera[Camera.allCamerasCount];
        Camera.GetAllCameras(cameras); 
        foreach (Camera cam in cameras)
        {
            if (cam.GetComponent<UniversalAdditionalCameraData>().renderType == CameraRenderType.Overlay)
            {
                canvasCamera = cam;
                break;
            }
        }

        levelStatus = Camera.main.GetComponent<LevelStatus>();
        carrouselTransform = transform.GetComponent<RectTransform>();
        carrouselButtonCount = levelStatus.ItemCount();
        carrouselButtons = new Transform[levelStatus.ItemCount()];

        for (int i = 0; i < levelStatus.ItemCount(); i++)
        {
            carrouselButtons[i] = Instantiate(carrousselButtonPrefab).GetComponent<Transform>();
            carrouselButtons[i].SetParent(transform);
            carrouselButtons[i].GetComponent<CarrouselButton>().Initialize(levelStatus.GetItem(i));
            carrouselButtons[i].localPosition = new Vector3(-446 + 2*halfSquare*i, 0, 0);
            carrouselButtons[i].localScale = Vector3.one;
        }

        if(levelStatus.ItemCount() > 5f)
        {
            float initialBoundOffset = (levelStatus.ItemCount() - 5f) * 180f;
            carrouselBoundLeft = -initialBoundOffset;
            carrouselBoundRight = initialBoundOffset;
        }
    }
    
    void Update()
    {
        bool touchExists = Input.touchCount > 0;
        bool clickExists = Input.GetMouseButton(0);
        if (touchExists || clickExists)
            castCarrouselRay(canvasCamera.ScreenPointToRay(touchExists ? Input.GetTouch(0).position : Input.mousePosition), touchExists);
        else
            isScrolling = false;

        if (isScrolling) return;

        if (transform.localPosition.x > carrouselBoundRight)
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(carrouselBoundRight, transform.localPosition.y, transform.localPosition.z), ref emptyVector, boundReajustTime);
        else if (transform.localPosition.x < carrouselBoundLeft)
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(carrouselBoundLeft, transform.localPosition.y, transform.localPosition.z), ref emptyVector, boundReajustTime);
    }

    private void castCarrouselRay(Ray ray, bool touchExists)
    {
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, 10f)) { isScrolling = false; return; }

        GameObject hitObject = hitInfo.collider.gameObject;
        Vector3 currentPosition = ray.GetPoint(10f);
        if (!hitObject.CompareTag("Carrousel")) return;

        if (Input.GetMouseButtonDown(0) || (touchExists && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            scrollStartPosition = currentPosition;
            clickOffset = transform.position.x - currentPosition.x;
        }
        else
        {
            isScrolling |= (scrollStartPosition - currentPosition).magnitude > 1 / scrollSensitivity;
        }

        transform.position = new Vector3(currentPosition.x + clickOffset, transform.position.y, transform.position.z);
    }


    private void LateUpdate()
    {
        previouslyScrolling = isScrolling;
    }

    public void DisappearThenSlide(Transform levelItem)
    {
        for (int i = 0; i < levelStatus.ItemCount(); i++)
        {
            Transform carrouselButton = carrouselButtons[i];
            if (carrouselButton.GetComponent<CarrouselButton>().LevelItem() == levelItem)
            {
                StartCoroutine(disappearAnimation(carrouselButton));
                StartCoroutine(slideIntoPlace(i, true));
                return;
            }

        }
    }

    public void AppearThenSlide(Transform levelItem)
    {
        for (int i = 0; i < levelStatus.ItemCount(); i++)
        {
            Transform carrouselButton = carrouselButtons[i];
            if (carrouselButton.GetComponent<CarrouselButton>().LevelItem() == levelItem)
            {

                if (appearAnimationQueue.Count == 0)
                {
                    StartCoroutine(appearAnimation(carrouselButton));
                    StartCoroutine(slideIntoPlace(i, false));
                }

                appearAnimationQueue.Add(i);
                return;
            }

        }
    }

    IEnumerator disappearAnimation(Transform carrouselButton)
    {
        isAnimating = true;
        while (carrouselButton.localScale.x > 0.01f)
        {
            float lerpUpdate = Mathf.Lerp(carrouselButton.localScale.x, 0, animationTime);
            carrouselButton.localScale = new Vector3(lerpUpdate, lerpUpdate, 1f);
            yield return null;
        }

        carrouselButton.gameObject.SetActive(false);
        carrouselButton.localScale = new Vector3(0f,0f,1f);

        if(carrouselButtonCount > 5)
        {
            carrouselBoundLeft += halfSquare;
            carrouselBoundRight -= halfSquare;
        }
        carrouselButtonCount -= 1;
        isAnimating = false;
    }

    IEnumerator appearAnimation(Transform carrouselButton)
    {
        isAnimating = true;
        carrouselButton.gameObject.SetActive(true);

        while (carrouselButton.localScale.x <= 0.99f)
        {
            float lerpUpdate = Mathf.Lerp(carrouselButton.localScale.x, 1, animationTime);
            carrouselButton.localScale = new Vector3(lerpUpdate, lerpUpdate, animationTime);
            yield return null;
        }

        carrouselButton.localScale = Vector3.one;
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

                    targetPosition.Add(carrouselButtons[i].localPosition.x + halfSquare);
                }
                else if ( i > clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].localPosition.x - halfSquare);
                }
                else
                {
                    targetPosition.Add(carrouselButtons[i].localPosition.x);
                }
            }
        }
        else
        {
            for (int i = 0; i < carrouselButtons.Length; i++)
            {
                if (i < clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].localPosition.x - halfSquare);
                }
                else if (i > clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].localPosition.x + halfSquare);
                }
                else if (i == clickedButtonIndex)
                {
                    targetPosition.Add(carrouselButtons[i].localPosition.x);
                }
            }
        }

        

        while (!slidingComplete)
        {
            slidingComplete = true;
            
            for (int i = 0; i < carrouselButtons.Length; i++)
            {
                carrouselButtons[i].localPosition = new Vector3(Mathf.Lerp(carrouselButtons[i].localPosition.x, targetPosition[i], animationTime), carrouselButtons[i].localPosition.y, carrouselButtons[i].localPosition.z);
                
                if (!(carrouselButtons[i].localPosition.x <= targetPosition[i] + 1f && carrouselButtons[i].localPosition.x >= targetPosition[i] - 1f))
                {
                    slidingComplete = false;
                }
            }
            
            yield return null;
        }

        for (int i=0; i < carrouselButtons.Length; i++)
        {
            carrouselButtons[i].localPosition = new Vector3(targetPosition[i], carrouselButtons[i].localPosition.y, carrouselButtons[i].localPosition.z);
        }

        if (appearAnimationQueue.Count <= 1)
        {
            isAnimating = false;
        }
        else
        {
            StartCoroutine(appearAnimation(carrouselButtons[appearAnimationQueue[1]]));
            StartCoroutine(slideIntoPlace(appearAnimationQueue[1], false));
        }

        if(!isDisappearing)
        {
            appearAnimationQueue.RemoveAt(0);
        }
    }

    public bool IsScrolling()
    {
        return isScrolling;
    }

    public bool IsAnimating()
    {
        return isAnimating;
    }

    public bool PreviouslyScrolling()
    {
        return previouslyScrolling;
    }
}