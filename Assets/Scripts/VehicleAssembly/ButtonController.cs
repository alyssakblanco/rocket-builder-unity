using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace VehicleAssembly
{
    public class ButtonController : MonoBehaviour
    {
        [Header("Button Groups")]
        public Button[] noseButtons;
        public Button[] propellantButtons;
        public Button[] controlButtons;
        public Button[] stagesButtons;

        [Header("Styling")]
        [Tooltip("Color for the selected/active button")]
        public Color activeColor = Color.green;
        [Tooltip("Color for all other buttons")]
        public Color inactiveColor = Color.white;

        [Header("Text Styling")]
        private Color selectedTextColor   = Color.white;
        private FontStyles selectedTextStyle = FontStyles.Bold;
        private Color normalTextColor     = Color.black;
        private FontStyles normalTextStyle   = FontStyles.Normal;

        void Start()
        {
            // Initialize each group
            InitializeGroup(noseButtons);
            InitializeGroup(propellantButtons);
            InitializeGroup(controlButtons);
            InitializeGroup(stagesButtons);
        }

        void InitializeGroup(Button[] buttons)
        {
            // 1) Wire up click listeners
            for (int i = 0; i < buttons.Length; i++)
            {
                int idx = i; // capture for closure
                buttons[i].onClick.AddListener(() => OnButtonClicked(buttons, idx));
            }

            // 2) Set the first one active by default
            if (buttons.Length > 0)
                UpdateGroupVisuals(buttons, 0);
        }

        void OnButtonClicked(Button[] buttons, int selectedIndex)
        {
            UpdateGroupVisuals(buttons, selectedIndex);
        }

        void UpdateGroupVisuals(Button[] buttons, int activeIndex)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                // 1) Button image
                var img = buttons[i].GetComponent<Image>();
                if (img != null)
                    img.color = (i == activeIndex ? activeColor : inactiveColor);

                // 2) Button text (TMPro)
                var tmp = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null)
                {
                    if (i == activeIndex)
                    {
                        tmp.color     = selectedTextColor;
                        tmp.fontStyle = selectedTextStyle;
                    }
                    else
                    {
                        tmp.color     = normalTextColor;
                        tmp.fontStyle = normalTextStyle;
                    }
                }
            }
        }
    }
}
