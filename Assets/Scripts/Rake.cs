using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Rendering;
using UnityEngine;

public class Rake : Item
{
  private static readonly int PushHash = Animator.StringToHash("Push");
  private static readonly int PullHash = Animator.StringToHash("Pull");

  [Header("Attributes")]
  public float pushForce = 500f;
  public float pullForce = 250f;
  public float2 pushForceVariance = new float2(-10f, 10f);

  public float torqueForce = 100f;

  [Header("Components")]
  public SpriteRenderer spriteRenderer;
  public GameObject secondCl;
  public GameObject pickupTextPrefab;
  public GameObject pickupText;

  private BoxCollider2D cl;
  private Animator animator;
  private List<GameObject> gameObjects = new();

  private void Awake()
  {
    cl = GetComponent<BoxCollider2D>();
    animator = GetComponent<Animator>();
  }

  private void Start()
  {
    pickupText = Instantiate(pickupTextPrefab, Vector2.zero, Quaternion.identity);
    pickupText.SetActive(false);
    
    secondCl.layer = 6;
    animator.enabled = false;
  }

  public override void EnablePickup()
  {
    base.EnablePickup();

    if (pickupText != null)
    {
      pickupText.SetActive(true);
      pickupText.transform.position = (Vector2)transform.position + Vector2.up;
    }
  }

  public override void DisablePickup()
  {
    base.DisablePickup();

    if (pickupText != null)
    {
      pickupText.SetActive(false);
    }
  }

  public override void Pickup(Transform parent)
  {
    base.Pickup(parent);

    secondCl.layer = 0;
    animator.enabled = true;

    transform.SetParent(parent);
    transform.localPosition = Vector2.zero;
    transform.localRotation = Quaternion.identity;

    spriteRenderer.sortingOrder = 1;

    if (pickupText != null)
    {
      pickupText.SetActive(false);
    }
  }

  public override void Drop(Transform parent)
  {
    base.Drop(parent);

    secondCl.layer = 6;
    animator.enabled = false;

    spriteRenderer.sortingOrder = 0;

    transform.SetParent(null);

    if (pickupText != null)
    {
      pickupText.SetActive(true);
      pickupText.transform.position = (Vector2)transform.position + Vector2.up;
    }
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

    animator.ResetTrigger(PullHash);
    animator.SetTrigger(PushHash);
  }

  public override void AltUse(Vector2? dir = null)
  {
    base.AltUse();
  
    if (dir == null) return;

    foreach (var go in gameObjects)
    {
      if (go.TryGetComponent(out Rigidbody2D rb)) {
        rb.AddForce(dir.Value.normalized * (-pullForce + UnityEngine.Random.Range(pushForceVariance.x, pushForceVariance.y)));
      } 
    }

    animator.ResetTrigger(PushHash);
    animator.SetTrigger(PullHash);
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
