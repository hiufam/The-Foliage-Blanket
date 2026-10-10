using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class WindArea : MonoBehaviour
{
  [Header("Attributes")]
  public float force = 100f;
  public float clusterDragForce = 50f;
  public float2 forceVariance = new Vector2(-10f, 10f);
  public float2 forceAngleVariance = new Vector2(-10f, 10f);
  public float interval = 1f;
  public int maxLeaves = 10;
  public float maxLeavesPercentage = 0.3f;
  public Vector2 windDir = Vector2.right;

  private float applyForceTimer = 0f;
  public List<GameObject> objects = new();

  private void Update()
  {
    if (applyForceTimer > interval)
    {
      applyForceTimer = 0f;

      ApplyForce();
    }
    
    applyForceTimer += Time.deltaTime;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    objects.Add(collision.gameObject);
  }

  
  private void OnTriggerExit2D(Collider2D collision)
  {
    if (objects.Contains(collision.gameObject))
    {
      objects.Remove(collision.gameObject);
    }
  }

  private List<int> RandomIndices(int max, int count)
  {
    System.Random random = new();
    List<int> randomNumbers = new List<int>();

    for (int i = 0; i < count; i++)
    {
      randomNumbers.Add(random.Next(0, max));
    }

    return randomNumbers;
  }

  public void ChangeDir(int dir)
  {
    switch (dir)
    {
      case 1: 
        windDir = Vector2.right;
        break;

      case 2: 
        windDir = Vector2.left;
        break;

      case 3: 
        windDir = Vector2.down;
        break;

      case 4: 
        windDir = Vector2.up;
        break;


      default:
        windDir = Vector2.right;
        break;
    }
  }

  private void ApplyForce()
  {
    if (objects.Count == 0) return;
    maxLeaves = (int)(objects.Count * maxLeavesPercentage);
    List<int> randomIndices = RandomIndices(objects.Count, maxLeaves);

    for (int i = 0; i < randomIndices.Count; i++)
    {
      GameObject go = objects[randomIndices[i]];
      if (go.TryGetComponent(out Leaf leaf))
      {
        Quaternion angle = Quaternion.Euler(0, 0, UnityEngine.Random.Range(forceAngleVariance.x, forceAngleVariance.y));
        int? label = leaf.label;
        
        float dragForce = (label != null && label.Value > 0) ? 1 : 0;

        float finalForce = force 
          + UnityEngine.Random.Range(forceVariance.x, forceVariance.y)
          - dragForce * clusterDragForce;

        leaf.ApplyForce(angle * windDir * finalForce);
      }
    }
  }
}
