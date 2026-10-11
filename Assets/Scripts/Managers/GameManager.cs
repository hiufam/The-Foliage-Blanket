using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public WindRayGroup[] windRayGroups; // l, r, t, b
  public ParticleSystem[] windParticles;
  public Player player;
  public WindArea windArea;


  [Header("Attributes")]
  public float windDoT = 0.1f;
  public float healOT = 0.2f;
  public float timeBeforeHeal = 1f;
  public float windStartDelay = 1f;
  public float windDuration = 30f;
  public float gameTime = 300f;
  public bool callWindOnStart = true;


  [Header("UI")]
  public HealthBar healthBar;
  public HealthBar healthBarSmall;
  public Menu menu;
  public Timer timer;
  public TextMeshProUGUI gameText;

  [Header("Readings")]
  public bool playerHit = false;
  public bool survive = false;
  public int windDirection = 0; // l r t b (1, 2, 3, 4)


  private int currentWindDir = 0;
  private bool allowHeal = false;
  private List<int> windDirs = new() {1, 2, 3, 4};

  public static GameManager Instance { get; private set; }

  private void Awake()
  {
    // Check if an instance already exists
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
  }


  private void Start()
  {
    if (callWindOnStart) StartWind(1);
    StartCoroutine(CallWind());
    StartCoroutine(GameTimer());
    gameText.gameObject.SetActive(false);
  }

  private void Update()
  {
    if (survive)
    {
      if (!gameText.gameObject.activeSelf)
      {
        menu.Open();

        gameText.gameObject.SetActive(true);
        gameText.text = "YOU SURVIVED!";
      }
      return;
    }

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

    if (!player.isDead)
    {
      healthBar.SetValue(player.health / player.maxHealth);    
      healthBarSmall.SetValue(player.health / player.maxHealth);
    }

    if (player.health <= 0 && !player.isDead)
    {
      player.Die();
      menu.Open();

      gameText.gameObject.SetActive(true);
      gameText.text = "YOU DIDN'T LAST!";
    }
  }

  private void FixedUpdate()
  {
    DetectHit();
  }

  public void Reset()
  {
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
  }

  private void HitPlayer()
  {
    player.health = Mathf.Max(0, player.health - windDoT * Time.deltaTime);
  }

  private void HealPlayer()
  {
    player.health = Mathf.Min(player.maxHealth, player.health + healOT * Time.deltaTime);
  }

  private void StartWind(int dir)
  {
    if (dir <= 0 || dir > 4) return;

    playerHit = false;

    foreach (var item in windRayGroups)
      item.Deactivate();
  
    foreach(var item in windParticles)
      item.Stop();

    int index = dir - 1;

    windParticles[index].gameObject.SetActive(true);
    windParticles[index].Play();

    StartCoroutine(Delay(windStartDelay, () =>
    {
      windRayGroups[index].Activate();
    }));

    windArea.ChangeDir(dir);

    currentWindDir = dir;
  }

  private IEnumerator CallWind()
  {
    float elapsedTime = 0f;
    
    while (elapsedTime < windDuration)
    {
      elapsedTime += Time.deltaTime;

      if (elapsedTime >= windDuration)
      {
        int randomDirIndex = UnityEngine.Random.Range(0, 3);
        int[] bundleDir = windDirs.Where((i) => i != currentWindDir).ToArray();
      
        int randomDir = bundleDir[randomDirIndex];

        StartWind(randomDir);

        elapsedTime = 0f;
      }

      yield return null;
    }
  }

  private IEnumerator Delay(float delay, Action action)
  {
    yield return new WaitForSeconds(delay);
    action?.Invoke();
  }

  private IEnumerator GameTimer()
  {
    float elapsedTime = 0f;

    while (elapsedTime < gameTime)
    {
      elapsedTime += Time.deltaTime;

      float remainingTime = gameTime - elapsedTime;
  
      TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);

      timer.SetText($"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}"); 

      yield return null;
    }

    survive = true;
  }

  private void DetectHit()
  {
    if (currentWindDir <= 0 || currentWindDir > 4) return;
    playerHit = windRayGroups[currentWindDir - 1].playerHit;
  }
}