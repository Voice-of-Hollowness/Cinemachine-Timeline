using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public LayerMask targetLayer;
    public SpriteRenderer dot;
    public Color dotHighlight;
  private Color originalDotColor;



    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }



    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray,100,targetLayer))
        {
            dot.color = dotHighlight;
        }
        else
        {
            dot.color = originalDotColor;
        }
    }
}
