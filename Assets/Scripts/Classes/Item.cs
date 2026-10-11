using UnityEngine;

public abstract class Item : MonoBehaviour
{
  public bool allowPickup = false;
  public virtual void EnablePickup()
  {
    allowPickup = true;
  }

  public virtual void DisablePickup()
  {
    allowPickup = false;
  }

  public virtual void Pickup(Transform parent) {}

  public virtual void Drop(Transform parent) {}

  public virtual void Use(Vector2? dir = null) {}  

  public virtual void AltUse(Vector2? dir = null) {}  
}
