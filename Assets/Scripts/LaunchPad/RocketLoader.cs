using UnityEngine;
using System.Linq;

[System.Serializable]
public struct RocketEntry
{
    public string rocketName;
    public GameObject rocketPrefab;
}

public class RocketLoader : MonoBehaviour
{
    [Tooltip("Fill this with name-to-prefab pairs in the inspector")]
    public RocketEntry[] rocketMappings;
    public GameObject parentObject;

    private Vector3 spawnPosition;
    private Vector3 spawnEulerAngles; 
    private Vector3 spawnScale;

    void Awake()
    {
        string choice = RocketSelection.SelectedRocketName;
        
        // USE CUSTOM ROCKET
        if (string.IsNullOrEmpty(choice))
        {
            // IMPORTING BUILD YOUR OWN
            GameObject rocket = GameObject.Find("Rocket");
            if (rocket != null)
            {
                // Set world-space position to (593.1, 12.5, 443.8)
                rocket.transform.position = new Vector3(593.1f, 12.5f, 443.8f);
                // Set rotation to (0,0,0) — identical to Quaternion.identity here
                rocket.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                // Match the inspector’s uniform scale of 4.8832
                rocket.transform.localScale = new Vector3(4.8832f, 4.8832f, 4.8832f);
                rocket.transform.SetParent(parentObject.transform);


                Transform bottom = GameObject.FindWithTag("BottomStage")?.transform;
                Transform enginesT = bottom.Find("engines");
                 enginesT.localRotation = Quaternion.identity;
            }else{
                choice = "saturn";
            }
        }

        // USE PRE BUILT ROCKET
        switch (choice)
        {
            case "f_9":
                spawnPosition    = new Vector3(584.4f,  13.2f, 444.07f);
                spawnEulerAngles = new Vector3(0f, 142.7f, 0f);
                spawnScale       = Vector3.one * 136.77f;
                break;

            case "f_heavy":
                spawnPosition    = new Vector3(613f,  83.3f, 480f);
                spawnEulerAngles = new Vector3(0f, 0f, 0f);
                spawnScale       = Vector3.one * 0.3f;
                break;

            case "saturn":
                spawnPosition    = new Vector3(584.2f, 14.4f, 447f);
                spawnEulerAngles = new Vector3(0f, 00f, 0f);
                spawnScale       = Vector3.one * 1.01f;
                break;

            case "v2":
                spawnPosition    = new Vector3(586.13f, 34f, 441.64f);
                spawnEulerAngles = new Vector3(-90f, 0f, -35.3f);
                spawnScale       = Vector3.one * 3.03f;
                break;

            default:
                return;
        }

        // find its prefab
        var entry = rocketMappings.FirstOrDefault(e => e.rocketName == choice);
        
        // instantiate at the spawn point
        GameObject rocketInstance = Instantiate(
            entry.rocketPrefab, 
            spawnPosition, 
            Quaternion.Euler(spawnEulerAngles)
        );
        rocketInstance.transform.SetParent(parentObject.transform);
        rocketInstance.transform.localScale = spawnScale;
    }
}
