using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{
    public bool withLoadingScreen;

    public void SwitchScene(int sceneIndex)
    {
        if (GameManager.instance != null)
        {
            GameManager.instance.LoadScene(sceneIndex, withLoadingScreen);
        }
        else
        {
            Debug.LogError("GameManager instance is not set.");
        }
    }
}
