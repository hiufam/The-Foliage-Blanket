using UnityEngine;

public class ItemDetector : MonoBehaviour
{
  public Item detectedItem;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.TryGetComponent(out Item item))
    {
      item.EnablePickup();
      detectedItem = item;
    }
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    if (collision.TryGetComponent(out Item item))
    {
      item.DisablePickup();      
      detectedItem = null;
    }
  }
}