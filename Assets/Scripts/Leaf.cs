using UnityEngine;

public class Leaf : MonoBehaviour
{
  public int? label = null;
  public int trueLabel = 0;
  [HideInInspector]
  public LeafPile leafPile = null;

  private SpriteRenderer spriteRenderer;
  private Rigidbody2D rb;

  private Color[] colors = {Color.red, Color.green, Color.blue, Color.yellow, Color.purple};

  private void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    rb = GetComponent<Rigidbody2D>();
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

  public void ApplyForce(Vector2 force)
  {
    rb.AddForce(force);
  }

  public Vector2 GetPosition()
  {
    return transform.position;
  }
}
