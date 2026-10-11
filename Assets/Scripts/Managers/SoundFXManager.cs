using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// Source: https://www.youtube.com/watch?v=DU7cgVsU2rM&t=469s
public class SoundFXManager : MonoBehaviour
{
  public static SoundFXManager instance;
  
  [Header("Attributes")]
  public int maxDuplicatedSoundClips = 3;
  public float audibleRadius = 25f;

  [SerializeField] private AudioSource soundFXObject;
  [SerializeField] private Sound soundPrefab;

  private SoundPool soundPool;

  [Header("Debug")]
  public Dictionary<AudioClip, int> soundCounts = new();

  public void Reset()
  {
    soundCounts = new();
  }

  private void Awake()
  {
    if (instance == null)
      instance = this;
  
    soundPool = new(soundPrefab);
  }

  public void PlaySoundFXClip(AudioClip audioClip, Transform transform, float? volume = null, float? volModifer = null, float2? pitch = null, float? solidPitch = null)
  {    
    if (soundCounts.TryGetValue(audioClip, out int count) && count >= maxDuplicatedSoundClips) return;
    // else
    //   soundCounts.Add(audioClip, 0);

    float finalVolume;

    if (volume != null)
    {
      finalVolume = volume.Value;
    }
    else
    {
      float distance = Vector2.Distance(Camera.main.transform.position, transform.position);
      float distanceVolume = Mathf.Clamp01(1f - (distance / audibleRadius));

      finalVolume = distanceVolume;
    }

    Sound sound = soundPool.pool.Get();
    sound.Init(audioClip, soundPool, soundCounts);
    sound.transform.position = transform.position;

    AudioSource audioSource = sound.audioSource;

    audioSource.clip = audioClip;
    audioSource.volume = finalVolume;
    
    if (pitch != null)
      audioSource.pitch = UnityEngine.Random.Range(pitch.Value.x, pitch.Value.y);

    if (solidPitch != null)
      audioSource.pitch = solidPitch.Value;

    if (volModifer != null)
      audioSource.volume *= volModifer.Value;

    audioSource.Play();

    float clipLength = audioSource.clip.length;

    // Destroy(audioSource.gameObject, clipLength);
    sound.ReleaseTimer(clipLength);
  }

  public void PlaySoundFXClipMixture(AudioClip[] audioClips, Transform transform, float? volume = null, float? volModifer = null, float2? pitch = null)
  {    
    int index = UnityEngine.Random.Range(0, audioClips.Length);
    PlaySoundFXClip(audioClips[index], transform, volume, volModifer, pitch);
  }
}
