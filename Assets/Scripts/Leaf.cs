using UnityEngine;

public class Leaf : MonoBehaviour
{
  public int? label = null;
  public int trueLabel = 0;
  [HideInInspector]
  public LeafPile leafPile = null;

  private SpriteRenderer spriteRenderer;

  private Color[] colors = {Color.red, Color.green, Color.blue, Color.yellow, Color.purple};

  private void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void Start()
  {
    LeafPileManager.Instance.leaves.Add(this);
  }

  public void Update()
  {
    if (label != null && label >= 0)
    {
      trueLabel = label.Value;
      spriteRenderer.color = colors[trueLabel % colors.Length];
    }
    else
    {
      spriteRenderer.color = Color.white;
    }
  }

  public Vector2 GetPosition()
  {
    return transform.position;
  }
}
