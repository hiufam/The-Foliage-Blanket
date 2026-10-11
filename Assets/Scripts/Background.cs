using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
  public RawImage img;
  public float x = Mathf.Sqrt(1), y = Mathf.Sqrt(1);

  private void Update()
  {
    img.uvRect = new Rect(img.uvRect.position + new Vector2(x, y) * Time.deltaTime, img.uvRect.size);
  }
}
