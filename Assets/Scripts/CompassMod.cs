using System;
using lorex;
using HarmonyLib;
using StationeersMods.Interface;
[StationeersMod("CompassMod","CompassMod [StationeersMods]","0.2.4657.21547.1")]
public class CompassMod : ModBehaviour
{
    // private ConfigEntry<bool> configBool;
    
    public override void OnLoaded(ContentHandler contentHandler)
    {
        UnityEngine.Debug.Log("CompassMod says: Hello World!");
        
        //Config example
        // configBool = Config.Bind("Input",
        //     "Boolean",
        //     true,
        //     "Boolean description");
        
        Harmony harmony = new Harmony("CompassMod");
        PrefabPatch.prefabs = contentHandler.prefabs;
        harmony.PatchAll();
        UnityEngine.Debug.Log("CompassMod Loaded with " + contentHandler.prefabs.Count + " prefab(s)");
    }
}
