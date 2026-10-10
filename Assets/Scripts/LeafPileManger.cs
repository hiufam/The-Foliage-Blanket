using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

// Source: https://en.wikipedia.org/wiki/DBSCAN
public class LeafPileManager : MonoBehaviour
{
  [Header("Attributes")]
  public float eps = 3f;
  public int minLeafs = 3;
  public float scanRate = 1f;

  public List<Leaf> leaves = new();
  public List<GameObject> polys = new();

  private List<LeafPile> leafPiles = new();
  private int clusterCounter = 0;
  private LayerMask layer;

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
  }

  private void Start()
  {
    layer = LayerMask.GetMask("Env");

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

        DBSCAN(leaves, distFunc, eps, minLeafs);

        ResetPoly();

        for (int i = 0; i < clusterCounter; i++)
        {
          int clusterIndex = i + 1;
          Leaf[] sortedLeaves = leaves.Where(l => l.label.Value == clusterIndex).ToArray();

          ConstructConvexPolygon(sortedLeaves);
        }
      }

      yield return null;
    }
  }

  private void ConstructConvexPolygon(Leaf[] leaves)
  {
    GameObject poly = new GameObject()
    {
      layer = 6,
    };

    PolygonCollider2D cl = poly.AddComponent<PolygonCollider2D>();
    List<Vector2> points = leaves.Select(leaf => leaf.GetPosition()).ToList();
    List<Vector2> convexHullPoints = ComputeConvexHull(points);
    
    cl.points = convexHullPoints.ToArray();
    cl.excludeLayers = layer;
    cl.includeLayers = ~layer;

    polys.Add(poly);
  }

  private void ResetCluster()
  {
    clusterCounter = 0;

    leaves.ForEach((leaf) => leaf.label = null);
    leafPiles = new();
  }

  private void ResetPoly()
  { 
    polys.ForEach((p) => Destroy(p.gameObject));

    polys = new();
  }

  private void DBSCAN(List<Leaf> leaves, Func<Vector2, Vector2, float> distFunc, float eps, int minLeafs)
  {
    ResetCluster();

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

      if (dis <= eps)
      {
        leafPile.leaves.Add(leaf);
      }
    }

    leafPiles.Add(leafPile);

    return leafPile;
  }

  // Source: https://loyc.net/2014/2d-convex-hull-in-cs.html?st_source=ai_overview
  private List<Vector2> ComputeConvexHull(List<Vector2> points, bool sortInPlace = false)
  {
    if (!sortInPlace)
    {
      points = new List<Vector2>(points);
      points.Sort((a, b) => a.x == b.x ? a.y.CompareTo(b.y) : (a.x > b.x ? 1 : - 1));
    }

    List<Vector2> hull = new List<Vector2>();
    int l = 0, u = 0;

    for (int i = points.Count - 1; i >= 0; i--)
    {
      Vector2 p = points[i], p1;

      while (l >= 2 && Cross((p1 = hull.Last()) - hull[hull.Count - 2], p - p1) >= 0)
      {
        hull.RemoveAt(hull.Count - 1);
        l--; 
      }

      hull.Add(p);
      l++;

      while (u >= 2 && Cross((p1 = hull.First()) - hull[1], p - p1) <= 0)
      {
        hull.RemoveAt(0);
        u--;
      }

      if (u != 0)
      {
        hull.Insert(0, p);
      }

      u++;
    }

    hull.RemoveAt(hull.Count - 1);
    return hull;
  }

  public float Cross(Vector2 a, Vector2 b)
  {
    return a.x * b.y - a.y * b.x;
  }
}
