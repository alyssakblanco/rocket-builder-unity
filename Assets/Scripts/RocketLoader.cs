using UnityEngine;

public class RocketLoader : MonoBehaviour
{
    void Awake()
    {
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
