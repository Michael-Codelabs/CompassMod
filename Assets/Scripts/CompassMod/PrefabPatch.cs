using Assets.Scripts.Objects;
using Assets.Scripts.UI;
using HarmonyLib;
using StationeersMods.Interface;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace lorex
{
  [HarmonyPatch]
  public class PrefabPatch
  {
    public static ReadOnlyCollection<GameObject> prefabs { get; set; }

    [HarmonyPatch(typeof(Prefab), "LoadAll")]
    [HarmonyPrefix]
    public static void PrefixLoadAll()
    {
      try
      {
        Debug.Log("Prefab Patch started");
        if (prefabs == null) return;

        foreach (var gameObject in prefabs)
        {
          Thing thing = gameObject.GetComponent<Thing>();
          if (thing != null)
          {
            Debug.Log(gameObject.name + " added to WorldManager");
            WorldManager.Instance.SourcePrefabs.Add(thing);
          }
        }
      }
      catch (Exception ex)
      {
        Debug.Log(ex.Message);
        Debug.LogException(ex);
      }
    }

    [HarmonyPatch(typeof(PlayerStateWindow), "Awake")]
    [HarmonyPostfix]
    public static void PostfixAwake(PlayerStateWindow __instance)
    {
      __instance.StartCoroutine(InstantiateCompassAtTopCenter(__instance));
    }

    private static System.Collections.IEnumerator InstantiateCompassAtTopCenter(PlayerStateWindow window)
    {
      GameObject compassPrefab = null;

      while (compassPrefab == null)
      {
        if (PrefabPatch.prefabs != null)
          compassPrefab = PrefabPatch.prefabs.FirstOrDefault(p => p != null && p.name.Equals("Compass"));

        if (compassPrefab == null)
          yield return null;
      }

      Compass compassComponent = compassPrefab.GetComponent<Compass>();
      if (compassComponent == null)
      {
        Debug.LogError("Compass component not found on prefab!");
        yield break;
      }

      Compass compass = UnityEngine.Object.Instantiate(compassComponent);

      RectTransform rect = compass.GetComponentInChildren<RectTransform>();
      rect.SetParent(window.transform.parent, false);

      rect.anchorMin = new Vector2(0.5f, 1f);
      rect.anchorMax = new Vector2(0.5f, 1f);
      rect.pivot = new Vector2(0.5f, 1f);

      rect.anchoredPosition = new Vector2(0f, -30f);

      Debug.Log("Compass instantiated at top-center.");
    }
  }
}