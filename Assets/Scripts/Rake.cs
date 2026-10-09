using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class Rake : Item
{
  [Header("Attributes")]
  public float pushForce = 500f;
  public float2 pushForceVariance = new float2(-10f, 10f);

  public float torqueForce = 100f;

  [Header("Components")]

  private BoxCollider2D cl;
  private List<GameObject> gameObjects = new();

  private void Awake()
  {
    cl = GetComponent<BoxCollider2D>();
  }

  public override void Use(Vector2? dir = null)
  {
    base.Use();
  
    if (dir == null) return;

    foreach (var go in gameObjects)
    {
      if (go.TryGetComponent(out Rigidbody2D rb)) {
        rb.AddForce(dir.Value.normalized * (pushForce + UnityEngine.Random.Range(pushForceVariance.x, pushForceVariance.y)));
        // rb.AddTorque(UnityEngine.Random.value > 0.5f ? torqueForce : -torqueForce);
      } 
    }
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    gameObjects.Add(collision.gameObject);
  }

  private void OnTriggerExit2D(Collider2D collision)
  {
    GameObject go = collision.gameObject;
    if (gameObjects.Contains(go))

    {
      gameObjects.Remove(go);
    }
  }
}
