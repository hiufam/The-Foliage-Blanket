using UnityEngine;
using UnityEngine.Pool;

// Source: https://docs.unity3d.com/6000.5/Documentation/ScriptReference/Pool.ObjectPool_1.html
public class SoundPool
{
  public ObjectPool<Sound> pool;
  public Sound prefab;

  public SoundPool(Sound prefab, int maxSize = 150, int defaultCapacity = 10)
  {
    this.prefab = prefab;

    pool = new ObjectPool<Sound>(
      createFunc: CreateItem,
      actionOnGet: OnGet,
      actionOnRelease: OnRelease,
      actionOnDestroy: OnDestroyItem,
      collectionCheck: true,   // helps catch double-release mistakes
      defaultCapacity: defaultCapacity,
      maxSize: maxSize
    );
  }

  private Sound CreateItem()
  {
    Sound pooledObject = GameObject.Instantiate(prefab, Vector2.zero, Quaternion.identity);

    pooledObject.gameObject.SetActive(false);
    return pooledObject;
  }

  // Called when an item is taken from the pool.
  private void OnGet(Sound pooledObject)
  {
    pooledObject.gameObject.SetActive(true);
  }

  // Called when an item is returned to the pool.
  private void OnRelease(Sound pooledObject)
  {
    pooledObject.gameObject.SetActive(false);
  }

  // Called when the pool decides to destroy an item (e.g., above max size).
  private void OnDestroyItem(Sound pooledObject)
  {
    GameObject.Destroy(pooledObject);
  }
}
