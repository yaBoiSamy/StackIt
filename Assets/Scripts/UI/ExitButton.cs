using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButton : MonoBehaviour
{
    void Start()
    {
        Button b = transform.Find("Button").GetComponent<Button>();
        b.onClick.AddListener(() =>
            {
                GameManager.instance.LoadScene(0);
            }
        );
    }
}
