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
        PlaceObject[] placeObjects = new PlaceObject[levelStatus.itemCount];
        for (int i = 0; i < levelStatus.itemCount; i++)
            placeObjects[i] = levelStatus.items[i].GetComponent<PlaceObject>();

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
