using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class BuilderController : MonoBehaviour
{
    public enum RocketPart { Stage, Nose, Propellant, Control }

    public static readonly string[] NoseCones = new[]
    {
        "A smooth, pointed design—like the tip of a sharpened pencil. It slices through the air with very little resistance, helping your rocket go faster and fly straight.",
        "A rounded, dome-like tip. It’s tougher against heat but makes more air push against it, so the rocket slows down a bit more.",
        "A cover that protects equipment or satellites. During flight, it can open or split apart to let the cargo pop out. It usually looks like a cone or a modified cone shape."
    };

    public static readonly string[] Propellants = new[]
    {
        "Fuel and oxidizer are mixed into one solid block. You light it once, and it burns until it’s gone. It delivers a quick, powerful push—perfect for fast launches—but you can’t turn it off or change its power level.",        
        "Fuel and oxidizer stay in liquid form until they reach the engine. Pumps push them in, so you can throttle the engine up or down. This is great if you need to adjust speed or turn in space.",
    };

    public static readonly string[] Controls = new[]
    {
        "Rocket fins help keep your rocket steady in flight by keeping it pointed in the right direction and on the right path—but because they need air to push against, they don’t work once the rocket reaches the vacuum of space.",
        "The entire engine can tilt back and forth, like swiveling a garden hose. By pointing the thrust a little left or right, you change the rocket’s direction. Works both inside the atmosphere and in space.",
    };

    public static readonly string[] Stages = new[]
    {
        "All in one piece—engine, fuel, and payload stay together. Usually used for short, suborbital trips (it goes up and comes right back down).",
        "The first stage blasts off and then falls away; the second stage takes over to reach orbit. This is the kind that can put satellites around Earth.",
        "After the second stage drops off, a third stage fires to push the rocket beyond Earth orbit. It’s powerful enough to head toward the Moon or travel to other planets."
    };

    [Header("Bottom Stage")]
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

    private bool isPayload = false;
    // gimbal animation
    private bool gimbalShouldSwing = false;
    public float swingSpeed = 50f;
    public float swingAngle = 10f;
    private float swingTimer = 0f;

    [Header("UI Content")]
    public TextMeshProUGUI noseDesc;
    public TextMeshProUGUI propellantDesc;
    public TextMeshProUGUI controlDesc;
    public TextMeshProUGUI stagesDesc;
    public TextMeshProUGUI noseTitle;
    public TextMeshProUGUI propellantTitle;
    public TextMeshProUGUI controlTitle;
    public TextMeshProUGUI stagesTitle;
    public TextMeshProUGUI noseHeader;
    public TextMeshProUGUI propellantHeader;
    public TextMeshProUGUI controlHeader;
    public TextMeshProUGUI stagesHeader;

    void Start()
    {
        // 1) INITIALIZE DEFAULTS
        currentBuild[RocketPart.Stage]      = "1";
        currentBuild[RocketPart.Nose]       = "ogive";
        currentBuild[RocketPart.Propellant] = "solid";
        currentBuild[RocketPart.Control]    = "fins";

        SetCostAndWeight();

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
        // move engines
        if (gimbalShouldSwing && gimbalPrefab != null)
        {
            swingTimer += Time.deltaTime * swingSpeed;
            float z = Mathf.Sin(swingTimer) * swingAngle;
            gimbalPrefab.transform.localEulerAngles  = new Vector3(0f, 0f, z);
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
        UpdateContent("stages", sel);
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

        SetCostAndWeight();
    }

    private void UpdateNose(string sel)
    {
        UpdateContent("nose", sel);
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
        nose.transform.SetParent(transform, true);  

        if(sel == "payload"){
            isPayload = true;
        }else{
           isPayload = false; 
        }

        SetCostAndWeight();
    }

    private void UpdatePropellant(string selection)
    {
        UpdateContent("propellant", selection);
        if (selection != "solid" && selection != "liquid")
        {
            Debug.LogError($"Invalid propellant selection: {selection}");
            return;
        }
        currentBuild[RocketPart.Propellant] = selection;
        
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

        SetCostAndWeight();
    }

    private void UpdateControl(string selection)
    {
        UpdateContent("control", selection);
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

        SetCostAndWeight();
    }

    // LAUNCH PAD PREP
    public void AssembleRocket()
    {
        Transform bottom = GameObject.FindWithTag("BottomStage")?.transform;
        Transform middle = GameObject.FindWithTag("MiddleStage")?.transform;
        Transform top    = GameObject.FindWithTag("TopStage")?.transform;
        Transform nose   = GameObject.FindWithTag("NoseCone")?.transform;

        var rocket = new GameObject("Rocket"){ tag = "Rocket" };
        bottom.SetParent(rocket.transform, true);
        if (middle != null) middle.SetParent(rocket.transform, true);
        if (top    != null) top.SetParent(rocket.transform, true);
        if (nose   != null) nose.SetParent(rocket.transform, true);
        
        DontDestroyOnLoad(rocket);
    }

    public void GoToLaunchPad()
    {
        AssembleRocket();
        RocketSelection.SelectedRocketName = null;
        SceneManager.LoadScene("launch_pad");
    }

    // UI Control
    public void UpdateContent(string section, string sel){
        if(section == "nose"){
            if(sel == "ogive"){
                noseTitle.text = "Ogive";
                noseHeader.text = "Ogive";
                noseDesc.text = NoseCones[0];
            }
            if(sel == "blunt"){
                noseTitle.text = "Blunt";
                noseHeader.text = "Blunt";
                noseDesc.text = NoseCones[1];
            }
            if(sel == "payload"){
                noseTitle.text = "Payload";
                noseHeader.text = "Payload";
                noseDesc.text = NoseCones[2];
            }
        }
        if(section == "propellant"){
            if(sel == "solid"){
                propellantTitle.text = "Solid";
                propellantHeader.text = "Solid";
                propellantDesc.text = Propellants[0];
            }
            if(sel == "liquid"){
                propellantTitle.text = "Liquid";
                propellantHeader.text = "Liquid";
                propellantDesc.text = Propellants[1];
            }
        }
        if(section == "control"){
            if(sel == "fins"){
                controlTitle.text = "Movable Fins";
                controlHeader.text = "Movable Fins";
                controlDesc.text = Controls[0];
            }
            if(sel == "gimbal"){
                controlTitle.text = "Gimbaled Engines";
                controlHeader.text = "Gimbaled Engines";
                controlDesc.text = Controls[1];
            }
        }
        if(section == "stages"){
            if(sel == "1"){
                stagesTitle.text = "1";
                stagesHeader.text = "One Stage";
                stagesDesc.text = Stages[0];
            }
            if(sel == "2"){
                stagesTitle.text = "2";
                stagesHeader.text = "Two Stages";
                stagesDesc.text = Stages[1];
            }
            if(sel == "3"){
                stagesTitle.text = "3";
                stagesHeader.text = "Three Stages";
                stagesDesc.text = Stages[2];
            }
        }
    }

    // COST WIEGHT STUFF
    [Header("Cost & Weight")]
    public Color32 activeColor = new Color32(0xEE, 0x9B, 0x00, 0xFF);
    public Color inactiveColor = Color.white;
    public Image[] money;
    public Image[] anvil;

    public void SetCostAndWeight(){
        float cost = 0;
        float weight = 0;

        if(currentBuild[RocketPart.Nose] == "ogive"){
            cost += 1;
            weight += 1;
        }
        if(currentBuild[RocketPart.Nose] == "blunt"){
            cost += 1;
            weight += 1;
        }
        if(currentBuild[RocketPart.Nose] == "payload"){
            cost += 2;
            weight += 3;
        }

        if(currentBuild[RocketPart.Propellant] == "solid"){
            cost += 1;
            weight += 2;
        }
        if(currentBuild[RocketPart.Propellant] == "liquid"){
            cost += 2;
            weight += 1;
        }

        if(currentBuild[RocketPart.Control] == "gimbal"){
            cost += 2;
            weight += 2;
        }
        if(currentBuild[RocketPart.Control] == "fins"){
            cost += 1;
            weight += 1;
        }

        if(currentBuild[RocketPart.Stage] == "1"){
            cost += 1;
            weight += 1;
        }
        if(currentBuild[RocketPart.Stage] == "2"){
            cost += 2;
            weight += 2;
        }
        if(currentBuild[RocketPart.Stage] == "3"){
            cost += 3;
            weight += 3;
        }

        cost = Mathf.Ceil(cost/2);
        for (int i = 0; i < money.Length; i++)
        {
            var img = money[i];
            img.color = (i < cost) ? activeColor : inactiveColor;
        }

        weight = Mathf.Ceil(weight/2);
        for (int i = 0; i < anvil.Length; i++)
        {
            var img = anvil[i];
            img.color = (i < weight) ? activeColor : inactiveColor;
        }
    }
}
