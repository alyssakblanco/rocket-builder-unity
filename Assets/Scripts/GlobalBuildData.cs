using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
public static class GameData
{
    public static Dictionary<BuilderController.RocketPart, string> currentBuild
        = new Dictionary<BuilderController.RocketPart, string>();

    // 0: "deployment", 1: "atmosphere", 2: "planet"
    public static int selectedMission;

    public static bool keepCurrentSelections;

    public static float currentCost;
    public static float currentWeight;

    public static bool missionStatus;
    public static bool makesItToSpace;
    public static string hint;

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

    public static void SetMissionStatus(){
        // sat deployment
        if(selectedMission == 0){
            if(currentBuild[BuilderController.RocketPart.Stage] == "1"){
                missionStatus = false;
                makesItToSpace = false;
                hint = "Your single stage rocket isn't powerful enough to make it into space with your satellite.";
            }else if(currentBuild[BuilderController.RocketPart.Nose] != "payload"){
                missionStatus = false;
                makesItToSpace = true;
                hint = "You dont have the right nose cone for this mission. Which nose cone allows you to deploy a satellite?";
            }else{
                missionStatus = true;
                makesItToSpace = true;
                if(currentBuild[BuilderController.RocketPart.Control] == "fins"){
                    hint = "Your rocket made it into space but the fins were no help in space so your satillite didnt deploy at the right place. What kind of control allows you to control your rocket isn space?";
                }
                hint = "You're on your way to being a rocket scientist!";
            }
        }

        // atmosphere
        if(selectedMission == 1){
            missionStatus = true;
            makesItToSpace = false;
            hint = "You've made it to the upper parts of the atmosphere! Can you try a more challenging mission next?";
        }

        // planet
        if(selectedMission == 2){
            if(currentBuild[BuilderController.RocketPart.Stage] != "3"){
                missionStatus = false;
                if(currentBuild[BuilderController.RocketPart.Stage] == "2"){makesItToSpace = true;}else{makesItToSpace = false;}
                hint = "Your rocket isn't powerful enough to make it to another planet. What number of stages is best for long term flight?";
            } else if (currentBuild[BuilderController.RocketPart.Control] == "fins"){
                missionStatus = false;
                makesItToSpace = true;
                hint = "Your fins dont work in space and you can't steer your rocket to the right planet!";
            } else if (currentBuild[BuilderController.RocketPart.Propellant] != "liquid"){
                missionStatus = false;
                makesItToSpace = true;
                hint = "Your propellant gets all used up in one shot and you use all your fuel before you get to the planet!";
            } else {
                missionStatus = true;
                makesItToSpace = true;
                if(currentBuild[BuilderController.RocketPart.Nose] != "payload"){
                    hint = "Your rocket is on its way! But which nose cone will allow you to deploy isntruments for research at the target planet?";
                }
                hint = "Congrats, after one slingshot around the globe you'll be on your way to another world!";
            }
        }
    }
}
