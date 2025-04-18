using UnityEngine;

public class ChangeAlpha : MonoBehaviour
{
    // Set the desired alpha level (0 = fully transparent, 1 = fully opaque)
    [Range(0f, 1f)]
    public float targetAlpha = 0.5f;

    void Start()
    {
        // Get the Renderer component from the GameObject
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            
            Color c = mat.GetColor("_MainTex");
            c.a = targetAlpha;         
            mat.SetColor("_MainTex", c);
        }

    }
}
