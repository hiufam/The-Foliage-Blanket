using Unity.VisualScripting;
using UnityEngine;

public class WindRay : MonoBehaviour
{
  [SerializeField] private LayerMask layerMask;
  [SerializeField] private float distance = 1000f;
  public Vector2 direction = Vector2.right;
  public bool playerHit = false;

  private Ray ray;

  private void FixedUpdate()
  {
    DetectHit();
  }  

  private void DetectHit()
  {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, layerMask);
    playerHit = hit && hit.collider.gameObject.layer == 3;
  }
}
