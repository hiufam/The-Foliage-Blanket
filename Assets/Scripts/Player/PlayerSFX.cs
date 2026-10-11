using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
  [Header("SFX")]
  public AudioClip runClip;
  public AudioClip fallClip;

  public void PlayFallSFX()
  {
    SoundFXManager.instance.PlaySoundFXClip(fallClip, transform, volume: 0.4f, pitch: new(1f, 1.2f));
  }

  public void PlayRunSFX()
  {
    SoundFXManager.instance.PlaySoundFXClip(runClip, transform, volume: 0.4f, pitch: new(1f, 1.2f));
  }
}
