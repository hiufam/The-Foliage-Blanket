using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ConfigManager : MonoBehaviour
{
  public bool isEndless = false;

  public static ConfigManager Instance { get; private set; }

  private void Awake()
  {
    // Check if an instance already exists
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
  
    DontDestroyOnLoad(this);
  }

  public void ToggleEndless()
  {
    isEndless = !isEndless;
    GameManager.Instance.Reset();
  }
}
