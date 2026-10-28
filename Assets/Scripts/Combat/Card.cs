namespace DeckBound.Combat
{
    // O copie a unei carti in pachetul din lupta. Mai multe Card pot folosi acelasi CardData
    // (ex. 3x Lovitură). Aici vor sta mai tarziu upgrade-urile/buff-urile per carte.
    public class Card
    {
        public CardData Data { get; }

        public Card(CardData data)
        {
            Data = data;
        }

        public string Name => Data.cardName;
        public int Cost => Data.energyCost;
        public int Damage => Data.damage;
        public int Armor => Data.armor;
        public int Lifesteal => Data.lifesteal;
        public int Poison => Data.poison;
        public int Focus => Data.focus;
        public int Vulnerable => Data.vulnerable;
        public string Description => Data.GetDescription();
    }
}
