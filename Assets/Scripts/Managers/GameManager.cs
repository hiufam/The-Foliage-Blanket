using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public WindRayGroup[] windRayGroups; // l, r, t, b
  public ParticleSystem[] windParticles;
  public Player player;
  public WindArea windArea;
  public Cat MrSnuffle;


  [Header("Attributes")]
  public float windDoT = 0.1f;
  public float healOT = 0.2f;
  public float timeBeforeHeal = 1f;
  public float windStartDelay = 1f;
  public float windDuration = 30f;
  public float gameTime = 300f;
  public float diffRate = 30f;
  public int diff = 1;
  public bool callWindOnStart = true;
  public bool callBossText = false;

  [Header("Diff")]
  public float obstacleIntervalRate = 0.1f;
  public float2 obstacleDiff = new float2(0.2f, 2f);
  public float windThrowForceRate = 20f;
  public float2 windThrowForceDiff = new float2(200f, 400f);
  public float leafPercentageRate = 0.02f;
  public float2 leafPercentageDiff = new float2(0.6f, 0.8f);


  [Header("UI")]
  public HealthBar healthBar;
  public HealthBar healthBarSmall;
  public Menu menu;
  public Timer timer;
  public TextMeshProUGUI gameText;
  public TextMeshPro[] tutorialTexts;
  public TextMeshPro bossText;


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
    // StartGame();
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
      gameText.text = "HYPOTHERMIA!";
    }
  }

  private void FixedUpdate()
  {
    DetectHit();
  }

  public void StartGame()
  {
    windArea.activate = true;

    if (callWindOnStart) StartWind(1);

    StartCoroutine(HideTutorial());
    StartCoroutine(CallWind());
    StartCoroutine(GameTimer());

    gameText.gameObject.SetActive(false);
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

  private void UpdateDiff(float time)
  {
    diff = Mathf.Max(0, (int)(time / diffRate));

    // Update wind group
    foreach (var item in windRayGroups)
    {
      item.obstacleInterval = Mathf.Max(obstacleDiff.x, obstacleDiff.y - obstacleIntervalRate * diff);
      item.throwForce = Mathf.Min(windThrowForceDiff.y, windThrowForceDiff.x + windThrowForceRate * diff);
    } 

    // Update wind area
    windArea.maxLeavesPercentage = Mathf.Min(leafPercentageDiff.y, leafPercentageDiff.x + leafPercentageRate * diff);

    MrSnuffle.gameObject.SetActive(diff >= 5);

    if (!callBossText && MrSnuffle.gameObject.activeSelf)
    {
      bossText.gameObject.SetActive(true);

      StartCoroutine(HideBossText());
      callBossText = true;
    }
  }

  private IEnumerator HideBossText(float time = 3f, float hideTime = 1f)
  {
    yield return new WaitForSeconds(time);

    float elapsedTime = 0f;

    while (elapsedTime < hideTime)
    {
      elapsedTime += Time.deltaTime;
      
      float alpha = Mathf.Lerp(1, 0, elapsedTime / hideTime);
      Color color = new Color(255, 255, 255, alpha);

      bossText.color = color;

      yield return null;
    }
  }

  private IEnumerator HideTutorial(float time = 3f, float hideTime = 1f)
  {
    yield return new WaitForSeconds(time);

    float elapsedTime = 0f;

    while (elapsedTime < hideTime)
    {
      elapsedTime += Time.deltaTime;
      
      float alpha = Mathf.Lerp(1, 0, elapsedTime / hideTime);
      Color color = new Color(255, 255, 255, alpha);

      foreach (var item in tutorialTexts)
      {
        item.color = color;
      } 

      yield return null;
    }
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

    timer.gameObject.SetActive(true);

    while (elapsedTime < gameTime)
    {
      elapsedTime += Time.deltaTime;

      float remainingTime = gameTime - elapsedTime;
  
      TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);

      UpdateDiff(elapsedTime);

      timer.SetText($"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}"); 

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