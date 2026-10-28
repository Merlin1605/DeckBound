using System.Collections.Generic;
using DeckBound.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckBound.UI
{
    // Leaga CombatManager de UI-ul din Canvas. Inlocuieste CombatDebugUI.
    public class CombatUI : MonoBehaviour
    {
        [SerializeField] private CombatManager combat;

        [Header("Mana")]
        [Tooltip("Obiectul Hand, cu HandLayout pe el.")]
        [SerializeField] private RectTransform handContainer;
        [Tooltip("Optional; daca e gol se ia de pe Hand Container.")]
        [SerializeField] private HandLayout handLayout;
        [SerializeField] private CardView cardPrefab;

        [Header("Erou / inamic")]
        [SerializeField] private HealthBarView heroHealthBar;
        [SerializeField] private HealthBarView enemyHealthBar;
        [SerializeField] private GameObject enemyIntentRoot;
        [SerializeField] private TMP_Text enemyIntentText;
        [Tooltip("Optional: iconita intentiei, schimbata dupa tipul mutarii.")]
        [SerializeField] private Image enemyIntentIcon;
        [SerializeField] private Sprite intentAttackSprite;
        [SerializeField] private Sprite intentDefendSprite;
        [Tooltip("Optional: pentru mutari fara atac care aplica poison.")]
        [SerializeField] private Sprite intentDebuffSprite;

        [Header("Resurse")]
        [SerializeField] private TMP_Text energyText;
        [SerializeField] private TMP_Text drawPileText;
        [SerializeField] private TMP_Text discardPileText;
        [Tooltip("Optional.")]
        [SerializeField] private TMP_Text turnText;

        [Header("Butoane")]
        [SerializeField] private Button endTurnButton;

        [Header("Final de lupta (optional)")]
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject defeatPanel;
        [SerializeField] private Button restartButton;

        [Header("Log (optional)")]
        [SerializeField] private TMP_Text logText;

        private readonly List<CardView> cardViews = new List<CardView>();
        private readonly List<CardView> activeViews = new List<CardView>();

        private void Awake()
        {
            if (combat == null) combat = FindObjectOfType<CombatManager>();
            if (handLayout == null && handContainer != null) handLayout = handContainer.GetComponent<HandLayout>();
        }

        private void OnEnable()
        {
            combat.OnCombatChanged += Refresh;
            if (endTurnButton != null) endTurnButton.onClick.AddListener(combat.EndPlayerTurn);
            if (restartButton != null) restartButton.onClick.AddListener(combat.StartCombat);
        }

        private void OnDisable()
        {
            combat.OnCombatChanged -= Refresh;
            if (endTurnButton != null) endTurnButton.onClick.RemoveListener(combat.EndPlayerTurn);
            if (restartButton != null) restartButton.onClick.RemoveListener(combat.StartCombat);
        }

        private void Start()
        {
            if (heroHealthBar != null) heroHealthBar.SetTarget(combat.Hero);
            if (enemyHealthBar != null) enemyHealthBar.SetTarget(combat.Enemy);
            Refresh();
        }

        private void Refresh()
        {
            if (combat.Deck == null) return;

            bool playerTurn = combat.State == CombatState.PlayerTurn;
            bool over = combat.State == CombatState.Victory || combat.State == CombatState.Defeat;

            RefreshHand();

            if (energyText != null) energyText.text = $"{combat.Energy}/{combat.MaxEnergy}";
            if (drawPileText != null) drawPileText.text = combat.Deck.DrawPile.Count.ToString();
            if (discardPileText != null) discardPileText.text = combat.Deck.DiscardPile.Count.ToString();
            if (turnText != null) turnText.text = $"Turn {combat.TurnNumber}";

            if (enemyIntentRoot != null) enemyIntentRoot.SetActive(!over && !combat.Enemy.IsDead);
            RefreshIntent();

            if (endTurnButton != null) endTurnButton.interactable = playerTurn;
            if (victoryPanel != null) victoryPanel.SetActive(combat.State == CombatState.Victory);
            if (defeatPanel != null) defeatPanel.SetActive(combat.State == CombatState.Defeat);

            if (logText != null) logText.text = string.Join("\n", combat.Log);
        }

        // Intentia arata mutarea pe care inamicul o va face in tura lui.
        private void RefreshIntent()
        {
            var move = combat.Enemy.NextMove;
            if (move == null) return;

            bool debuffOnly = !move.IsAttack && move.armor == 0 && move.poison > 0;

            if (enemyIntentText != null)
                enemyIntentText.text = move.IsAttack ? move.damage.ToString()
                    : debuffOnly ? move.poison.ToString()
                    : move.armor.ToString();

            if (enemyIntentIcon != null)
            {
                var sprite = move.IsAttack ? intentAttackSprite
                    : debuffOnly ? intentDebuffSprite
                    : intentDefendSprite;
                if (sprite != null) enemyIntentIcon.sprite = sprite;
            }
        }

        // Refoloseste CardView-urile existente in loc sa le distruga la fiecare refresh.
        private void RefreshHand()
        {
            var hand = combat.Deck.Hand;
            activeViews.Clear();

            while (cardViews.Count < hand.Count)
                cardViews.Add(Instantiate(cardPrefab, handContainer));

            for (int i = 0; i < cardViews.Count; i++)
            {
                var view = cardViews[i];
                bool used = i < hand.Count;
                view.gameObject.SetActive(used);
                if (!used) continue;

                view.Bind(hand[i], OnCardClicked);
                view.SetPlayable(combat.CanPlay(hand[i]));
                activeViews.Add(view);
            }

            if (handLayout != null) handLayout.SetCards(activeViews);
        }

        private void OnCardClicked(Card card)
        {
            // In evantai cartile se suprapun; jucam cartea ridicata, nu cea de sub ea.
            if (handLayout != null && handLayout.Hovered != null) card = handLayout.Hovered.Card;
            combat.PlayCard(card);
        }
    }
}
