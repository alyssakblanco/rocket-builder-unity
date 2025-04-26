using UnityEngine;
using UnityEngine.UI;

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

    // Colors for active/inactive
    private readonly Color activeColor = Color.white;
    private readonly Color inactiveColor = Color.gray;

    public void SetCurrentSection(string selection)
    {
        // turn off all sections
        noseSection.SetActive(false);
        propellantSection.SetActive(false);
        controlSection.SetActive(false);
        stagesSection.SetActive(false);

        // enable requested one
        switch (selection)
        {
            case "Nose":
                noseSection.SetActive(true);
                break;
            case "Propellant":
                propellantSection.SetActive(true);
                break;
            case "Control":
                controlSection.SetActive(true);
                break;
            case "Stages":
                stagesSection.SetActive(true);
                break;
            default:
                Debug.LogWarning($"[SectionController] Unknown section «{selection}»");
                break;
        }

        UpdateButtonColors(selection);
    }

    private void UpdateButtonColors(string activeSelection)
    {
        // Get each button’s Image and set its color based on whether it’s the active one
        noseButton.image.color        = (activeSelection == "Nose")      ? activeColor : inactiveColor;
        propellantButton.image.color  = (activeSelection == "Propellant")? activeColor : inactiveColor;
        controlButton.image.color     = (activeSelection == "Control")   ? activeColor : inactiveColor;
        stagesButton.image.color      = (activeSelection == "Stages")    ? activeColor : inactiveColor;
    }
}