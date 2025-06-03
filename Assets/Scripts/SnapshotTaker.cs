using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SnapshotTaker : MonoBehaviour
{
    public string defaultPath = "";
    public Vector2Int resolution = new Vector2Int(1080, 1920);
    public Canvas uiCanvas; // Assign this in Inspector or dynamically

    public void Start()
    {
        if (string.IsNullOrEmpty(defaultPath))
            defaultPath = Application.dataPath + "/Snapshots/";

        if (uiCanvas == null)
            uiCanvas = transform.Find("Canvas")?.GetComponent<Canvas>();

        uiCanvas.transform.Find("SnapshotButton").GetComponent<Button>().onClick.AddListener(TakeScreenshot);
    }

    public void TakeScreenshot()
    {
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"screenshot_{timestamp}.png";
        string path = Path.Combine(defaultPath, filename);

        if (!Directory.Exists(defaultPath))
            Directory.CreateDirectory(defaultPath);

        StartCoroutine(CaptureWithoutUI(path));
    }

    private IEnumerator CaptureWithoutUI(string path)
    {
        // Hide the UI before the screenshot
        if (uiCanvas != null)
            uiCanvas.enabled = false;

        // Wait till the end of frame so we get a clean render
        yield return new WaitForEndOfFrame();

        ScreenCapture.CaptureScreenshot(path);
        Debug.Log($"Screenshot saved to: {path}");

        // Wait one more frame before re-enabling UI to ensure nothing flashes
        yield return null;

        if (uiCanvas != null)
            uiCanvas.enabled = true;
    }
}

