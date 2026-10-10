using UnityEngine;

public class Bottle : Obstacle
{
  public float pushForce = 20f;
  public override ObstalceType OType => ObstalceType.Shoe;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.TryGetComponent(out Player player))
    {
      player.Stun();
      Destroy(gameObject);
    }
  }
}
