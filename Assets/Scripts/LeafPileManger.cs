using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Source: https://en.wikipedia.org/wiki/DBSCAN
public class LeafPileManager : MonoBehaviour
{
  [Header("Attributes")]
  public float eps = 3f;
  public int minLeafs = 3;
  public float scanRate = 1f;

  public List<Leaf> leaves = new();

  private List<LeafPile> leafPiles = new();
  private int clusterCounter = 0;

  Func<Vector2, Vector2, float> distFunc = (q, p) =>
  {
    return Vector2.Distance(p, q);
  };

  public static LeafPileManager Instance { get; private set; }

  private void Awake()
  {
    // Check if an instance already exists
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;

    DontDestroyOnLoad(gameObject);
  }

  private void Start()
  {
    StartCoroutine(ClusterScan(scanRate));
  }

  private IEnumerator ClusterScan(float rate)
  {
    float elapsedTime = 0f;
    while (elapsedTime < rate)
    {
      elapsedTime += Time.deltaTime;

      if (elapsedTime >= rate)
      {
        elapsedTime = 0f;

        leaves.ForEach((leaf) => leaf.label = null);
        leafPiles = new();

        DBSCAN(leaves, distFunc, eps, minLeafs);
      }

      yield return null;
    }
  }

  private void DBSCAN(List<Leaf> leaves, Func<Vector2, Vector2, float> distFunc, float eps, int minLeafs)
  {
    clusterCounter = 0;
    foreach (Leaf leaf in leaves)
    {
      if (leaf.label != null) continue;

      LeafPile leafPile = RangeQuery(leaves, distFunc, leaf, eps);

      if (leafPile.leaves.Count < minLeafs)
      {
        leaf.label = -1;
        continue;
      }

      clusterCounter += 1;

      leaf.label = clusterCounter;

      //Seed set without current leaf
      List<Leaf> seedSetLeafPile = leafPile.leaves.Where(i => i != leaf).ToList();

      for (int i = 0; i < seedSetLeafPile.Count; i++)
      {
        Leaf qLeaf = seedSetLeafPile[i];

        if (qLeaf.label == -1) qLeaf.label = clusterCounter;
        if (qLeaf.label != null) continue;

        qLeaf.label = clusterCounter;

        LeafPile qLifePile = RangeQuery(leaves, distFunc, qLeaf, eps);

        if (qLifePile.leaves.Count >= minLeafs)
        {
          seedSetLeafPile.AddRange(qLifePile.leaves);
        }
      }
    }
  }

  private void SubScan(List<Leaf> leaves, Func<Vector2, Vector2, float> distFunc, float eps, int minLeafs)
  {
    foreach (Leaf qLeaf in leaves)
    {
      if (qLeaf.label == -1) qLeaf.label = clusterCounter;
      if (qLeaf.label != null) continue;

      qLeaf.label = clusterCounter;

      LeafPile qLifePile = RangeQuery(leaves, distFunc, qLeaf, eps);

      if (qLifePile.leaves.Count >= minLeafs)
      {
        SubScan(qLifePile.leaves, distFunc, eps, minLeafs);
      }
    }
  }

  private LeafPile RangeQuery(List<Leaf> leaves, Func<Vector2, Vector2, float> distFunc, Leaf qLeaf, float eps)
  {
    LeafPile leafPile = new();
    foreach (Leaf leaf in leaves)
    {
      float dis = distFunc(qLeaf.GetPosition(), leaf.GetPosition());
      
      Debug.Log(dis);
      
      if (dis <= eps)
      {
        leafPile.leaves.Add(leaf);
      }
    }

    leafPiles.Add(leafPile);

    return leafPile;
  }
}
