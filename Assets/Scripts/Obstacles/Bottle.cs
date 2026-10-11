using UnityEngine;

public class Bottle : Obstacle
{
  public float pushForce = 100f;
  public float torqueForce = 50f;

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
      Item item = player.DropItem();

      if (item != null && item.TryGetComponent(out Rigidbody2D rb))
      {
        float x = UnityEngine.Random.Range(-1, 1);
        float y = UnityEngine.Random.Range(-1, 1);
        
        rb.AddForce(pushForce * new Vector2(x, y).normalized);
        rb.AddTorque(torqueForce);
      }

      Destroy(gameObject);
    }
  }
}
