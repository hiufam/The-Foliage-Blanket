using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
  [Header("SFX")]
  public AudioClip runClip;

  public void PlayRunSFX()
  {
    SoundFXManager.instance.PlaySoundFXClip(runClip, transform, pitch: new(1f, 1.2f));
  }
}
