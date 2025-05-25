using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetButton : MonoBehaviour
{
    void Start()
    {
        LevelStatus levelStatus = Camera.main.GetComponent<LevelStatus>();
        HightBar heightBar = levelStatus.heightBar.GetComponent<HightBar>();
        PlaceObject[] placeObjects = new PlaceObject[levelStatus.ItemCount()];
        for (int i = 0; i < levelStatus.ItemCount(); i++)
        {
            placeObjects[i] = levelStatus.GetItem(i).GetComponent<PlaceObject>();
        }

        Button b = transform.Find("Button").GetComponent<Button>();
        b.onClick.AddListener(() =>
            {
                for (int i = 0; i < placeObjects.Length; i++) {
                    placeObjects[i].DeactivateObject();
                }
                heightBar.ResetBar();
            }
        );
    }
}
