using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
public static class GameData
{
    public static Dictionary<BuilderController.RocketPart, string> currentBuild
        = new Dictionary<BuilderController.RocketPart, string>();

    public static int selectedMission;

    public static bool keepCurrentSelections;
}
