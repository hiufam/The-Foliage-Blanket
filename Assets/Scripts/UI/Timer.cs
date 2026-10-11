using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
  public TextMeshProUGUI textDisplay;

  public void SetText(string text)
  {
    textDisplay.text = text;
  }  
}
