using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace DeckBound.UI
{
    // Aseaza cartile din mana intr-un arc (ca in Slay the Spire).
    // La hover, cartea se ridica, se indreapta si se mareste; vecinii se dau la o parte.
    // Se pune pe obiectul Hand, intins pe tot Canvas-ul. Nu folosi Layout Group pe acelasi obiect.
    [RequireComponent(typeof(RectTransform))]
    public class HandLayout : MonoBehaviour
    {
        [Header("Arc")]
        [Tooltip("Scala cartilor in mana (prefabul e 400x560).")]
        [SerializeField] private float cardScale = 0.6f;
        [Tooltip("Raza cercului pe care stau cartile. Mai mare = arc mai plat.")]
        [SerializeField] private float arcRadius = 1800f;
        [Tooltip("Unghiul dintre doua carti vecine (grade).")]
        [SerializeField] private float anglePerCard = 5f;
        [Tooltip("Unghiul maxim al intregului evantai, ca mana mare sa nu se intinda prea mult.")]
        [SerializeField] private float maxFanAngle = 30f;
        [Tooltip("Inaltimea cartii din mijloc fata de marginea de jos a obiectului Hand.")]
        [SerializeField] private float baseY = 140f;

        [Header("Hover")]
        [SerializeField] private float hoverScale = 1f;
        [Tooltip("Spatiul dintre marginea de jos a cartii ridicate si pivotul obiectului Hand.")]
        [SerializeField] private float hoverBottomPadding = 10f;
        [Tooltip("Cat se impinge cartea vecina; urmatoarele primesc jumatate, o treime...")]
        [SerializeField] private float neighborPush = 90f;

        [Header("Animatie")]
        [SerializeField] private float moveSpeed = 14f;
        [Tooltip("Optional: de aici pornesc cartile trase (ex. iconita draw pile).")]
        [SerializeField] private RectTransform spawnPoint;

        private struct Pose
        {
            public Vector2 position;
            public float angle;
            public float scale;
        }

        private readonly List<CardView> cards = new List<CardView>();
        private readonly HashSet<CardView> placed = new HashSet<CardView>();
        private RectTransform rectTransform;
        private Canvas canvas;
        private CardView hovered;

        public CardView Hovered => hovered;

        // Mijlocul marginii de jos a dreptunghiului Hand, indiferent de pivot/ancore.
        private Vector2 Origin => new Vector2(rectTransform.rect.center.x, rectTransform.rect.yMin);

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
            canvas = GetComponentInParent<Canvas>();
        }

        // Ordinea cartilor din mana, de la stanga la dreapta. Apelat de CombatUI.
        public void SetCards(IReadOnlyList<CardView> views)
        {
            cards.Clear();
            cards.AddRange(views);

            foreach (var card in cards)
            {
                if (placed.Contains(card)) continue;
                var rt = (RectTransform)card.transform;
                rt.localPosition = spawnPoint != null
                    ? rectTransform.InverseTransformPoint(spawnPoint.position)
                    : (Vector3)(Origin + new Vector2(0f, baseY));
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one * (cardScale * 0.3f);
            }
            placed.Clear();
            placed.UnionWith(cards);

            if (hovered != null && !placed.Contains(hovered)) hovered = null;
            UpdateSiblingOrder();
        }

        private void Update()
        {
            UpdateHovered();

            float t = 1f - Mathf.Exp(-moveSpeed * Time.unscaledDeltaTime);
            int hoveredIndex = hovered != null ? cards.IndexOf(hovered) : -1;

            for (int i = 0; i < cards.Count; i++)
            {
                var pose = i == hoveredIndex ? HoverPose(i) : RestPose(i);
                if (hoveredIndex >= 0 && i != hoveredIndex)
                {
                    int distance = i - hoveredIndex;
                    pose.position.x += Mathf.Sign(distance) * neighborPush / Mathf.Abs(distance);
                }

                var rt = (RectTransform)cards[i].transform;
                rt.localPosition = Vector3.Lerp(rt.localPosition, pose.position, t);
                rt.localRotation = Quaternion.Slerp(rt.localRotation, Quaternion.Euler(0f, 0f, pose.angle), t);
                rt.localScale = Vector3.Lerp(rt.localScale, Vector3.one * pose.scale, t);
            }
        }

        private Pose RestPose(int index)
        {
            int count = cards.Count;
            float step = count > 1 ? Mathf.Min(anglePerCard, maxFanAngle / (count - 1)) : 0f;
            float angle = ((count - 1) * 0.5f - index) * step; // pozitiv = inclinata spre stanga
            float rad = angle * Mathf.Deg2Rad;

            return new Pose
            {
                position = Origin + new Vector2(-arcRadius * Mathf.Sin(rad), baseY + arcRadius * (Mathf.Cos(rad) - 1f)),
                angle = angle,
                scale = cardScale,
            };
        }

        private Pose HoverPose(int index)
        {
            var rest = RestPose(index);
            var rt = (RectTransform)cards[index].transform;
            // Marginea de jos a cartii ridicate sta la hoverBottomPadding deasupra marginii de jos a Hand.
            float y = Origin.y + hoverBottomPadding + rt.rect.height * rt.pivot.y * hoverScale;

            return new Pose { position = new Vector2(rest.position.x, y), angle = 0f, scale = hoverScale };
        }

        // Hover-ul se calculeaza din pozitiile de repaus, nu din cele animate,
        // ca sa nu clipeasca atunci cand cartea se ridica de sub cursor.
        private void UpdateHovered()
        {
            CardView next = null;

            if (TryGetPointer(out var local))
            {
                int hoveredIndex = hovered != null ? cards.IndexOf(hovered) : -1;
                if (hoveredIndex >= 0 && Contains(hoveredIndex, HoverPose(hoveredIndex), local))
                {
                    next = hovered;
                }
                else
                {
                    // Cartile din dreapta sunt desenate deasupra, deci le verificam primele.
                    for (int i = cards.Count - 1; i >= 0; i--)
                    {
                        if (Contains(i, RestPose(i), local))
                        {
                            next = cards[i];
                            break;
                        }
                    }
                }
            }

            if (next == hovered) return;
            hovered = next;
            UpdateSiblingOrder();
        }

        private bool Contains(int index, Pose pose, Vector2 localPoint)
        {
            var rt = (RectTransform)cards[index].transform;
            Vector2 offset = localPoint - pose.position;
            offset = Quaternion.Euler(0f, 0f, -pose.angle) * offset;
            offset /= pose.scale;
            return rt.rect.Contains(offset);
        }

        private bool TryGetPointer(out Vector2 local)
        {
            local = default;
            if (!TryGetPointerScreenPosition(out var screen)) return false;
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screen, EventCamera, out local);
        }

        // Camera pentru conversii ecran -> UI (null pentru Screen Space - Overlay).
        public Camera EventCamera =>
            canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay ? canvas.worldCamera : null;

        public static bool TryGetPointerScreenPosition(out Vector2 screen)
        {
#if ENABLE_INPUT_SYSTEM
            screen = default;
            if (Mouse.current == null) return false;
            screen = Mouse.current.position.ReadValue();
#else
            screen = Input.mousePosition;
#endif
            return true;
        }

        private void UpdateSiblingOrder()
        {
            for (int i = 0; i < cards.Count; i++)
                cards[i].transform.SetSiblingIndex(i);
            if (hovered != null) hovered.transform.SetAsLastSibling();
        }
    }
}
