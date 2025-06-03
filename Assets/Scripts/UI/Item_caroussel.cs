using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Item_caroussel : MonoBehaviour
{
    public GameObject carrousselButtonPrefab;
    public float animationSpeed;
    public float scrollSensitivity;

    private LevelStatus levelStatus;
    private RectTransform[] carrouselButtons;
    private RectTransform carrousselRT;

    private Queue<Animation> animationQueue = new Queue<Animation>();

    private Camera canvasCamera;
    public float boundPadding;
    private float parentCanvasWidth;
    private float buttonSize;
    private float buttonScale;
    private float carrouselWidth;
    public float boundReajustSpeed;
    private float clickOffset;

    private Vector2 carrouselVelocity = Vector2.zero;
    private Vector2 scrollStartPosition = Vector2.zero;

    [HideInInspector] public bool isScrolling;
    [HideInInspector] public bool previouslyScrolling;
    [HideInInspector] public bool isAnimating;

    struct Animation
    {
        public enum AnimationType { APPEARING, DISAPPEARING };
        public AnimationType type;
        public int index;

        public Animation(AnimationType type, int index)
        {
            this.type = type;
            this.index = index;
        }
    }

    void Start()
    {
        RectTransform carrouselButtonRT = carrousselButtonPrefab.GetComponent<RectTransform>();
        buttonScale = carrouselButtonRT.localScale.x;
        buttonSize = carrouselButtonRT.sizeDelta.x * buttonScale;

        carrousselRT = GetComponent<RectTransform>();
        carrousselRT.anchoredPosition = new Vector2(boundPadding, carrousselRT.anchoredPosition.y);

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
        carrouselButtons = new RectTransform[levelStatus.itemCount];

        parentCanvasWidth = GetComponentInParent<Canvas>().GetComponent<RectTransform>().rect.width;
        carrouselWidth = buttonSize * levelStatus.itemCount * transform.localScale.x;

        for (int i = 0; i < levelStatus.itemCount; i++)
        {
            carrouselButtons[i] = Instantiate(carrousselButtonPrefab).GetComponent<RectTransform>();
            carrouselButtons[i].SetParent(transform);
            carrouselButtons[i].GetComponent<CarrouselButton>().Initialize(levelStatus.items[i]);
            carrouselButtons[i].anchoredPosition = new Vector2(buttonSize * i, 0);
            carrouselButtons[i].localScale = buttonScale * Vector2.one;
        }

        float carrouselWidthScaleNormalized = carrouselWidth / transform.localScale.x;

        GetComponent<BoxCollider>().size = new Vector3(carrouselWidthScaleNormalized + 2 * boundPadding, carrousselRT.sizeDelta.y, 1);
        GetComponent<BoxCollider>().center = new Vector3(carrouselWidthScaleNormalized / 2, 0, 0);

    }

    void Update()
    {
        bool touchExists = Input.touchCount > 0;
        bool clickExists = Input.GetMouseButton(0);
        if (touchExists || clickExists)
            CastCarrouselRay(canvasCamera.ScreenPointToRay(touchExists ? Input.GetTouch(0).position : Input.mousePosition), touchExists);
        else
            isScrolling = false;

        if (isScrolling) return;

        if (carrousselRT.anchoredPosition.x + Mathf.Max(carrouselWidth, parentCanvasWidth - 2 * boundPadding) < parentCanvasWidth - boundPadding)
        {
            Vector2 objective = new Vector3(parentCanvasWidth - boundPadding - Mathf.Max(carrouselWidth, parentCanvasWidth - 2 * boundPadding), carrousselRT.anchoredPosition.y);
            carrousselRT.anchoredPosition = Vector2.SmoothDamp(carrousselRT.anchoredPosition, objective, ref carrouselVelocity, 1 / boundReajustSpeed);
        }
        else if (carrousselRT.anchoredPosition.x > boundPadding)
        {
            Vector2 objective = new Vector3(boundPadding, carrousselRT.anchoredPosition.y);
            carrousselRT.anchoredPosition = Vector2.SmoothDamp(carrousselRT.anchoredPosition, objective, ref carrouselVelocity, 1 / boundReajustSpeed);
        }

        if (!isAnimating && animationQueue.Count != 0) 
        {
            StartCoroutine(CarrouselAnimation(animationQueue.Dequeue()));
        }
    }

    private void CastCarrouselRay(Ray ray, bool touchExists)
    {
        RaycastHit hitInfo;
        if (!Physics.Raycast(ray, out hitInfo, 10f)) { isScrolling = false; return; }

        GameObject hitObject = hitInfo.collider.gameObject;
        Vector2 currentPosition = ray.GetPoint(10f);
        if (!hitObject.CompareTag("Carrousel")) return;

        if (Input.GetMouseButtonDown(0) || (touchExists && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            scrollStartPosition = currentPosition;
            clickOffset = carrousselRT.position.x - currentPosition.x;
        }
        else
        {
            isScrolling |= (scrollStartPosition - currentPosition).magnitude > 1 / scrollSensitivity;
        }

        carrousselRT.position = new Vector2(currentPosition.x + clickOffset, carrousselRT.position.y);
    }


    private void LateUpdate()
    {
        previouslyScrolling = isScrolling;
    }

    public void DisappearThenSlide(int carrouselButtonIndex)
    {
        animationQueue.Enqueue(new Animation(Animation.AnimationType.DISAPPEARING, carrouselButtonIndex));
    }

    public void AppearThenSlide(int carrouselButtonIndex)
    {
        animationQueue.Enqueue(new Animation(Animation.AnimationType.APPEARING, carrouselButtonIndex));
    }

    public void DisappearThenSlide(Transform levelItem)
    {
        for (int i = 0; i < levelStatus.itemCount; i++)
        {
            if (carrouselButtons[i].GetComponent<CarrouselButton>().LevelItem() == levelItem)
            {
                DisappearThenSlide(i);
                return;
            }
        }
    }

    public void AppearThenSlide(Transform levelItem)
    {
        for (int i = 0; i < levelStatus.itemCount; i++)
        {
            if (carrouselButtons[i].GetComponent<CarrouselButton>().LevelItem() == levelItem)
            {
                AppearThenSlide(i);
                return;
            }
        }
    }

    IEnumerator CarrouselAnimation(Animation anim)
    {
        bool isAppearing = anim.type == Animation.AnimationType.APPEARING;
        int animationSign = isAppearing ? 1 : -1;

        isAnimating = true;
        float animationProgress = 0;
        float animationVelocity = 0;

        Vector2[] initialPositions = new Vector2[carrouselButtons.Length];
        for (int i = 0; i < carrouselButtons.Length; i++)
            initialPositions[i] = carrouselButtons[i].anchoredPosition;


        RectTransform animatedButton = carrouselButtons[anim.index];
        if (isAppearing)
            animatedButton.gameObject.SetActive(true);

        while (animationProgress < 0.99f)
        {
            float slideScaling = Mathf.Lerp(0, buttonSize / 2, animationProgress);
            float buttonScaling = isAppearing ? animationProgress : 1 - animationProgress;
            animatedButton.localScale = buttonScale * (new Vector3(buttonScaling, buttonScaling, 1));
            for (int i = anim.index + 1; i < carrouselButtons.Length; i++)
            {
                RectTransform curr = carrouselButtons[i];
                Vector2 currInitPos = initialPositions[i];
                curr.anchoredPosition = new Vector2(currInitPos.x + animationSign * 2 * slideScaling, currInitPos.y);
            }
            animationProgress = Mathf.SmoothDamp(animationProgress, 1f, ref animationVelocity, 1 / animationSpeed);
            yield return null;
        }

        animatedButton.localScale = isAppearing? buttonScale * Vector3.one : Vector3.zero;
        for (int i = anim.index + 1; i < carrouselButtons.Length; i++)
        {
            RectTransform curr = carrouselButtons[i];
            Vector2 currInitPos = initialPositions[i];
            curr.anchoredPosition = new Vector2(currInitPos.x + animationSign * buttonSize, currInitPos.y);
        }

        if (!isAppearing)
            animatedButton.gameObject.SetActive(false);
        carrouselWidth += animationSign * buttonSize;
        isAnimating = false;
    }
}