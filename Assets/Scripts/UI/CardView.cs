using System;
using DeckBound.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DeckBound.UI
{
    // Se pune pe root-ul prefabului Card. Afiseaza o carte si trimite click-ul la CombatUI.
    // Pozitia, rotatia si scala sunt controlate de HandLayout.
    public class CardView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Referinte din prefab")]
        [SerializeField] private Image artImage;
        [SerializeField] private Image frameImage;
        [SerializeField] private TMP_Text costText;
        [Tooltip("Optional, daca prefabul are text pentru nume.")]
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text descriptionText;

        [Header("Rame pe tip")]
        [SerializeField] private Sprite attackFrame;
        [SerializeField] private Sprite skillFrame;

        [Header("Feedback")]
        [SerializeField, Range(0f, 1f)] private float unplayableAlpha = 0.5f;

        private CanvasGroup canvasGroup;
        private Action<Card> onClicked;
        private bool playable;

        public Card Card { get; private set; }
        public TMP_Text DescriptionText => descriptionText;

        // Nu in Awake: Awake nu ruleaza daca obiectul e creat sub un parinte inactiv.
        private CanvasGroup Group
        {
            get
            {
                if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
                return canvasGroup;
            }
        }

        public void Bind(Card card, Action<Card> onClick)
        {
            Card = card;
            onClicked = onClick;

            if (artImage != null && card.Data.art != null) artImage.sprite = card.Data.art;
            if (frameImage != null)
            {
                var frame = card.Data.type == CardType.Attack ? attackFrame : skillFrame;
                if (frame != null) frameImage.sprite = frame;
            }
            if (costText != null) costText.text = card.Cost.ToString();
            if (nameText != null) nameText.text = card.Name;
            if (descriptionText != null) descriptionText.text = Keywords.Highlight(card.Description);
        }

        public void SetPlayable(bool value)
        {
            playable = value;
            Group.alpha = value ? 1f : unplayableAlpha;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (playable && Card != null) onClicked?.Invoke(Card);
        }
    }
}
