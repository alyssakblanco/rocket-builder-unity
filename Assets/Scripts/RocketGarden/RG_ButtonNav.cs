using UnityEngine;
using TMPro;
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine.SceneManagement;

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
    public GameObject mainCanvas;

    private string rocketToShow;
    public TextMeshProUGUI nameTextUI;
    public TextMeshProUGUI bodyTextUI;

    private Dictionary<string, RocketInfo> rocketInfoDict;
    private List<string> rocketOrder;
    private int currentIndex = 0;

    public Camera mainCamera; // Assign in Inspector
    public float cameraMoveSpeed; // Adjustable speed

    private string rocketName;

    void Start()
    {
        RocketInfo[] rocketInfoArray = new RocketInfo[]
        {
            new RocketInfo
            {
                code = "v2",
                name = "V-2",
                body = @"
<b>Country of origin:</b> Germany
<b>Years in use:</b> 1944 - 1952
<b>Number of stages:</b> 1

<b>Fun facts:</b>
• The V2 rocket was the <u>first artificial object to travel into space</u>!

• Developed during the Second World War as the world’s first long-range ballistic missile.",
                cameraPosition = new Vector3(-97.1f, 7.2f, 62.8f)
            },
            new RocketInfo
            {
                code = "f_heavy",
                name = "Falcon Heavy",
                body = @"
<b>Country of origin:</b> USA – SpaceX
<b>Years in use:</b> 2018 - today
<b>Number of stages:</b> 2.5
<b>Used for:</b> Carry cargo into Earth orbit and beyond

<b>Fun facts:</b>
• Composed of three reusable Falcon 9 nine-engine cores together, generating more than 5 million pounds of thrust at liftoff, equal to approximately <b>18 airplanes</b>!

• Falcon Heavy is called a ""2.5-stage rocket"" because its side boosters drop off first while the center core keeps firing, making it feel like a bonus half stage before the second stage takes over.",
                cameraPosition = new Vector3(37.7f, 38.4f, 119.3f)
            },
            new RocketInfo
            {
                code = "f_9",
                name = "Falcon 9",
                body = @"
<b>Country of origin:</b> USA – SpaceX
<b>Years in use:</b> 2010 - today
<b>Number of stages:</b> 2
<b>Mission:</b> Reliable and safe transport of people and payloads into Earth orbit and beyond

<b>Fun facts:</b>
• The world’s first <u>reusable rocket</u>!

• The first stage carries the second stage and payload to the target speed and altitude, after which the second stage accelerates the payload to its target orbit. The first stage booster is capable of landing vertically so it can be reused.",
                cameraPosition = new Vector3(-4.3f, 38.4f, 119.3f)
            },
            new RocketInfo
            {
                code = "saturn",
                name = "Saturn V",
                body = @"
<b>Country of origin:</b> USA – NASA
<b>Years in use:</b> 1967 - 1973
<b>Number of stages:</b> 3
<b>Mission:</b> Apollo Program – human exploration of the Moon

<b>Fun facts:</b>
• The <b>only</b> launch vehicle to have carried humans beyond low Earth orbit (LEO)!

• Used for nine crewed flights to the Moon and to launch Skylab (the first American space station).",
                cameraPosition = new Vector3(-89.7f, 59.5f, 163.7f)
            }
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
        StartCoroutine(InitialRoutine());
    }

    private IEnumerator InitialRoutine()
    {
        yield return StartCoroutine(InitalSceneSetup(rocketInfoDict[rocketToShow]));
    }
    
    private IEnumerator InitalSceneSetup(RocketInfo rocket)
    {
        // 1) update text immediately
        nameTextUI.text = rocket.name;
        bodyTextUI.text = rocket.body;

        // 2) wait until camera has moved into position
        yield return StartCoroutine(MoveCamera(rocket.cameraPosition, 100));

        // 3) only now activate the canvas (once!)
        if (!mainCanvas.activeSelf)
            mainCanvas.SetActive(true);
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
            StartCoroutine(MoveCamera(rocket.cameraPosition, cameraMoveSpeed));
        }
        if (!mainCanvas.activeSelf){
            mainCanvas.SetActive(true);
        }
    }

    IEnumerator MoveCamera(Vector3 targetPosition, float moveSpeed)
    {
        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
        {
            mainCamera.transform.position = Vector3.MoveTowards(
                mainCamera.transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
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

    public void MoveToLaunch()
    {
        RocketSelection.SelectedRocketName = rocketToShow;
        SceneManager.LoadScene("launch_pad");
    }

}
