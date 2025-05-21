using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarrouselButton : MonoBehaviour
{
    private Transform levelItem = null;
    public void Initialize(Transform levelItem)
    {
        this.levelItem = levelItem;
        Transform button = transform.Find("Button");
        PlaceObject script = levelItem.GetComponent<PlaceObject>();
        button.GetComponent<Button>().onClick.AddListener(() => {
            script.spawnObject();
        });
        button.GetComponent<Image>().sprite = script.buttonImage;
    }

    public Transform LevelItem()
    {
        return levelItem;
    }
}
