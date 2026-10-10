using UnityEngine;

public class WindRayGroup : MonoBehaviour
{
  [SerializeField] private WindRay windRayPrefab;
  public Vector2 startPos;
  public Vector2 dir = Vector2.down;
  public Vector2 rayDir = Vector2.right;
  public int raysCount;
  public float raysGap;
  public bool playerHit = false;

  private WindRay[] windRays;

  private void Start()
  {
    CreateWindRays();
  }

  private void FixedUpdate()
  {
    DetectHit();
  }

  private void DetectHit()
  {
    foreach (var item in windRays)
    {
      if (item.playerHit)
      {
        playerHit = true;
        return;
      }
    }
    playerHit = false;
  }

  private void CreateWindRays()
  {
    windRays = new WindRay[raysCount];

    for (int i = 0; i < raysCount; i++)
    {
      Vector2 spawnPos = startPos + raysGap * i * dir;

      WindRay windRay = Instantiate(windRayPrefab, spawnPos, Quaternion.identity);
      windRay.direction = rayDir;
      windRay.transform.SetParent(transform);

      windRays[i] = windRay; 
    }
  }  
}
