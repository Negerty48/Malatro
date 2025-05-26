using System;

public class CardParser : IComparable<CardParser>
{
    public string Suit;
    public int Value;

    public CardParser(string Suit, int Value)
    {
        this.Suit = Suit;
        this.Value = Value;
    }

    public int CompareTo(CardParser otro)
    {
        return this.Value.CompareTo(otro.Value); // Menor a mayor
    }
}