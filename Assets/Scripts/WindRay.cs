using Unity.VisualScripting;
using UnityEngine;

public class WindRay : MonoBehaviour
{
  [SerializeField] private LayerMask layerMask;
  [SerializeField] private float distance = 1000f;

  private Ray ray;

  private void FixedUpdate()
  {
    DetectHit();
  }  

  private void DetectHit()
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, distance, layerMask);

    if (hit)
    {
      Debug.Log(hit.transform.name);
    }
  }
}
