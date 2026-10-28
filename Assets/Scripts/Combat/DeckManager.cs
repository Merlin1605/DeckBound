using System;
using System.Collections.Generic;

namespace DeckBound.Combat
{
    // Draw pile, mana si discard pile pentru o lupta.
    public class DeckManager
    {
        private readonly List<Card> drawPile = new List<Card>();
        private readonly List<Card> hand = new List<Card>();
        private readonly List<Card> discardPile = new List<Card>();

        public IReadOnlyList<Card> DrawPile => drawPile;
        public IReadOnlyList<Card> Hand => hand;
        public IReadOnlyList<Card> DiscardPile => discardPile;

        public event Action OnChanged;
        public event Action OnReshuffled;

        public DeckManager(IEnumerable<CardData> deck)
        {
            foreach (var data in deck)
            {
                if (data != null) drawPile.Add(new Card(data));
            }
            Shuffle(drawPile);
            OnChanged?.Invoke();
        }

        // Trage carti; cand draw pile e gol, discard-ul se amesteca inapoi in draw pile.
        public void Draw(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (drawPile.Count == 0)
                {
                    if (discardPile.Count == 0) break; // toate cartile sunt deja in mana
                    ReshuffleDiscardIntoDraw();
                }

                int last = drawPile.Count - 1;
                hand.Add(drawPile[last]);
                drawPile.RemoveAt(last);
            }
            OnChanged?.Invoke();
        }

        public void Discard(Card card)
        {
            if (!hand.Remove(card)) return;
            discardPile.Add(card);
            OnChanged?.Invoke();
        }

        public void DiscardHand()
        {
            discardPile.AddRange(hand);
            hand.Clear();
            OnChanged?.Invoke();
        }

        private void ReshuffleDiscardIntoDraw()
        {
            drawPile.AddRange(discardPile);
            discardPile.Clear();
            Shuffle(drawPile);
            OnReshuffled?.Invoke();
        }

        // Fisher-Yates
        private static void Shuffle(List<Card> cards)
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }
    }
}
