using System;
using UnityEngine;

public enum ObstalceType
{
  Shoe
}

public abstract class Obstacle : MonoBehaviour
{
  public abstract ObstalceType OType {get;}

  private Rigidbody2D rb;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  public void ApplyForce(Vector2 force, float? torque = null)
  {
    rb.AddForce(force);

    if (torque != null)
      rb.AddTorque(torque.Value);
  }
}
