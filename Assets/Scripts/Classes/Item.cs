using UnityEngine;

public abstract class Item : MonoBehaviour
{
  public virtual void Use(Vector2? dir = null) {}  
}
