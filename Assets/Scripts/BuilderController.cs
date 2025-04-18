using UnityEngine;
using System.Collections.Generic;

public class BuilderController : MonoBehaviour
{
    // Prefabs for rocket parts
    public GameObject bottomStagePrefab;
    public GameObject[] stagePrefabs;     
    public GameObject[] noseConePrefabs;    
    public GameObject fins;
    public GameObject engines;

    public enum RocketPart
    {
        Stage,
        Nose,
        Propellant,
        Control
    }

    private Dictionary<RocketPart, string> currentBuild = new Dictionary<RocketPart, string>();

    public void Start()
    {
        currentBuild[RocketPart.Stage] = "1";       // 1, 2, 3
        currentBuild[RocketPart.Nose] = "ogive";      // ogive, blunt, payload
        currentBuild[RocketPart.Propellant] = "solid"; // solid, liquid
        currentBuild[RocketPart.Control] = "fins";      // gimbal, fins
    }

    // General update method that delegates based on part type
    private void UpdateRocket(RocketPart partType, string userSelection)
    {
        switch (partType)
        {
            case RocketPart.Stage:
                UpdateStage(userSelection);
                break;
            case RocketPart.Nose:
                UpdateNose(userSelection);
                break;
            case RocketPart.Propellant:
                UpdatePropellant(userSelection);
                break;
            case RocketPart.Control:
                UpdateControl(userSelection);
                break;
        }
    }

    //
    // Update methods for each category
    //

    private void UpdateStage(string selection)
    {
        if (selection == "1")
        {
            // Remove middle and top stage (if present)
            // Bring nose to bottom stage connection point
            Debug.Log("Selected Stage configuration 1: Removing extra stages and using bottom stage only.");
        }
        else if (selection == "2")
        {
            // Remove top stage (if present)
            // Add middle stage (if not present)
            // Attach nose cone and bottom stage appropriately
            Debug.Log("Selected Stage configuration 2: Adding middle stage.");
        }
        else if (selection == "3")
        {
            // Add top stage
            // Attach nose cone and attach to middle stage (if not already)
            Debug.Log("Selected Stage configuration 3: Adding top stage.");
        }
    }

    private void UpdateNose(string selection)
    {
        // Options: "ogive", "blunt", "payload"
        // Destroy current nose and replace with the appropriate prefab.
        if (selection == "ogive")
        {
            Debug.Log("Replacing nose with ogive prefab.");
            // Instantiate the ogive nose prefab and position it
        }
        else if (selection == "blunt")
        {
            Debug.Log("Replacing nose with blunt prefab.");
            // Instantiate the blunt nose prefab and position it
        }
        else // assume payload
        {
            Debug.Log("Replacing nose with payload prefab.");
            // Instantiate the payload nose prefab and position it
        }
    }

    private void UpdatePropellant(string selection)
    {
        // Options: "solid", "liquid"
        if (selection == "liquid")
        {
            Debug.Log("Showing liquid propellant image.");
        }
        else
        {
            Debug.Log("Showing solid propellant image.");
        }
    }

    private void UpdateControl(string selection)
    {
        // Options: "gimbal", "fins"
        if (selection == "gimbal")
        {
            Debug.Log("Selecting gimbal control: hiding fins and starting engine animation.");
        }
        else
        {
            Debug.Log("Selecting fins control: showing fins and stopping engine animation.");
        }
    }

    //
    // Public methods for UI to call for each selection
    //

    public void SelectStages(string userSelection)
    {
        currentBuild[RocketPart.Stage] = userSelection;
        UpdateRocket(RocketPart.Stage, userSelection);
    }

    public void SelectNose(string userSelection)
    {
        currentBuild[RocketPart.Nose] = userSelection;
        UpdateRocket(RocketPart.Nose, userSelection);
    }

    public void SelectPropellant(string userSelection)
    {
        currentBuild[RocketPart.Propellant] = userSelection;
        UpdateRocket(RocketPart.Propellant, userSelection);
    }

    public void SelectControl(string userSelection)
    {
        currentBuild[RocketPart.Control] = userSelection;
        UpdateRocket(RocketPart.Control, userSelection);
    }
}
