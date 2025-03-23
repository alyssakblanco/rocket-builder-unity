using UnityEngine;
using TMPro;
using System.Collections; 
using System.Collections.Generic; 

[System.Serializable]
public class RocketInfo
{
    public string code;  
    public string name;
    public string body;
    public Vector3 cameraPosition;
}

public class RG_ButtonNav : MonoBehaviour
{
    private string rocketToShow;
    public TextMeshProUGUI nameTextUI;
    public TextMeshProUGUI bodyTextUI;

    private Dictionary<string, RocketInfo> rocketInfoDict;
    private List<string> rocketOrder;
    private int currentIndex = 0;

    public Camera mainCamera; // Assign in Inspector
    private float cameraMoveSpeed = 50f; // Adjustable speed

    void Start()
    {
        RocketInfo[] rocketInfoArray = new RocketInfo[]
        {
            // todo, add camera position
            new RocketInfo { code = "v2", name = "V-2", body = "Info about V-2 rocket.", cameraPosition = new Vector3(-79.1f, 7.6f, 28.7f)},
            new RocketInfo { code = "sat", name = "Saturn V", body = "Info about Saturn V.", cameraPosition = new Vector3(19.7f, 51.6f, -57.7f)},
            new RocketInfo { code = "f9", name = "Falcon 9", body = "Info about Falcon 9.", cameraPosition = new Vector3(66.5f, 33f, -21.1f)},
            new RocketInfo { code = "fh", name = "Falcon Heavy", body = "Info about Falcon Heavy.", cameraPosition = new Vector3(121.9f, 33f, -21.1f)}
        };

        // Convert array to dictionary for fast lookup
        rocketInfoDict = new Dictionary<string, RocketInfo>();
        rocketOrder = new List<string>();

        foreach (var rocket in rocketInfoArray)
        {
            rocketInfoDict.Add(rocket.code, rocket);
            rocketOrder.Add(rocket.code);
        }

        // Initialize with the first rocket
        rocketToShow = rocketOrder[currentIndex];
        NavigateToRocket();
    }

    public void NavigateToRocket()
    {
        if (rocketInfoDict.ContainsKey(rocketToShow))
        {
            var rocket = rocketInfoDict[rocketToShow];
            nameTextUI.text = rocket.name;
            bodyTextUI.text = rocket.body;

            // Move camera smoothly
            StopAllCoroutines(); // Stop any existing movement coroutine
            StartCoroutine(MoveCamera(rocket.cameraPosition));
        }
        else
        {
            Debug.LogWarning($"Rocket code '{rocketToShow}' not found!");
        }
    }

    IEnumerator MoveCamera(Vector3 targetPosition)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(
                mainCamera.transform.position,
                targetPosition,
                cameraMoveSpeed * Time.deltaTime
            );
            yield return null;
        }
        mainCamera.transform.position = targetPosition;
    }

    public void ShowNextRocket()
    {
        currentIndex++;
        if (currentIndex >= rocketOrder.Count)
            currentIndex = 0; // Loop back to start

        rocketToShow = rocketOrder[currentIndex];
        NavigateToRocket();
    }

    public void ShowPreviousRocket()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = rocketOrder.Count - 1; // Loop to last item

        rocketToShow = rocketOrder[currentIndex];
        NavigateToRocket();
}
}
