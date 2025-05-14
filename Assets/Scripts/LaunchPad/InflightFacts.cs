using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class RandomFactSlider : MonoBehaviour
{
    public static readonly string[] randomFacts = new[]
    {
       "The Kármán line, 100 km above Earth, is where space begins!",
       "Low Earth Orbit (LEO) is from about 160 km to 2,000 km up; this is where most satellites (and the ISS) fly.",
       "The International Space Station (ISS) orbits at roughly 420 km high and goes all the way around Earth in about 90 minutes.",
       "The Saturn V rocket stood 110 m tall and is still the biggest rocket ever flown.",
       "Most rockets reach their target Low Earth Orbit (LEO) in about 10 minutes!",
       "To escape Earth’s gravity, a rocket must hit speeds over 28,000 km/h!",
       "The Moon is about 384,000 km from Earth—almost 1,000 times farther than the ISS!",
       "Astronauts experience ‘microgravity’—it feels like floating, but they’re actually falling!",
       "Sound can’t travel in space because there’s no air to carry the vibrations.",
       "Re-entry into Earth’s atmosphere heats spacecraft to over 1,600°C—hotter than lava!",
       "The first human in space was Yuri Gagarin in 1961—he orbited Earth once in 108 minutes!",
       "Earth’s atmosphere gets thinner with altitude—rockets need less lift the higher they go.",
       "Most rocket launches happen near the equator to take advantage of Earth’s rotation.",
       "The Hubble Space Telescope orbits at about 540 km—just above the ISS.",
       "In space, astronauts see about 16 sunrises and sunsets every day!"
    };

    // Runtime list of facts not yet shown
    private List<string> _unusedFacts;

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

    void Awake()
    {
        // Initialize the unused-facts list
        ResetFactPool();
    }

    /// <summary>
    /// Call this to re-populate the pool once you've shown them all.
    /// </summary>
    private void ResetFactPool()
    {
        _unusedFacts = new List<string>(randomFacts);
    }

    public void ShowRandomFact()
    {
        if (_unusedFacts == null || _unusedFacts.Count == 0 || uiCanvas == null)
            return;
        StartCoroutine(CreateAndSlideFact());
    }

    private IEnumerator CreateAndSlideFact()
    {
        // 1) Pick and remove a random fact from the unused pool
        if (_unusedFacts.Count == 0)
        {
            // all shown—reset for next cycle
            ResetFactPool();
        }

        int idx = Random.Range(0, _unusedFacts.Count);
        string fact = _unusedFacts[idx];
        _unusedFacts.RemoveAt(idx);

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
        float randomX = (pick < leftWidth)
            ? Random.Range(minX, leftMax)
            : Random.Range(rightMin, maxX);

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
