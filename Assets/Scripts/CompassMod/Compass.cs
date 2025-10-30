using Assets.Scripts;
using Assets.Scripts.GridSystem;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Entities;
using Assets.Scripts.UI;
using Assets.Scripts.Vehicles;
using UnityEngine;
using UnityEngine.UI;

#nullable disable
namespace lorex
{
  public class Compass : MonoBehaviour
  {
    public RawImage CompassImage;
    public RectTransform CompassRect;
    public RectTransform CompassBackGround;

    private float visibleY = 0f;
    private float hiddenY = 60f;
    private float lerpSpeed = 5f;

    private string cachedCustomName;
    private PlayerStateWindow window;

    private void Update()
    {
      if (window == null)
      {
        window = PlayerStateWindow.Instance;
        if (window == null)
          return;
      }

      if (GameManager.GameState == GameState.Running && string.IsNullOrEmpty(cachedCustomName))
      {
        if (Human.LocalHuman != null)
          cachedCustomName = Human.LocalHuman.CustomName;
      }

      if (window == null || !window.IsVisible || window.Parent == null)
        return;

      Human localplayer = Human.LocalHuman != null ? Human.LocalHuman.RootParentHuman : null;

      if (localplayer == null && !string.IsNullOrEmpty(cachedCustomName))
      {
        foreach (var rover in Rover.AllRovers)
        {
          if (rover == null)
            continue;

          var driver = rover.DriverSlot?.Get<Human>();
          if (driver != null && driver.CustomName == cachedCustomName)
          {
            localplayer = driver;
            break;
          }

          var passenger = rover.PassengerSlot?.Get<Human>();
          if (passenger != null && passenger.CustomName == cachedCustomName)
          {
            localplayer = passenger;
            break;
          }
        }
      }

      if (localplayer == null)
        return;

      bool showHud = localplayer.InternalsOn;

      if (localplayer.SpeciesClass == CharacterCustomisation.SpeciesClass.Robot ||
          localplayer.SpeciesClass == CharacterCustomisation.SpeciesClass.None)
      {
        showHud = true;
      }

      float targetY = showHud ? visibleY : hiddenY;

      Vector2 currentPos = CompassRect.anchoredPosition;
      currentPos.y = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * lerpSpeed);
      CompassRect.anchoredPosition = currentPos;

      if (CompassBackGround != null)
      {
        Vector2 bgPos = CompassBackGround.anchoredPosition;
        bgPos.y = currentPos.y;
        CompassBackGround.anchoredPosition = bgPos;
      }

      if (showHud)
      {
        CompassImage.enabled = true;
        if (CompassBackGround != null)
          CompassBackGround.gameObject.SetActive(true);

        float num = EulerAnglesY(window.Parent);
        CompassImage.uvRect = new Rect(num / 360f, 0f, 1f, 1f);
      }
      else if (currentPos.y >= 30)
      {
        CompassImage.enabled = false;
        if (CompassBackGround != null)
          CompassBackGround.gameObject.SetActive(false);
      }
    }

    private static float EulerAnglesY(Entity entity)
    {
      if (entity == null)
        return 0f;

      try
      {
        float y = entity.EntityRotation.eulerAngles.y;
        return (y + 180f) % 360f;
      }
      catch
      {
        return 0f;
      }
    }
  }
}
