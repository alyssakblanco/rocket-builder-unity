using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionControl : MonoBehaviour
{
    public void MissionSelection(int mission)
    {
        GameData.selectedMission = mission;
        GameData.keepCurrentSelections = false;
        SceneManager.LoadScene("vehicle_assembly");
    }
}
