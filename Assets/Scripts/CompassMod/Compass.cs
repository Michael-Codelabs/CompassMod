using Assets.Scripts.Objects;
using Assets.Scripts.Objects.Entities;
using Assets.Scripts.UI;
using TMPro;
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

    private float visibleY = 0f;   // when InternalsOn = true
    private float hiddenY = 60f;   // when InternalsOn = false
    private float lerpSpeed = 5f;

    private void Update()
    {
      var window = PlayerStateWindow.Instance;
      if (window == null || !window.IsVisible || window.Parent == null)
        return;

      var localplayer = Human.LocalHuman?.RootParentHuman;
      if (localplayer == null)
        return;

      bool showHud = localplayer.InternalsOn;
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
      else
      {
        if (currentPos.y >= 30)
        {
          CompassImage.enabled = false;
          if (CompassBackGround != null)
            CompassBackGround.gameObject.SetActive(false);
        }
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
