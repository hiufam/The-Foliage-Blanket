using System;
using System.Collections;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  private static readonly int Walking = Animator.StringToHash("Walking");
  private static readonly int Default = Animator.StringToHash("Default");
  private static readonly int Knockout = Animator.StringToHash("Knockout");
  private static readonly int Teleport = Animator.StringToHash("Teleport");

  [Header("Attributes")]
  [DefaultValue(10)]
  public float speed = 1f;
  public float health = 100f;
  public float maxHealth = 100f;
  public float stunTime = 0.5f;

  [Header("Reading")]
  public bool isStun = false;
  public bool isDead = false;

  [Header("Objects")]
  public Item item;
  public GameObject arm;
  public Animator animator;
  public SpriteRenderer spriteRenderer;
  public ItemDetector itemDetector;

  private Vector3 moveDirection;
  private Vector2 lookDirection;
  private Rigidbody2D rb;
  private int aniClip = -1;
  private Coroutine stunRoutine;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void Start()
  {

  }

  private void Update()
  {
    itemDetector.enabled = !item;

    if (isStun || isDead) return;
    // UpdatePosition();
    UpdateArmRotation();


    if (aniClip != Default && moveDirection == Vector3.zero)
    {
      animator.Play(Default);
      aniClip = Default;
    }
  }
  

  private void FixedUpdate() 
  {
    if (isStun || isDead) return;

    rb.linearVelocity = moveDirection * speed;  
  }

  public void Revive()
  {
    isDead = false;
    isStun = false;

    if (aniClip != Default)
    {
      animator.Play(Default);
      aniClip = Default;
    }
  }

  public void Die()
  {
    if (isDead) return;

    isStun = true;
    isDead =  true;

    rb.linearVelocity = Vector2.zero;
    moveDirection = Vector2.zero;

    if (stunRoutine != null)
      StopCoroutine(stunRoutine);

    if (aniClip != Knockout)
    {
      animator.Play(Knockout);
      aniClip = Knockout;
    }
  }

  public void Stun()
  {
    if (isStun) return;
    
    isStun = true;

    rb.linearVelocity = Vector2.zero;
    moveDirection = Vector2.zero;

    stunRoutine = StartCoroutine(Delay(stunTime, () =>
    {
      isStun = false;
    }));

    if (aniClip != Knockout)
    {
      animator.Play(Knockout);
      aniClip = Knockout;
    }
  }

  public void OnMove(InputValue inputValue)
  {
    if (isStun || isDead) return;

    moveDirection = inputValue.Get<Vector2>();
    if (aniClip != Walking)
    {
      animator.Play(Walking);
      aniClip = Walking;
    }

    if (moveDirection.x < 0)
    {
      spriteRenderer.flipX = true;      
    }
    
    if (moveDirection.x > 0)
    {
      spriteRenderer.flipX = false;      
    }
  }

  public void OnAttack(InputValue inputValue)
  {
    if (isStun || isDead) return;

    if (item != null)
    {
      item.Use(lookDirection);
    }
  }

  
  public void OnAltAttack(InputValue inputValue)
  {
    if (isStun || isDead) return;

    if (item != null)
    {
      item.AltUse(lookDirection);
    }
  }

  public void OnInteract(InputValue inputValue)
  {
    if (isStun || isDead) return;

    if (item == null)
    {
      Item detectedItem = itemDetector.detectedItem;

      if (detectedItem != null)
      {
        detectedItem.Pickup(arm.transform);
        item = detectedItem;
      }
    }
  }

  public void OnDrop(InputValue inputValue)
  {
    if (isStun || isDead) return;

    if (item != null)
    {
      item.Drop(arm.transform);
      item = null;
    }
  }

  private void UpdatePosition()
  {
    transform.position += speed * moveDirection * Time.deltaTime;
  }

  private void UpdateArmRotation()
  {
    if (Mouse.current != null && item != null)
    {
      Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
      Vector2 pointDir = mousePos - (Vector2)transform.position;

      lookDirection = pointDir;

      float angle = Mathf.Atan2(pointDir.y, pointDir.x) * Mathf.Rad2Deg;

      arm.transform.rotation = Quaternion.Euler(0, 0, angle);
    }
  }

  private IEnumerator Delay(float time, Action action)
  {
    yield return new WaitForSeconds(time);
    action?.Invoke();
  }
}
