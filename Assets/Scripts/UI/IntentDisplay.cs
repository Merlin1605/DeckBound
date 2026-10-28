using System.Collections.Generic;
using DeckBound.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBound.UI
{
    // Arata in bubble cate o iconita + numar pentru fiecare efect al mutarii urmatoare
    // (ex. sabie 6, scut 5, poison 3). Se pune pe obiectul Intent_Bubble.
    // Iconitele si textele se creeaza din cod; bubble-ul se latește singur dupa continut.
    public class IntentDisplay : MonoBehaviour
    {
        [Header("Iconite")]
        [SerializeField] private Sprite attackSprite;
        [SerializeField] private Sprite armorSprite;
        [SerializeField] private Sprite poisonSprite;
        [SerializeField] private Sprite vulnerableSprite;

        [Header("Aspect")]
        [SerializeField] private float iconSize = 56f;
        [SerializeField] private float fontSize = 34f;
        [SerializeField] private Color textColor = Color.white;
        [Tooltip("Distanta dintre efecte.")]
        [SerializeField] private float spacing = 14f;
        [SerializeField] private int padding = 12;

        private class Entry
        {
            public GameObject Root;
            public Image Icon;
            public TMP_Text Value;
        }

        private readonly List<Entry> entries = new List<Entry>();

        private void Awake()
        {
            var layout = GetComponent<HorizontalLayoutGroup>();
            if (layout == null) layout = gameObject.AddComponent<HorizontalLayoutGroup>();
            layout.padding = new RectOffset(padding, padding, padding / 2, padding / 2);
            layout.spacing = spacing;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            var fitter = GetComponent<ContentSizeFitter>();
            if (fitter == null) fitter = gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        public void Show(EnemyMove move)
        {
            int count = 0;
            if (move != null)
            {
                if (move.damage > 0) SetEntry(count++, attackSprite, move.damage);
                if (move.armor > 0) SetEntry(count++, armorSprite, move.armor);
                if (move.poison > 0) SetEntry(count++, poisonSprite, move.poison);
                if (move.vulnerable > 0) SetEntry(count++, vulnerableSprite, move.vulnerable);
            }

            for (int i = count; i < entries.Count; i++) entries[i].Root.SetActive(false);
        }

        private void SetEntry(int index, Sprite sprite, int value)
        {
            while (entries.Count <= index) entries.Add(CreateEntry());

            var entry = entries[index];
            entry.Root.SetActive(true);
            entry.Icon.sprite = sprite;
            entry.Icon.enabled = sprite != null;
            entry.Value.text = value.ToString();
        }

        private Entry CreateEntry()
        {
            var root = new GameObject("IntentEntry", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            root.transform.SetParent(transform, false);
            var rowLayout = root.GetComponent<HorizontalLayoutGroup>();
            rowLayout.spacing = 4f;
            rowLayout.childAlignment = TextAnchor.MiddleCenter;
            rowLayout.childControlWidth = true;
            rowLayout.childControlHeight = true;
            rowLayout.childForceExpandWidth = false;
            rowLayout.childForceExpandHeight = false;

            var iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            iconGo.transform.SetParent(root.transform, false);
            var icon = iconGo.GetComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            var iconLayout = iconGo.GetComponent<LayoutElement>();
            iconLayout.preferredWidth = iconSize;
            iconLayout.preferredHeight = iconSize;

            var textGo = new GameObject("Value", typeof(RectTransform), typeof(TextMeshProUGUI));
            textGo.transform.SetParent(root.transform, false);
            var text = textGo.GetComponent<TextMeshProUGUI>();
            text.fontSize = fontSize;
            text.fontStyle = FontStyles.Bold;
            text.color = textColor;
            text.alignment = TextAlignmentOptions.MidlineLeft;
            text.raycastTarget = false;
#if UNITY_6000_0_OR_NEWER
            text.textWrappingMode = TextWrappingModes.NoWrap;
#else
            text.enableWordWrapping = false;
#endif

            return new Entry { Root = root, Icon = icon, Value = text };
        }
    }
}
