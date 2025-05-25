using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadScene(int index)
    {
        GameManager.instance.LoadScene(index);
    }
}
