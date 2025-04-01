using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Out_of_bounds : MonoBehaviour
{
    public Transform inactivePos;
    private Item_caroussel item_Caroussel;
    private LevelStatus levelStatus;

    private void Start()
    {
        levelStatus = Camera.main.GetComponent<LevelStatus>();
        item_Caroussel = GameObject.FindWithTag("Carrousel").GetComponent<Item_caroussel>();
    }

    private void OnTriggerEnter(Collider collision)
    {
        DeactivateObject(collision.gameObject);
    }

    private void Update()
    {
        if (!(item_Caroussel.isScrolling || levelStatus.isPlacing))
        {
            if (Input.touchCount > 0 && EventSystem.current.currentSelectedGameObject == null)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    RaycastHit hitInfo;

                    if (Physics.Raycast(ray, out hitInfo, 20f))
                    {
                        DeactivateObject(hitInfo.collider.gameObject);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0) && EventSystem.current.currentSelectedGameObject == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(ray, out hitInfo, 20f))
                {
                    DeactivateObject(hitInfo.collider.gameObject);
                }
            }
        }
    }

    private void DeactivateObject(GameObject item)
    {
        Transform collisionParent = FindParentRigidbody(item);
        if(collisionParent == null)
        {
            return;
        }
        Rigidbody collisionRB = collisionParent.GetComponent<Rigidbody>();
        PlaceObject placeObject = collisionParent.GetComponent<PlaceObject>();

        if (!placeObject.placingObject && collisionParent.CompareTag("LevelObject"))
        {
            collisionParent.position = inactivePos.position;
            collisionParent.rotation = Quaternion.identity;
            item_Caroussel.AppearThenSlide(placeObject.objectIndex);
            collisionParent.gameObject.SetActive(false);
            collisionRB.velocity = Vector3.zero;
            collisionRB.angularVelocity = Vector3.zero;
        }
    }

    private Transform FindParentRigidbody(GameObject childObject)
    {
        Transform currentTransform = childObject.transform;
        while (currentTransform != null)
        {
            Rigidbody rigidbody = currentTransform.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                return currentTransform;
            }
            currentTransform = currentTransform.parent;
        }
        return null;
    }
}
