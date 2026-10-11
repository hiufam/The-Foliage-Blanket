using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
  public AudioSource audioSource;
  
  private SoundPool soundPool;
  private AudioClip audioClip;
  private Dictionary<AudioClip, int> soundCounts;

  private void Start()
  {
    soundCounts = SoundFXManager.instance.soundCounts;
  }

  private void Awake()
  {
    audioSource = GetComponent<AudioSource>();
  } 

  public void Init(AudioClip audioClip, SoundPool soundPool,  Dictionary<AudioClip, int> soundCounts)
  {
    this.audioClip = audioClip;
    this.soundPool = soundPool;
    this.soundCounts = soundCounts;

    if (soundCounts.ContainsKey(audioClip))
      soundCounts[audioClip] += 1;
    else
      soundCounts.Add(audioClip, 0);
  }

  public void SelfRelease()
  {
    soundPool.pool.Release(this);

    if (soundCounts.TryGetValue(audioClip, out int count))
      soundCounts[audioClip] = Math.Max(0, count - 1);
  }

  public void ReleaseTimer(float time)
  {
    StartCoroutine(WaitForAction(time, SelfRelease));
  }

  private IEnumerator WaitForAction(float time, Action action)
  {
    yield return new WaitForSeconds(time);
    action?.Invoke();
  }

  private void OnDestroy()
  {
    if (soundCounts.TryGetValue(audioClip, out int count))
      soundCounts[audioClip] = Math.Max(0, count - 1);
  }
}
