using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[Serializable]
public struct ObstacleStuct
{
  public Obstacle prefab;
  public int spawnWeight;
}

public class WindRayGroup : MonoBehaviour
{
  [SerializeField] private WindRay windRayPrefab;
  [SerializeField] private ObstacleStuct[] obstacles ;

  public Vector2 startPos;
  public Vector2 dir = Vector2.down;
  public Vector2 rayDir = Vector2.right;
  public float obstacleAimVariance = 2f;
  public float obstacleInterval = 2f;
  public float throwForce = 200f;
  public float throwTorque = 100f;
  public int raysCount;
  public float raysGap;
  public bool playerHit = false;
  public bool activate = false;

  private WindRay[] windRays;
  private Coroutine obstacleRoutine;

  private void Start()
  {
    CreateWindRays();
  }

  private void FixedUpdate()
  {
    if (!activate) return;
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

  public void Activate()
  {
    activate = true;
    obstacleRoutine = StartCoroutine(ObstacleInterval());

    if (windRays != null)
    {
      foreach (var item in windRays)
      {
        item.gameObject.SetActive(true);
      }
    }
  }

  public void Deactivate()
  {
    activate = false;
    if (obstacleRoutine != null)
    {
      StopCoroutine(obstacleRoutine);
    }

    if (windRays != null)
    {
      foreach (var item in windRays)
      {
        item.gameObject.SetActive(false);
      }
    }
  }

  private void ThrowObstacle()
  {
    ObstacleStuct? obstacleStr = PickObstacle();
    if (obstacleStr == null) return;

    Vector2 spawnPos = PickSpawnPos();

    Obstacle obstaclePrefab = obstacleStr.Value.prefab;
    Obstacle instance = Instantiate(obstaclePrefab, PickSpawnPos(), Quaternion.identity);

    Vector2 targetPos;

    Vector2 pos = (Vector2)instance.transform.position;
    Vector2 deviation = new Vector2(UnityEngine.Random.Range(-obstacleAimVariance, obstacleAimVariance), UnityEngine.Random.Range(-obstacleAimVariance, obstacleAimVariance));

    targetPos = Vector2.zero;

    if (instance.OType == ObstalceType.Shoe)
    {
      Vector2? posPile = PickLargestPilePosition();

      if (pos != null)
      {
        targetPos = posPile.Value;
      }
    }

    Vector2 aimDir = targetPos + deviation - pos;

    // Debug.DrawRay(spawnPos, aimDir * 5f, Color.red);

    instance.ApplyForce(throwForce * aimDir.normalized, throwTorque);

    StartCoroutine(Delay(5f, () =>
    {
      Destroy(instance.gameObject);
    }));
  }

  private Vector2 PickSpawnPos()
  {
    int index = UnityEngine.Random.Range(0, windRays.Length);
    return windRays[index].transform.position;
  }

  private Vector2? PickLargestPilePosition()
  {
    int max = -1;
    Vector2? largest = null;

    LeafPileManager leafPileManager = LeafPileManager.Instance;
    leafPileManager.polys.ForEach((poly) =>
    {
      if (poly.TryGetComponent(out PolygonCollider2D pcl))
      {
        int pCount = pcl.points.Length;
        
        if (pCount > max)
        {
          largest = pcl.points.Last();
          max = pCount;
        }
      }
    });

    return largest;
  }

  private ObstacleStuct? PickObstacle()
  {
    int totalWeight = 0;
    foreach (var item in obstacles)
    {
      totalWeight += item.spawnWeight;
    }

    int roll = UnityEngine.Random.Range(0, totalWeight);

    int cumulative = 0;

    foreach (var item in obstacles)
    {
      cumulative += item.spawnWeight;
      if (roll < cumulative)
      {
        return item;
      }
    }

    return null;
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
      windRay.gameObject.SetActive(false);

      windRays[i] = windRay; 
    }
  }

  private IEnumerator Delay(float time, Action action)
  {
    yield return new WaitForSeconds(time);
    action?.Invoke();
  }

  private IEnumerator ObstacleInterval()
  {
    float elapsedTime = 0f;

    while (elapsedTime < obstacleInterval)
    {
      elapsedTime += Time.deltaTime;

      if (elapsedTime >= obstacleInterval)
      {
        ThrowObstacle();
        elapsedTime = 0f;
      }

      yield return null;
    }
  }
}
