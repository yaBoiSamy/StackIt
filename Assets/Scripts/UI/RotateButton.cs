using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButton : MonoBehaviour
{
    void Start()
    {
        foreach (Transform item in Camera.main.GetComponent<LevelStatus>().itemsParent)
        {
            GetComponent<Button>().onClick.AddListener(() => {
                item.GetComponent<PlaceObject>().initiateRotation();
            });
        }
    }
}
