using UnityEngine;
using UnityEngine.UI;
  
public class AlterHitAlphaButton : MonoBehaviour
{
    [SerializeField] Image theButton;
     
    void Awake () 
    {
        theButton.alphaHitTestMinimumThreshold = 0.5f;
    }
}