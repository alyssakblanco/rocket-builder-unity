using UnityEngine;
using System.Collections; 
using TMPro;

public class LaunchSetup : MonoBehaviour
{
    public TextMeshProUGUI currentNose;
    public TextMeshProUGUI currentPropellant;
    public TextMeshProUGUI currentControl;
    public TextMeshProUGUI currentStages; 

    public TextMeshProUGUI success_hint;
    public TextMeshProUGUI fail_hint;

    public void Start(){
        currentNose.text = GameData.currentBuild[BuilderController.RocketPart.Nose];
        currentPropellant.text = GameData.currentBuild[BuilderController.RocketPart.Propellant];
        currentControl.text = GameData.currentBuild[BuilderController.RocketPart.Control];
        currentStages.text = GameData.currentBuild[BuilderController.RocketPart.Stage];

        GameData.SetMissionStatus();
        setupMissionReport();
    }

    private void setupMissionReport(){
        if(GameData.missionStatus){
            success_hint.text =  GameData.hint;
        }else{
            fail_hint.text =  GameData.hint;
        }
    }
}
