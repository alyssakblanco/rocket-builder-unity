using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigation : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        if(sceneName == "edit_build"){
            GameData.keepCurrentSelections = true;
            sceneName = "vehicle_assembly";
            Debug.Log("hello"  + GameData.keepCurrentSelections);
        }
        SceneManager.LoadScene(sceneName);
    }
}
