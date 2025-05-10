using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace VehicleAssembly
{
    public class ButtonController : MonoBehaviour
    {
        [Header("Button Groups")]
        public Button[] noseButtons, propellantButtons, controlButtons, stagesButtons;

        [Header("Styling")]
        public Color activeColor = Color.green;
        public Color inactiveColor = Color.white;

        [Header("Text Styling")]
        public Color selectedTextColor   = Color.white;
        public FontStyles selectedTextStyle = FontStyles.Bold;
        public Color normalTextColor     = Color.black;
        public FontStyles normalTextStyle   = FontStyles.Normal;

        void Start()
        {
            InitializeGroup(noseButtons,       BuilderController.RocketPart.Nose);
            InitializeGroup(propellantButtons, BuilderController.RocketPart.Propellant);
            InitializeGroup(controlButtons,    BuilderController.RocketPart.Control);
            InitializeGroup(stagesButtons,     BuilderController.RocketPart.Stage);
        }

        void InitializeGroup(Button[] buttons, BuilderController.RocketPart part)
        {
            // 1) hook up clicks
            for (int i = 0; i < buttons.Length; i++)
            {
                int idx = i;
                buttons[i].onClick.AddListener(() =>
                    OnButtonClicked(buttons, idx, part)
                );
            }

            // 2) find which one was saved
            string raw = GameData.currentBuild.TryGetValue(part, out var val)
                        ? val : "";
            string saved = raw.ToLowerInvariant().Trim();

            int defaultIndex = 0;
            for (int i = 0; i < buttons.Length; i++)
            {
                var tmp = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmp == null) continue;

                // normalize the label
                string label = tmp.text
                                .Replace("\r","")
                                .Replace("\n"," ")
                                .ToLowerInvariant()
                                .Trim();

                // **new**: match if either contains the other
                if (label == saved 
                || label.Contains(saved) 
                || saved.Contains(label))
                {
                    defaultIndex = i;
                    break;
                }
            }

            // 3) seed if missing
            if (!GameData.currentBuild.ContainsKey(part) && buttons.Length > 0)
            {
                var tmp0 = buttons[defaultIndex].GetComponentInChildren<TextMeshProUGUI>();
                string label0 = tmp0.text.Replace("\n", " ").ToLowerInvariant();
                GameData.currentBuild[part] = label0;
            }

            // 4) apply visuals
            UpdateGroupVisuals(buttons, defaultIndex);
        }

        void OnButtonClicked(Button[] buttons, int selectedIndex, BuilderController.RocketPart part)
        {
            UpdateGroupVisuals(buttons, selectedIndex);

            var tmp = buttons[selectedIndex].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                string label = tmp.text.Replace("\n", " ").ToLowerInvariant();
                GameData.currentBuild[part] = label;
            }
        }

        void UpdateGroupVisuals(Button[] buttons, int activeIndex)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                var img = buttons[i].GetComponent<Image>();
                if (img != null)
                    img.color = (i == activeIndex ? activeColor : inactiveColor);

                var txt = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (txt != null)
                {
                    if (i == activeIndex)
                    {
                        txt.color     = selectedTextColor;
                        txt.fontStyle = selectedTextStyle;
                    }
                    else
                    {
                        txt.color     = normalTextColor;
                        txt.fontStyle = normalTextStyle;
                    }
                }
            }
        }
    }
}
