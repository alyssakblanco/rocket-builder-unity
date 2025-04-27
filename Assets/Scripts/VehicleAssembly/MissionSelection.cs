using UnityEngine;
using TMPro;
using System.Collections.Generic; 
using UnityEngine.SceneManagement;

[System.Serializable]
public struct MissionInfo
{
    public string title;
    [TextArea]
    public string desc;
}

public class MissionSelection : MonoBehaviour
{
    public TextMeshProUGUI missionTitle;
    public TextMeshProUGUI missionDesc;
    public GameObject changeMissionModal;

    public static readonly MissionInfo[] Missions = new MissionInfo[]
    {
        new MissionInfo {
            title = "Satellite Deployment",
            desc  = "Leave Earth’s atmosphere, deploy a satellite and return."
        },
        new MissionInfo {
            title = "Atmosphere Research",
            desc  = "Stay in Earth’s atmosphere briefly and then return."
        },
        new MissionInfo {
            title = "Planet Exploration",
            desc  = "Breeze past our atmosphere and reach Mars."
        }
    };

    void Awake(){
        missionTitle.text = Missions[CurrentMission.SelectedMission].title;
        missionDesc.text  = Missions[CurrentMission.SelectedMission].desc;
    }

    // CHANGE MISSION MODAL FUNCTIONS
    public void ShowChangeMissionModal()
    {
        changeMissionModal.SetActive(true);
    } 

    public void HideChangeMissionModal()
    {
        changeMissionModal.SetActive(false);
    } 

    public void ChangeMission()
    {
        SceneManager.LoadScene("mission_select");
    } 
}
