using UnityEngine;

public class CameraPanIn : MonoBehaviour
{
    // camera 
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float panDuration = 2f;

    // UI
    public GameObject uiCanvas;

    void Start()
    {
        transform.position = startPosition;
        StartCoroutine(PanIn());
    }

    // pan in camera on scene load
    System.Collections.IEnumerator PanIn()
    {
        float elapsed = 0f;

        while (elapsed < panDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / panDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Make sure it ends exactly at the target
        transform.position = endPosition;

        // show canvas
        ShowUI();
    }

    // show canvas
    public void ShowUI(){
       uiCanvas.SetActive(true); 
    }
}
