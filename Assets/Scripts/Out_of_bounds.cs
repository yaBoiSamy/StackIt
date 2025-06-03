using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Out_of_bounds : MonoBehaviour
{
    private Item_caroussel item_Caroussel;
    private LevelStatus levelStatus;

    private void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();
        item_Caroussel = levelStatus.itemCarrousel.GetComponent<Item_caroussel>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (levelStatus.isPlacing) return;
        FindPlaceObject(collision).DeactivateObject();
    }

    private void Update()
    {
        if (item_Caroussel.isScrolling && !levelStatus.isPlacing) return;

        if (Input.touchCount > 0 && EventSystem.current.currentSelectedGameObject == null)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Ended)
                verifyObjectClicked(touch.position);
        }

        if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
            verifyObjectClicked(Input.mousePosition);
    }

    private void verifyObjectClicked(Vector3 clickPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 20f))
        {
            if (levelStatus.isPlacing) return;
            PlaceObject placeObject = FindPlaceObject(hitInfo.collider);
            if (placeObject == null) return;
            FindPlaceObject(hitInfo.collider).DeactivateObject();
        }
    }

    private PlaceObject FindPlaceObject(Collider coll)
    {
        Transform currentTransform = coll.transform;
        while (currentTransform != null)
        {
            PlaceObject script = currentTransform.GetComponent<PlaceObject>();
            if (script != null)
                return script;
            currentTransform = currentTransform.parent;
        }
        return null;
    }
}
