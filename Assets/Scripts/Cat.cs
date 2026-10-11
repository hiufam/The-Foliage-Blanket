using System;
using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Cat : MonoBehaviour
{
  public Player player;
  public float2 targetVariance = new(-2f, 2f);
  public float moveInterval = 1f;
  public float stuckTime = 4f;
  public float unstuckTime = 2f;
  public float idleTime = 1f;
  public float speed = 10f;
  public float pushForce = 25f;

  [Header("Reading")]
  public Vector2 des = Vector2.zero;
  public Vector2 moveDir = Vector2.zero;
  public bool desReached = true;
  public bool isStuck = false;
  public bool isIdle = false;

  private Rigidbody2D rb;
  private Coroutine stuckDetectRoutine;
  private Coroutine unstuckDetectRoutine;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void Start()
  {
    PickDestination();
  }

  private void Update()
  {    
    IsAtDestination();
  }

  private void FixedUpdate()
  {

    if (isIdle) return;

    if (!desReached && !isStuck && des != null)
    {
      Move();
    }

    if (isStuck)
    {
      MoveToUnstuck();
    }
  }

  // private void OnTriggerEnter2D(Collider2D collision)
  // {
  //   if (collision.TryGetComponent(out Rigidbody2D rb))
  //   {
  //     Vector2 forceDir = ( collision.transform.position - transform.position).normalized;
  //     rb.AddForce(forceDir * pushForce);   
  //   }
  // }

  private void Move()
  {
    rb.linearVelocity = moveDir * speed;  

  }

  private void MoveToUnstuck()
  {
    rb.linearVelocity = moveDir * speed;  
  }

  private void PickDestination()
  {
    if (player == null) return;

    float x = UnityEngine.Random.Range(targetVariance.x, targetVariance.y);
    float y = UnityEngine.Random.Range(targetVariance.x, targetVariance.y);
  
    Vector2 endPos = (Vector2)player.transform.position + new Vector2(x, y);

    des = endPos;
    desReached = false;
    isStuck = false;
    isIdle = false;

    moveDir = (des - (Vector2)transform.position).normalized;

    stuckDetectRoutine = StartCoroutine(DetectStuck());
  }

  private void IsAtDestination(float range = 0.2f)
  {
    if (Vector2.Distance(transform.position, des) <= range && !desReached)
    {
      desReached = true;

      if (stuckDetectRoutine != null) StopCoroutine(stuckDetectRoutine);
      if (unstuckDetectRoutine != null) StopCoroutine(unstuckDetectRoutine);

      StartCoroutine(Idle());
    }
  }

  private void TryToUnstuck()
  {
    Vector2 dir = (des - (Vector2)transform.position).normalized;

    float d = UnityEngine.Random.Range(-0.5f, 0.5f);


    if (Math.Abs(dir.x) > Math.Abs(dir.y))
    {
      moveDir = new Vector2(d, dir.y).normalized;
    }
    else
    {
      moveDir = new Vector2(dir.x, d).normalized;      
    }
  }

  private IEnumerator Idle()
  {
    isIdle = true;
    yield return new WaitForSeconds(idleTime);
    isIdle = false;

    PickDestination();
  }

  private IEnumerator DetectStuck()
  {
    yield return new WaitForSeconds(stuckTime);
    isStuck = !desReached;

    TryToUnstuck();

    unstuckDetectRoutine = StartCoroutine(Unstuck());
  }

  private IEnumerator Unstuck()
  {
    yield return new WaitForSeconds(unstuckTime);

    isStuck = false;
    desReached = false;

    StartCoroutine(Idle());
  }
}
