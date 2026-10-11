using UnityEngine;

public class Bottle : Obstacle
{
  public float pushForce = 20f;

  [Header("Particles")]
  public ParticleSystem brokenGlasses;

  [Header("SFX")]
  public AudioClip brokenBottleClip;

  public override ObstalceType OType => ObstalceType.Shoe;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (collision.TryGetComponent(out Player player))
    {
      Instantiate(brokenGlasses, transform.position, Quaternion.identity);
      SoundFXManager.instance.PlaySoundFXClip(brokenBottleClip, transform, volume: 0.6f);

      player.Stun();
      Destroy(gameObject);
    }
  }
}
