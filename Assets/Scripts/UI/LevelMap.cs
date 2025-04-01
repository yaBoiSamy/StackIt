using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelMap : MonoBehaviour
{
    private bool isSlidingLevels;
    public float smoothTime;
    private Vector3 smoothDampProgress;
    private int screenResX;
    public int worldCount;
    private int[] anchors;
    private float clickOffset;
    private int observedWorld = 0;

    private void Start()
    {
        screenResX = Screen.width;
        anchors = new int[worldCount];

        for (int i = 0; i < anchors.Length; i++)
        {
            anchors[i] = i * screenResX * -1;
        }
    }

    void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    isSlidingLevels = true;
                    clickOffset = transform.localPosition.x - touch.position.x;
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    isSlidingLevels = false;
                }
                else if ((observedWorld == 0 && Input.mousePosition.x + clickOffset >= anchors[observedWorld]) || (observedWorld == worldCount - 1 && Input.mousePosition.x + clickOffset <= anchors[observedWorld]))
                {
                    clickOffset = transform.localPosition.x - Input.mousePosition.x;
                }
                else
                {
                    transform.localPosition = new Vector3(touch.position.x + clickOffset, transform.localPosition.y, transform.localPosition.z);
                }
            }
            else if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isSlidingLevels = true;
                    clickOffset = transform.localPosition.x - Input.mousePosition.x;
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    isSlidingLevels = false;
                }
                else if ((observedWorld == 0 && Input.mousePosition.x + clickOffset >= anchors[observedWorld]) || (observedWorld == worldCount - 1 && Input.mousePosition.x + clickOffset <= anchors[observedWorld]))
                {
                    clickOffset = transform.localPosition.x - Input.mousePosition.x;
                }
                else
                {
                    transform.localPosition = new Vector3(Input.mousePosition.x + clickOffset, transform.localPosition.y, transform.localPosition.z);
                }
            }
        }

        if (anchors[observedWorld]  + screenResX / 2 < transform.localPosition.x && observedWorld != 0)
        {
            observedWorld--;
        }
        else if (anchors[observedWorld] - screenResX / 2 > transform.localPosition.x && observedWorld != worldCount - 1)
        {
            observedWorld++;
        }

        if (!isSlidingLevels)
        {
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, new Vector3(anchors[observedWorld], transform.localPosition.y, transform.localPosition.z), ref smoothDampProgress, smoothTime);
        }
    }
}
