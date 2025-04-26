using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButton : MonoBehaviour
{
    public GameObject homeModal;

    public void ShowHomeModal()
    {
        homeModal.SetActive(true);
    } 

    public void HideHomeModal()
    {
        homeModal.SetActive(false);
    } 

    public void ReturnHome()
    {
        SceneManager.LoadScene("main");
    } 
}
