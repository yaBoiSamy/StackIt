using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarrouselButton : MonoBehaviour
{
    public Transform levelObject;
    public Sprite objectImage;

    void Start()
    {
        Transform button = transform.Find("Button");
        PlaceObject script = levelObject.GetComponent<PlaceObject>();
        button.GetComponent<Button>().onClick.AddListener(script.spawnObject); 
        button.GetComponent<Image>().sprite = objectImage;
    }
}
