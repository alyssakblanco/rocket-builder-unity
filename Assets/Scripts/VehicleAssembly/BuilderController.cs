using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using System.Text.RegularExpressions;

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

    // Instantiated GameObjects
    private GameObject bottomStage, middleStage, topStage, nose, control;

    private float cameraMoveSpeed = 50f;

    // gimbal animation
    private bool gimbalShouldSwing = false;
    public float swingSpeed = 50f;
    public float swingAngle = 10f;
    private float swingTimer = 0f;

    [Header("Section Titles")]
    public TextMeshProUGUI noseTitle;
    public TextMeshProUGUI propellantTitle;
    public TextMeshProUGUI controlTitle;
    public TextMeshProUGUI stagesTitle;

    [Header("Option Headers")]
    public TextMeshProUGUI noseHeader;
    public TextMeshProUGUI propellantHeader;
    public TextMeshProUGUI controlHeader;
    public TextMeshProUGUI stagesHeader;
    
    [Header("Option Descriptions")]
    public TextMeshProUGUI noseDesc;
    public TextMeshProUGUI propellantDesc;
    public TextMeshProUGUI controlDesc;
    public TextMeshProUGUI stagesDesc;

    [Header("Payload Fairing Settings")]
    private bool isPayload = false;                  // toggle on when you want the fairings to move
    public float payloadSwingSpeed = 1f;    // how fast they oscillate
    public float payloadSeparation = 50f;  // max offset on each side

    private float payloadTimer = 0f;
    private Vector3 half1StartPos;
    private Vector3 half2StartPos;
    private Transform fairingHalf1;        // found at runtime
    private Transform fairingHalf2;        // found at runtime
    public Vector3 offsetPosition; // Set in Inspector or code
    public float moveSpeed = 1f;

    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private bool movingToTarget = true;
    private bool isMoving = false;

    void Start()
    {
        if(!GameData.keepCurrentSelections){
            GameData.currentBuild[BuilderController.RocketPart.Stage]      = "1";
            GameData.currentBuild[BuilderController.RocketPart.Nose]       = "ogive";
            GameData.currentBuild[BuilderController.RocketPart.Propellant] = "solid";
            GameData.currentBuild[BuilderController.RocketPart.Control]    = "fins";
        }
        
        SetCostAndWeight();

        // Spawn bottom stage
        bottomStage = bottomStageInstance;
        Transform bottomAnchor = bottomStage.transform.Find("anchor");

        // Spawn the nose according to the saved selection
        string noseSel = GameData.currentBuild[BuilderController.RocketPart.Nose];
        int noseIdx;
        switch (noseSel)
        {
            case "ogive":   noseIdx = 0; break;
            case "blunt":   noseIdx = 1; break;
            case "payload": noseIdx = 2; break;
            default:        noseIdx = 0; break;
        }
        nose = Instantiate(
            nosePrefabs[noseIdx],
            bottomAnchor.position,
            bottomAnchor.rotation,
            transform
        );

        UpdateNose(GameData.currentBuild[BuilderController.RocketPart.Nose]);
        UpdateStage(GameData.currentBuild[BuilderController.RocketPart.Stage]);
        UpdatePropellant(GameData.currentBuild[BuilderController.RocketPart.Propellant]);
        UpdateControl(GameData.currentBuild[BuilderController.RocketPart.Control]);

        // Apply the propellant sprite
        string propSel = GameData.currentBuild[BuilderController.RocketPart.Propellant].ToLower();
        propellantImage.sprite = (propSel == "liquid")
            ? liquidPropellantSprite
            : solidPropellantSprite;
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

        if (isPayload && !isMoving)
        {
            StartCoroutine(MoveFairing());
        }
    }

    private IEnumerator MoveFairing()
    {
        isMoving = true;

        Vector3 destination = movingToTarget ? targetPosition : originalPosition;

        while (fairingHalf2 != null && Vector3.Distance(fairingHalf2.localPosition, destination) > 0.01f)
        {
            fairingHalf2.localPosition = Vector3.MoveTowards(
                fairingHalf2.localPosition,
                destination,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        if (fairingHalf2 != null)
        {
            fairingHalf2.localPosition = destination;
        }

        yield return new WaitForSeconds(2f);
        movingToTarget = !movingToTarget;
        isMoving = false;
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
        GameData.currentBuild[BuilderController.RocketPart.Stage] = sel;

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
        UpdateNose(GameData.currentBuild[BuilderController.RocketPart.Nose]);

        SetCostAndWeight();
    }

    private void UpdateNose(string rawSel)
    {
        var sel = rawSel.Trim().ToLowerInvariant();
        if(sel == "payload"){
            isPayload = true;
        }else{
           isPayload = false; 
        }
        UpdateContent("nose", sel);
        GameData.currentBuild[BuilderController.RocketPart.Nose] = sel;
        int idx;
        switch(sel)
        {
        case "ogive":
            idx = 0;
            break;
        case "blunt":
            idx = 1;
            break;
        case "payload":
            idx = 2;
            break;
        default:
            idx = 0;
            break;
        }
        if (nose != null) Destroy(nose);

        // pick the right anchor
        Transform anchor;
        int stageCount = int.Parse(GameData.currentBuild[BuilderController.RocketPart.Stage]);
        if      (stageCount >= 3 && topStage    != null) anchor = topStage   .transform.Find("anchor");
        else if (stageCount >= 2 && middleStage != null) anchor = middleStage.transform.Find("anchor");
        else                                             anchor = bottomStage.transform.Find("anchor");

        nose = Instantiate(nosePrefabs[idx],
                        anchor.position,
                        anchor.rotation);
        nose.transform.SetParent(transform, true);  

        if(sel == "payload"){
            var all = transform.GetComponentsInChildren<Transform>(includeInactive: true);
            fairingHalf1 = all.FirstOrDefault(t => t.name == "fairing_half_l");
            fairingHalf2 = all.FirstOrDefault(t => t.name == "fairing_half_r");
            
            originalPosition = fairingHalf2.localPosition;
            targetPosition = originalPosition + offsetPosition;

            if (fairingHalf1 == null || fairingHalf2 == null)
                Debug.LogWarning("Could not locate one or both fairing halves in the scene!");
        }

        SetCostAndWeight();
    }

    private void UpdatePropellant(string selection)
    {
        UpdateContent("propellant", selection);
        GameData.currentBuild[BuilderController.RocketPart.Propellant] = selection;
        
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

        SetCostAndWeight();
    }

    private void UpdateControl(string selection)
    {
        var cleaned = selection
            .Replace("\r", "")
            .Replace("\n", " ")
            .Trim();

        var m = Regex.Match(cleaned, "(?i)gimbal");
        var norm = m.Success
            ? m.Value.ToLowerInvariant()   // “gimbal”
            : cleaned.ToLowerInvariant();  // fallback to the full cleaned string

        if(norm == "gimbal"){
            selection = norm;
        }

        UpdateContent("control", selection);
        GameData.currentBuild[BuilderController.RocketPart.Control] = selection;

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

        gimbalPrefab.transform.localEulerAngles  = new Vector3(0f, 0f, 0f);

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

        if(GameData.currentBuild[BuilderController.RocketPart.Nose] == "ogive"){
            cost += 1;
            weight += 1;
        }
        if(GameData.currentBuild[BuilderController.RocketPart.Nose] == "blunt"){
            cost += 1;
            weight += 1;
        }
        if(GameData.currentBuild[BuilderController.RocketPart.Nose] == "payload"){
            cost += 2;
            weight += 3;
        }

        if(GameData.currentBuild[BuilderController.RocketPart.Propellant] == "solid"){
            cost += 1;
            weight += 2;
        }
        if(GameData.currentBuild[BuilderController.RocketPart.Propellant] == "liquid"){
            cost += 2;
            weight += 1;
        }

        if(GameData.currentBuild[BuilderController.RocketPart.Control] == "gimbal"){
            cost += 2;
            weight += 2;
        }
        if(GameData.currentBuild[BuilderController.RocketPart.Control] == "fins"){
            cost += 1;
            weight += 1;
        }

        if(GameData.currentBuild[BuilderController.RocketPart.Stage] == "1"){
            cost += 1;
            weight += 1;
        }
        if(GameData.currentBuild[BuilderController.RocketPart.Stage] == "2"){
            cost += 2;
            weight += 2;
        }
        if(GameData.currentBuild[BuilderController.RocketPart.Stage] == "3"){
            cost += 3;
            weight += 3;
        }

        cost = Mathf.Ceil(cost/2);
        GameData.currentCost = cost;
        for (int i = 0; i < money.Length; i++)
        {
            var img = money[i];
            img.color = (i < cost) ? activeColor : inactiveColor;
        }

        weight = Mathf.Ceil(weight/2);
        GameData.currentWeight = weight;
        for (int i = 0; i < anvil.Length; i++)
        {
            var img = anvil[i];
            img.color = (i < weight) ? activeColor : inactiveColor;
        }
    }
}
