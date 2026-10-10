using System;
using Unity.VisualScripting;
using UnityEngine;

public class Shoe : Obstacle
{
  public float pushForce = 20f;
  public override ObstalceType OType => ObstalceType.Shoe;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.TryGetComponent(out Rigidbody2D rb))
    {
      Vector2 forceDir = ( collision.transform.position - transform.position).normalized;
      rb.AddForce(forceDir * pushForce);   
    }
  }
}
