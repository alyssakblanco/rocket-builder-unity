using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionControl : MonoBehaviour
{
    public void MissionSelection(int mission)
    {
        CurrentMission.SelectedMission = mission;
        SceneManager.LoadScene("vehicle_assembly");
    }
}
