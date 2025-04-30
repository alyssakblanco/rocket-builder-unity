using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
using System.Collections.Generic;

public class BuilderSectionSelection : MonoBehaviour
{
    // Section Canvases
    public GameObject noseSection;
    public GameObject propellantSection;
    public GameObject controlSection;
    public GameObject stagesSection; 

    // UI Buttons for each section
    public Button noseButton;
    public Button propellantButton;
    public Button controlButton;
    public Button stagesButton;

    private Button[] sectionButtons;

    // camera
    public Camera mainCamera;
    private float cameraMoveSpeed = 50f;

    // Colors for active/inactive
    private readonly Color activeColor = Color.white;
    private readonly Color inactiveColor = new Color(0.75f, 0.75f, 0.75f);

    // builder controller
    [SerializeField] private GameObject targetObject;
    private BuilderController _builderController;
    void Awake()
    {
        _builderController = targetObject.GetComponent<BuilderController>();
        if (_builderController == null)
            Debug.LogError("No BuilderController on " + targetObject.name);

        sectionButtons = new Button[] {
            noseButton,
            propellantButton,
            controlButton,
            stagesButton
        };

        UpdateGroupVisuals(0);
    }

    public void SetCurrentSection(string selection)
    {
        var build = _builderController.GetCurrentBuild();
        if (build.TryGetValue(BuilderController.RocketPart.Stage, out var stage))
        {
            Debug.Log("Current Stage selection: " + stage);
        }
        else
        {
            Debug.LogWarning("Stage not set yet.");
        }

        // turn off all sections
        noseSection.SetActive(false);
        propellantSection.SetActive(false);
        controlSection.SetActive(false);
        stagesSection.SetActive(false);

        // enable requested one
        switch (selection)
        {
            case "Nose":
                UpdateGroupVisuals(0);
                noseSection.SetActive(true);
                break;
            case "Propellant":
                UpdateGroupVisuals(1);
                StartCoroutine(MoveCamera("1"));
                propellantSection.SetActive(true);
                break;
            case "Control":
                UpdateGroupVisuals(2);
                StartCoroutine(MoveCamera("1"));
                controlSection.SetActive(true);
                break;
            case "Stages":
                UpdateGroupVisuals(3);
                StartCoroutine(MoveCamera(stage));
                stagesSection.SetActive(true);
                break;
            default:
                Debug.LogWarning($"[SectionController] Unknown section «{selection}»");
                break;
        }
    }

    // CAMEREA CONTROL
    public IEnumerator MoveCamera(string stages)
    {
        Vector3 targetPosition;

        if(stages == "3"){
            targetPosition = new Vector3(-17.45f, 9.68f, -24.33f);
        }else if(stages == "2"){
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

    // update button colors
    void UpdateGroupVisuals(int activeIndex)
        {
            for (int i = 0; i < sectionButtons.Length; i++)
            {
                var img = sectionButtons[i].GetComponent<Image>();
                if (img != null)
                    img.color = (i == activeIndex ? activeColor : inactiveColor);
            }
        }
}