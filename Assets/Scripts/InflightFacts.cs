using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class RandomFactSlider : MonoBehaviour
{

    public static readonly string[] randomFacts = new[]
    {
       "The Kármán line, 100 km above Earth, is where space begins!",
       "Low Earth Orbit (LEO) is from about 160 km to 2,000 km up; this is where most satellites (and the ISS) fly.",
       "The International Space Station (ISS) orbits at roughly 420 km high and goes all the way around Earth in about 90 minutes.",
       "The Saturn V rocket stood 110 m tall and is still the biggest rocket ever flown.",
       "Most rockets reach their target Low Earth Orbit (LEO) in about 10 minutes!"
    };

    [Header("UI References")]
    public Canvas uiCanvas;
    public Sprite backgroundSprite;
    
    [Header("TextMeshPro Settings")]
    public TMP_FontAsset textFont;
    public float fontSize;

    [Header("Layout / Timing")]
    public Vector2 panelSize;
    public float slideDuration;
    public float excludeCenterWidth;


    public void ShowRandomFact()
    {
        if (randomFacts == null || randomFacts.Length == 0 || uiCanvas == null)
            return;
        StartCoroutine(CreateAndSlideFact());
    }

    private IEnumerator CreateAndSlideFact()
    {
        // 1) Pick a random fact
        string fact = randomFacts[Random.Range(0, randomFacts.Length)];

        // 2) Build the panel
        GameObject panel = new GameObject("FactPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(uiCanvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.sizeDelta = panelSize;

        Image bg = panel.GetComponent<Image>();
        bg.sprite = backgroundSprite;
        bg.type = Image.Type.Sliced;

        // 3) Add the TextMeshProUGUI component
        GameObject textGO = new GameObject("FactText", typeof(RectTransform), typeof(TextMeshProUGUI));
        textGO.transform.SetParent(panel.transform, false);

        TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.font = textFont;
        tmp.fontSize = fontSize;
        tmp.text = fact;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.color = Color.black;

        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(10, 10);
        textRect.offsetMax = new Vector2(-10, -10);

        // Compute allowed X ranges excluding the center zone
        RectTransform canvasRect = uiCanvas.GetComponent<RectTransform>();
        float halfCanvasW = canvasRect.rect.width * 0.5f;
        float halfPanelW = panelSize.x * 0.5f;
        float minX = -halfCanvasW + halfPanelW;
        float maxX =  halfCanvasW - halfPanelW;
        float halfExclude = excludeCenterWidth * 0.5f;
        float leftMax = Mathf.Clamp(-halfExclude, minX, maxX);
        float rightMin = Mathf.Clamp(halfExclude, minX, maxX);

        // Determine randomX in left or right segment weighted by width
        float leftWidth = leftMax - minX;
        float rightWidth = maxX - rightMin;
        float pick = Random.Range(0f, leftWidth + rightWidth);
        float randomX;
        if (pick < leftWidth)
            randomX = Random.Range(minX, leftMax);
        else
            randomX = Random.Range(rightMin, maxX);

        float startY = canvasRect.rect.height * 0.5f + panelSize.y * 0.5f;
        float endY   = -startY;

        panelRect.anchoredPosition = new Vector2(randomX, startY);

        // Slide it down
        float elapsed = 0f;
        while (elapsed < slideDuration)
        {
            float t = elapsed / slideDuration;
            float y = Mathf.Lerp(startY, endY, t);
            panelRect.anchoredPosition = new Vector2(randomX, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final position and destroy
        panelRect.anchoredPosition = new Vector2(randomX, endY);
        Destroy(panel);
    }
}
