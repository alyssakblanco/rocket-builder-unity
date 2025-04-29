using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum RocketPart
{
    Stage,
    Nose,
    Propellant,
    Control
}

public class BuilderController : MonoBehaviour
{
    public enum RocketPart { Stage, Nose, Propellant, Control }

    [Header("Scene Instances (for manual defaults)")]
    public GameObject bottomStageInstance;   // drag the scene object here

    [Header("Stage Prefabs")]
    public GameObject bottomStagePrefab;       // your 1st‑stage model
    public GameObject[] stagePrefabs;          // [0]=middle‑stage, [1]=top‑stage

    [Header("Nose Cone Prefabs")]
    public GameObject[] nosePrefabs;           // [0]=ogive, [1]=blunt, [2]=payload

    [Header("Camera")]
    public Camera mainCamera;           // [0]=ogive, [1]=blunt, [2]=payload

    // propellant images
    public Image propellantImage; // Drag your UI Image component here in the Inspector
    public Sprite solidPropellantSprite; // Drag your "solid" sprite here
    public Sprite liquidPropellantSprite; // Drag your "liquid" sprite here

    [Header("Control Prefabs")]
    public GameObject finsPrefab;
    public GameObject gimbalPrefab;

    // Tracks current selections
    private Dictionary<RocketPart,string> currentBuild = new Dictionary<RocketPart,string>();
    public Dictionary<RocketPart, string> GetCurrentBuild()
    {
        return currentBuild;
    }

    // Instantiated GameObjects
    private GameObject bottomStage, middleStage, topStage, nose, control;

    private float cameraMoveSpeed = 50f;

    // gimbal animation
    private bool gimbalShouldSwing = false;
    public float swingSpeed = 50f;
    public float swingAngle = 10f;
    private float swingTimer = 0f;

    void Start()
    {
        // 1) INITIALIZE DEFAULTS
        currentBuild[RocketPart.Stage]      = "1";
        currentBuild[RocketPart.Nose]       = "ogive";
        currentBuild[RocketPart.Propellant] = "solid";
        currentBuild[RocketPart.Control]    = "fins";

        // Spawn bottom stage
        // 1) set up your bottomStage reference
        bottomStage = bottomStageInstance;
        // 2) find the anchor transform inside it
        Transform bottomAnchor = bottomStage.transform.Find("anchor");
        if (bottomAnchor == null)
        {
            Debug.LogError("Could not find an 'anchor' child under bottomStage!");
            return;
        }
        // 3) instantiate the default nose (ogive) at that spot,
        //    parented under the BuilderController (or leave un‑parented— your choice)
        nose = Instantiate(
            nosePrefabs[0],                  // prefab to spawn
            bottomAnchor.position,           // world‑space pos
            bottomAnchor.rotation,           // world‑space rot
            transform                        // parent: this BuilderController
        );

        propellantImage.sprite = solidPropellantSprite;
    }

    void Update()
    {
        if (gimbalShouldSwing && gimbalPrefab != null)
        {
            swingTimer += Time.deltaTime * swingSpeed;
            float z = Mathf.Sin(swingTimer) * swingAngle;
            gimbalPrefab.transform.localRotation = Quaternion.Euler(0f, 0f, z);
        }
    }

    // ——— PUBLIC UI CALLS ———
    public void SelectStage(string s)      => UpdateStage(s);
    public void SelectNose(string s)       => UpdateNose(s);
    public void SelectPropellant(string s) => UpdatePropellant(s);
    public void SelectControl(string s)    => UpdateControl(s);

    // CAMEREA CONTROL
    public IEnumerator MoveCamera(int stages)
    {
        Vector3 targetPosition;

        if(stages == 3){
            targetPosition = new Vector3(-17.45f, 9.68f, -24.33f);
        }else if(stages == 2){
            targetPosition = new Vector3(-15.25f, 8.3f, -21.38f);
        }else{
            targetPosition = new Vector3(-13.7f, 6.74f, -18.11f);
        }

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

    // ——— CORE LOGIC ———
    private void UpdateStage(string sel)
    {
        int count = int.Parse(sel);
        currentBuild[RocketPart.Stage] = sel;

        StartCoroutine(MoveCamera(count));

        // destroy existing
        if (middleStage != null) Destroy(middleStage);
        if (topStage    != null) Destroy(topStage);

        // 1→ just bottomStage
        // 2→ add middleStage at bottomAnchor
        if (count >= 2)
        {
            var bottomAnchor = bottomStage.transform.Find("anchor");
            middleStage = Instantiate(stagePrefabs[0],
                                    bottomAnchor.position,
                                    bottomAnchor.rotation);
            middleStage.transform.SetParent(transform, true); // flat
        }

        // 3→ add topStage at middleAnchor
        if (count >= 3)
        {
            var midAnchor = middleStage.transform.Find("anchor");
            topStage = Instantiate(stagePrefabs[1],
                                midAnchor.position,
                                midAnchor.rotation);
            topStage.transform.SetParent(transform, true);
        }

        // finally, re‑position your nose up top
        UpdateNose(currentBuild[RocketPart.Nose]);
    }

    private void UpdateNose(string sel)
    {
        currentBuild[RocketPart.Nose] = sel;
        int idx = sel == "ogive" ? 0 : sel == "blunt" ? 1 : 2;
        if (nose != null) Destroy(nose);

        // pick the right anchor
        Transform anchor;
        int stageCount = int.Parse(currentBuild[RocketPart.Stage]);
        if      (stageCount >= 3 && topStage    != null) anchor = topStage   .transform.Find("anchor");
        else if (stageCount >= 2 && middleStage != null) anchor = middleStage.transform.Find("anchor");
        else                                             anchor = bottomStage.transform.Find("anchor");

        nose = Instantiate(nosePrefabs[idx],
                        anchor.position,
                        anchor.rotation);
        nose.transform.SetParent(transform, true);  // flat again
    }

    private void UpdatePropellant(string selection)
    {
        if (selection != "solid" && selection != "liquid")
        {
            Debug.LogError($"Invalid propellant selection: {selection}");
            return;
        }
        currentBuild[RocketPart.Propellant] = selection;
        Debug.Log($"Propellant set to: {selection}");
        
        // Update the propellant image
        if (propellantImage != null)
        {
            if (selection == "solid" && solidPropellantSprite != null)
            {
                propellantImage.sprite = solidPropellantSprite;
            }
            else if (selection == "liquid" && liquidPropellantSprite != null)
            {
                propellantImage.sprite = liquidPropellantSprite;
            }
        }
        else
        {
            Debug.LogWarning("Propellant image is not assigned.");
        }
    }

    private void UpdateControl(string selection)
    {
        currentBuild[RocketPart.Control] = selection;

        // Toggle fins
        finsPrefab.SetActive(selection == "fins");

        // Control gimbal animation state
        gimbalShouldSwing = (selection == "gimbal");

        // Reset gimbal to neutral position if it's not supposed to swing
        if (!gimbalShouldSwing && gimbalPrefab != null)
        {
            gimbalPrefab.transform.localRotation = Quaternion.identity;
        }
    }

    // LAUNCH PAD PREP
    public void AssembleRocket()
    {
        Transform bottom = GameObject.FindWithTag("BottomStage")?.transform;
        Transform middle = GameObject.FindWithTag("MiddleStage")?.transform;
        Transform top    = GameObject.FindWithTag("TopStage")?.transform;
        Transform nose   = GameObject.FindWithTag("NoseCone")?.transform;

        var rocket = new GameObject("Rocket");
        bottom.SetParent(rocket.transform, true);
        if (middle != null) middle.SetParent(rocket.transform, true);
        if (top    != null) top.SetParent(rocket.transform, true);
        if (nose   != null) nose.SetParent(rocket.transform, true);
        
        DontDestroyOnLoad(rocket);
    }

    public void GoToLaunchPad()
    {
        AssembleRocket();
        SceneManager.LoadScene("launch_pad");
    }
}
