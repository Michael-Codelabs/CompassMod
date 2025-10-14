using Assets.Scripts.Objects;
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

    private void Update()
    {
      var window = PlayerStateWindow.Instance;
      if (window == null || !window.IsVisible || window.Parent == null)
        return;

      float num = EulerAnglesY(window.Parent);
      CompassImage.uvRect = new Rect(num / 360f, 0f, 1f, 1f);
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