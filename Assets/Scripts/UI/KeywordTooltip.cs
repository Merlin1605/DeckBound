using DeckBound.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBound.UI
{
    // Cand cursorul sta pe un cuvant cheie din descrierea cartii ridicate (ex. Armor, Lifesteal),
    // intuneca fundalul si arata o casuta cu explicatia, deasupra cartii.
    // Se pune pe obiectul Hand, langa HandLayout. Daca referintele sunt goale,
    // overlay-ul si casuta se creeaza automat din cod (poti sa le faci si de mana, ca sa le stilizezi).
    [RequireComponent(typeof(HandLayout))]
    public class KeywordTooltip : MonoBehaviour
    {
        [Header("Referinte")]
        [Tooltip("Image negru, intins pe tot Hand-ul.")]
        [SerializeField] private Image dimOverlay;
        [Tooltip("Casuta tooltip-ului (Image cu fundal).")]
        [SerializeField] private RectTransform panel;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Aspect (folosit cand casuta se creeaza din cod)")]
        [Tooltip("Optional: sprite pentru fundalul casutei (ex. intent_bubble, Sliced).")]
        [SerializeField] private Sprite panelSprite;
        [SerializeField] private Color panelColor = new Color(0.08f, 0.06f, 0.05f, 0.95f);
        [SerializeField] private Color titleColor = new Color(0.91f, 0.71f, 0.29f);
        [SerializeField] private float panelWidth = 380f;
        [SerializeField] private float titleSize = 30f;
        [SerializeField] private float descriptionSize = 24f;

        [Header("Comportament")]
        [SerializeField, Range(0f, 1f)] private float dimAlpha = 0.65f;
        [Tooltip("Distanta dintre marginea de sus a cartii si casuta.")]
        [SerializeField] private float gapAboveCard = 20f;
        [SerializeField] private float fadeSpeed = 12f;

        private HandLayout hand;
        private RectTransform handRect;
        private CanvasGroup panelGroup;
        private Keywords.Keyword shown;
        private CardView shownOn;
        private float visibility;

        private void Awake()
        {
            hand = GetComponent<HandLayout>();
            handRect = (RectTransform)transform;

            if (dimOverlay == null) dimOverlay = CreateDimOverlay();
            if (panel == null) CreatePanel();

            panelGroup = panel.GetComponent<CanvasGroup>();
            if (panelGroup == null) panelGroup = panel.gameObject.AddComponent<CanvasGroup>();
            panel.pivot = new Vector2(0.5f, 0f);

            // Overlay-ul si casuta nu trebuie sa blocheze click-ul pe carte.
            dimOverlay.raycastTarget = false;
            foreach (var graphic in panel.GetComponentsInChildren<Graphic>(true)) graphic.raycastTarget = false;

            SetVisibility(0f);
        }

        // LateUpdate: dupa ce HandLayout a ales cartea ridicata si a mutat-o.
        private void LateUpdate()
        {
            var card = hand.Hovered;
            var keyword = FindKeywordUnderPointer(card);

            if (keyword != null && (keyword != shown || card != shownOn))
            {
                shown = keyword;
                shownOn = card;
                titleText.text = keyword.Title;
                descriptionText.text = keyword.Description;
                BringToFront(card);
            }
            else if (keyword == null)
            {
                shown = null;
            }

            float target = shown != null ? 1f : 0f;
            SetVisibility(Mathf.MoveTowards(visibility, target, fadeSpeed * Time.unscaledDeltaTime));

            if (visibility > 0f && shownOn != null) PlaceAbove(shownOn);
        }

        private Keywords.Keyword FindKeywordUnderPointer(CardView card)
        {
            if (card == null || card.DescriptionText == null) return null;
            if (!HandLayout.TryGetPointerScreenPosition(out var screen)) return null;

            var text = card.DescriptionText;
            int index = TMP_TextUtilities.FindIntersectingLink(text, screen, hand.EventCamera);
            if (index < 0) return null;

            return Keywords.Find(text.textInfo.linkInfo[index].GetLinkID());
        }

        // Ordinea in Hand: celelalte carti, overlay, cartea ridicata, casuta.
        private void BringToFront(CardView card)
        {
            dimOverlay.transform.SetAsLastSibling();
            card.transform.SetAsLastSibling();
            panel.SetAsLastSibling();
        }

        private void PlaceAbove(CardView card)
        {
            var corners = new Vector3[4];
            ((RectTransform)card.transform).GetWorldCorners(corners);
            Vector3 topCenter = handRect.InverseTransformPoint((corners[1] + corners[2]) * 0.5f);

            float halfWidth = panel.rect.width * 0.5f;
            Rect bounds = handRect.rect;
            float x = Mathf.Clamp(topCenter.x, bounds.xMin + halfWidth, bounds.xMax - halfWidth);

            panel.localPosition = new Vector3(x, topCenter.y + gapAboveCard, 0f);
        }

        private Image CreateDimOverlay()
        {
            var go = new GameObject("DimOverlay", typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(transform, false);
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(-2000f, -2000f); // mai mare decat ecranul, ca sa acopere tot
            rt.offsetMax = new Vector2(2000f, 2000f);

            var image = go.GetComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            return image;
        }

        private void CreatePanel()
        {
            var go = new GameObject("KeywordTooltip", typeof(RectTransform), typeof(Image),
                typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            panel = (RectTransform)go.transform;
            panel.SetParent(transform, false);
            panel.anchorMin = panel.anchorMax = new Vector2(0.5f, 0.5f);
            panel.sizeDelta = new Vector2(panelWidth, 100f);

            var background = go.GetComponent<Image>();
            background.sprite = panelSprite;
            background.type = panelSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            background.color = panelSprite != null ? Color.white : panelColor;

            var layout = go.GetComponent<VerticalLayoutGroup>();
            layout.padding = new RectOffset(20, 20, 16, 18);
            layout.spacing = 6f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;

            var fitter = go.GetComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            titleText = CreateText("Title", titleSize, titleColor, FontStyles.Bold);
            descriptionText = CreateText("Description", descriptionSize, Color.white, FontStyles.Normal);
        }

        private TMP_Text CreateText(string objectName, float size, Color color, FontStyles style)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(panel, false);

            var text = go.GetComponent<TextMeshProUGUI>();
            text.fontSize = size;
            text.color = color;
            text.fontStyle = style;
#if UNITY_6000_0_OR_NEWER
            text.textWrappingMode = TextWrappingModes.Normal;
#else
            text.enableWordWrapping = true;
#endif
            text.alignment = TextAlignmentOptions.TopLeft;
            return text;
        }

        private void SetVisibility(float value)
        {
            visibility = value;
            panelGroup.alpha = value;

            var color = dimOverlay.color;
            color.a = value * dimAlpha;
            dimOverlay.color = color;

            bool active = value > 0f;
            if (panel.gameObject.activeSelf != active) panel.gameObject.SetActive(active);
            if (dimOverlay.gameObject.activeSelf != active) dimOverlay.gameObject.SetActive(active);
        }
    }
}
