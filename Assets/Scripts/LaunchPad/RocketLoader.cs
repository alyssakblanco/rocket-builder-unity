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

    private Vector3 spawnPosition;
    private Vector3 spawnEulerAngles; 

    void Awake()
    {
        string choice = RocketSelection.SelectedRocketName;
        
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
            }
        }

        if (choice == "f_9"){
            spawnPosition = new Vector3(593.1f, 12.5f, 443.8f);
            spawnEulerAngles = new Vector3(0f,   180f,  0f); 
        }else if (choice == "f_heavy"){
            spawnPosition = new Vector3(593.1f, 12.5f, 443.8f);
            spawnEulerAngles = new Vector3(0f,   180f,  0f); 
        }else if (choice == "saturn"){
            spawnPosition = new Vector3(583.1f, 12.5f, 443.8f);
            spawnEulerAngles = new Vector3(0f,   180f,  0f); 
        }else if (choice == "v2"){
            spawnPosition = new Vector3(593.1f, 12.5f, 443.8f);
            spawnEulerAngles = new Vector3(0f,   180f,  0f); 
        }

        // find its prefab
        var entry = rocketMappings.FirstOrDefault(e => e.rocketName == choice);

        Quaternion spawnRotation = Quaternion.Euler(spawnEulerAngles);
        // instantiate at the spawn point
        Instantiate(entry.rocketPrefab, spawnPosition, spawnRotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
