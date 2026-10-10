using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public WindRayGroup[] windRayGroups; // l, r, t, b
  public Player player;
  public float windDoT = 0.1f;
  public float healOT = 0.2f;
  public float timeBeforeHeal = 1f;

  [Header("UI")]
  public HealthBar healthBar;

  [Header("Readings")]
  public bool playerHit = false;

  private bool allowHeal = false;

  private void Update()
  {
    if (playerHit)
    {
      allowHeal = false;
      HitPlayer();
    } 
    else
    {
      if (!allowHeal)
      {
        StartCoroutine(Delay(timeBeforeHeal, () =>
        {
          allowHeal = true;
        }));
      }
      else
      {
        HealPlayer();
      }
    }
    healthBar.SetValue(player.health / player.maxHealth);
  }

  private void FixedUpdate()
  {
    DetectHit();
  }

  private void HitPlayer()
  {
    player.health = Mathf.Max(0, player.health - windDoT * Time.deltaTime);
  }

  private void HealPlayer()
  {
    player.health = Mathf.Min(player.maxHealth, player.health + healOT * Time.deltaTime);
  }

  private IEnumerator Delay(float delay, Action action)
  {
    yield return new WaitForSeconds(delay);
    action?.Invoke();
  }

  private void DetectHit()
  {
    foreach (var item in windRayGroups)
    {
      if (item.playerHit)
      {
        playerHit = true;
        return;
      }
    }
    playerHit = false;
  }
}