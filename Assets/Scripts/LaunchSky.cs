using UnityEngine;

public class LaunchSky : MonoBehaviour
{
    [Header("Camera Movement")]
    public float ascendSpeed = 50f;
    public float targetHeight = 1000f;

    [Header("Sky Fade")]
    public Renderer skyRenderer;           // Assign the inverted_sphere's Renderer
    public float fadeStartHeight = 100f;
    public float fadeEndHeight = 500f;

    private Material skyMaterial;
    private Color originalColor;

    void Start()
    {
        if (skyRenderer != null)
        {
            skyMaterial = skyRenderer.material; // Get a unique instance
            originalColor = skyMaterial.color;
            SetAlpha(0f); // Start fully transparent
        }
    }

    void Update()
    {
        // === Camera Ascension ===
        if (transform.position.y < targetHeight)
        {
            Vector3 newPos = transform.position;
            newPos.y += ascendSpeed * Time.deltaTime;
            transform.position = newPos;
        }

        // === Fade In Sky Sphere ===
        if (skyMaterial != null)
        {
            float height = transform.position.y;
            float t = Mathf.InverseLerp(fadeStartHeight, fadeEndHeight, height);
            float alpha = Mathf.Clamp01(t);
            SetAlpha(alpha);
        }
    }

    void SetAlpha(float alpha)
    {
        Color c = skyMaterial.color;
        c.a = alpha;
        skyMaterial.color = c;
    }
}
