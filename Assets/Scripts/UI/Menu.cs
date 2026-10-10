using UnityEngine;

public class Menu : MonoBehaviour
{
  private static readonly int Enable = Animator.StringToHash("Enable");
  private static readonly int Disable = Animator.StringToHash("Disable");

  private Animator animator;

  private void Awake()
  {
    animator = GetComponent<Animator>();
  }

  public void Open()
  {
    animator.ResetTrigger(Disable);
    animator.SetTrigger(Enable);
  }

  public void Hide()
  {
    animator.ResetTrigger(Enable);
    animator.SetTrigger(Disable);
  }
}
