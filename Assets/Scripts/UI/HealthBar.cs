using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  [SerializeField] private Image fill;
  private Slider slider;
  
  private void Awake()
  {
    slider = GetComponent<Slider>();
  }

  public void SetValue(float value)
  {
    if (slider == null) return;

    Color color = Color.Lerp(Color.blue, Color.orange, value); 
    
    slider.value = value;
    fill.color = color;
  }
}
