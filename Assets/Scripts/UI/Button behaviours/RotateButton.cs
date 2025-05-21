using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateButton : MonoBehaviour
{
    void Start()
    {
        Button b = GetComponent<Button>();
        foreach (Transform item in Camera.main.GetComponent<LevelStatus>().itemsParent)
        {
            b.onClick.AddListener(() => {
                item.GetComponent<PlaceObject>().initiateRotation();
            });
        }
    }
}
