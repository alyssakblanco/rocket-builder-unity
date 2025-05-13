using UnityEngine;
using UnityEngine.UI;
using System.Collections; 
using System.Text.RegularExpressions;
using TMPro;

public class LaunchSetup : MonoBehaviour
{
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDesc;

    public TextMeshProUGUI missionSuccessTitle;
    public TextMeshProUGUI missionSuccessDesc;

    public TextMeshProUGUI missionFailTitle;
    public TextMeshProUGUI missionFailDesc;
    
    public TextMeshProUGUI currentNose;
    public TextMeshProUGUI currentPropellant;
    public TextMeshProUGUI currentControl;
    public TextMeshProUGUI currentStages; 

    public TextMeshProUGUI success_hint;
    public TextMeshProUGUI fail_hint;

    public void Start(){
        GameData.SetMissionStatus();
        SetBuildInfo();
        setupMissionReport();
        SetCostAndWeight();
    }
    
    public void SetBuildInfo(){
        missionTitle.text = GameData.Missions[GameData.selectedMission].title;
        missionDesc.text  = GameData.Missions[GameData.selectedMission].desc;

        missionSuccessTitle.text = GameData.Missions[GameData.selectedMission].title;
        missionSuccessDesc.text  = GameData.Missions[GameData.selectedMission].desc;

        missionFailTitle.text = GameData.Missions[GameData.selectedMission].title;
        missionFailDesc.text  = GameData.Missions[GameData.selectedMission].desc;

        string control = GameData.currentBuild[BuilderController.RocketPart.Control];
        if(control == "fins"){
            control = "movable fins";
        }
        
        currentNose.text = ToTitleCaseInvariant(GameData.currentBuild[BuilderController.RocketPart.Nose]);
        currentPropellant.text = ToTitleCaseInvariant(GameData.currentBuild[BuilderController.RocketPart.Propellant]);
        currentControl.text = ToTitleCaseInvariant(control);
        currentStages.text = ToTitleCaseInvariant(GameData.currentBuild[BuilderController.RocketPart.Stage]); 
    }

    public static string ToTitleCaseInvariant(string input)
    {
        return Regex.Replace(
            input.ToLower(),             // normalize
            @"\b[a-z]",                  // find lowercase letters at word boundaries
            match => match.Value.ToUpper()
        );
    }

    private void setupMissionReport(){
        if(GameData.missionStatus){
            success_hint.text =  GameData.hint;
        }else{
            fail_hint.text =  GameData.hint;
        }
    }

        // COST WIEGHT STUFF
    [Header("Cost & Weight")]
    public Color32 activeColor;
    public Color inactiveColor = Color.white;
    public Image[] money;
    public Image[] anvil;

    public void SetCostAndWeight(){
        for (int i = 0; i < money.Length; i++)
        {
            var img = money[i];
            img.color = (i < GameData.currentCost) ? activeColor : inactiveColor;
        }

        for (int i = 0; i < anvil.Length; i++)
        {
            var img = anvil[i];
            img.color = (i < GameData.currentWeight) ? activeColor : inactiveColor;
        }
    }
}
