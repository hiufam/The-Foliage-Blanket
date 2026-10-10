using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
  [Header("Attributes")]
  [DefaultValue(10)]
  public float speed = 1f;
  public float health = 100f;
  public float maxHealth = 100f;

  [Header("Objects")]
  public Item item;
  public GameObject arm;

  private Vector3 moveDirection;
  private Vector2 lookDirection;
  private Rigidbody2D rb;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void Start()
  {

  }

  private void Update()
  {
    // UpdatePosition();
    UpdateArmRotation();
  }

  private void FixedUpdate() 
  {
    rb.linearVelocity = moveDirection * speed;  
  }

  public void OnMove(InputValue inputValue)
  {
    moveDirection = inputValue.Get<Vector2>();
  }

  public void OnAttack(InputValue inputValue)
  {
    if (item != null)
    {
      item.Use(lookDirection);
    }
  }

  
  public void OnAltAttack(InputValue inputValue)
  {
    if (item != null)
    {
      item.AltUse(lookDirection);
    }
  }

  public void OnInteract(InputValue inputValue)
  {
    if (item != null)
    {
      item.transform.SetParent(arm.transform);
      item.transform.localPosition = Vector2.zero;
      item.transform.localRotation = Quaternion.identity;
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
}
