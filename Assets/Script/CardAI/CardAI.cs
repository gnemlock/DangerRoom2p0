/* Created by Matthew Francis Keating */

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CardAI
{
    using StringFormats = Utility.CardAIStringFormats;
    using Labels = Utility.CardAILabels;
    using Log = Utility.CardAIDebug;
    using Tags = Utility.CardAITags;

    #if UNITY_EDITOR
    using Tooltips = Utility.CardAITooltips;
    using Colours = Utility.CardAIColours;
    using Dimensions = Utility.CardAIDimensions;
    #endif

    public static class CardAI
    {
        public const int CARD_COUNT = 52;
        public const int SUIT_COUNT = 4;
        public const int VALUE_COUNT = 13;

        public static readonly Suit[] DEFAULT_SUITS
            = { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades};

        /// <summary>Converts a character-based card value to a numerical value.</summary>
        /// <returns>The numerical value associated with the character.</returns>
        /// <param name="charValue">The first letter of the face name associated with a card.
        /// </param>
        /// <remarks>Considers both upper and lower case; Considers Ace, Jack, Queen and King.
        /// All other characters return <value>0</value>.</remarks>
        public static int CharToValue(char charValue)
        {
            switch(charValue)
            {
                case 'a':
                case 'A':
                    return 1;
                    break;
                case 'j':
                case 'J':
                    return 11;
                    break;
                case 'q':
                case 'Q':
                    return 12;
                    break;
                case 'k':
                case 'K':
                    return 13;
                    break;
                default:
                    return 0;
            }
        }

        public static Suit[] ExpandSuitArray(Suit[] suitArray, int finalLength)
        {
            Suit[] newSuitArray = new Suit[finalLength];

            // Else, the final length is not disible by the current length, so we will assume 
            int i = 0;

            for( ; i < suitArray.Length; i++)
            {
                newSuitArray[i] = suitArray[i];
            }

            for( ; i < newSuitArray.Length; i++)
            {
                newSuitArray[i] = suitArray[suitArray.Length - 1];
            }

            return newSuitArray;
        }

        /// <summary>
        /// Expands the suit array evenly.
        /// </summary>
        /// <returns>The suit array evenly.</returns>
        /// <param name="suitArray">Suit array.</param>
        /// <param name="finalLength">Final length.</param>
        public static Suit[] ExpandSuitArrayEvenly(Suit[] suitArray, int finalLength)
        {
            Suit[] newSuitArray = new Suit[finalLength];
            int cardCountPerSuit = finalLength / suitArray.Length;
            int remainingCardCount = finalLength % suitArray.Length;

            for(int i = 0; i < suitArray.Length; i++)
            {
                for(int j = 0; j < cardCountPerSuit; j++)
                {
                    newSuitArray[(i * j) + j] = suitArray[i];
                }
            }

            if(remainingCardCount > 0)
            {
                for(int k = 0; k < remainingCardCount; k++)
                {
                    newSuitArray[(cardCountPerSuit * suitArray.Length) + k]
                        = suitArray[suitArray.Length - 1];
                }
            }

            return newSuitArray;
        }
    }

    public class Card : CardInterface
    {
        public Suit suit
        {
            get
            {
                return suit;
            }
            set
            {
                suit = value;
            }
        }

        public int cardValue
        {
            get
            {
                return cardValue;
            }
            set
            {
                cardValue = value;
            }
        }

        #region Operators
        public Card()
        {
            suit = Suit.Unspecified;
            cardValue = 0;
        }

        public Card(int cardValue)
        {
            suit = Suit.Unspecified;
            this.cardValue = cardValue;
        }

        public Card(int cardValue, Suit suit)
        {
            this.suit = suit;
            this.cardValue = cardValue;
        }
        #endregion

        public override bool Equals(Card card)
        {
            return (card.suit == suit && card.cardValue == cardValue);
        }

        public static bool operator ==(Card left, Card right)
        {
            return (left.suit == right.suit && left.cardValue == right.cardValue);
        }

        public static bool operator !=(Card left, Card right)
        {
            return !(left.suit == right.suit && left.cardValue == right.cardValue);
        }

        public static explicit operator int(Card card)
        {
            return card.cardValue;
        }

        public static explicit operator Suit(Card card)
        {
            return card.suit;
        }
    }

    public class PyramidCard : CardInterface
    {
        public Card card;
        public PyramidCard leftParent;
        public PyramidCard rightParent;
        public PyramidCard leftChild;
        public PyramidCard rightChild;

        public int cardValue
        {
            get
            {
                return card.cardValue;
            }
            set
            {
                card.cardValue = value;
            }
        }

        public Suit suit
        {
            get
            {
                return card.suit;
            }
            set
            {
                card.suit = value;
            }
        }

        public bool hasChildren
        {
            get
            {
                return (leftChild != null || rightChild != null);
            }
        }

        #region Constructors
        public PyramidCard()
        {
            card = new Card();
        }

        public PyramidCard(Card card)
        {
            this.card = card;
        }

        public PyramidCard(Card card, PyramidCard parent)
        {
            this.card = card;
            this.parent = parent;
        }

        public PyramidCard(Card card, PyramidCard leftChild, PyramidCard rightChild)
        {
            this.card = card;
            this.leftChild = leftChild;
            this.rightChild = rightChild;
        }

        public PyramidCard(Card card, PyramidCard parent, PyramidCard leftChild,
            PyramidCard rightChild)
        {
            this.card = card;
            this.parent = parent;
            this.leftChild = leftChild;
            this.rightChild = rightChild;
        }
        #endregion

        public int GetBlockedCardCount()
        {
            int blockedCardCount = 2;

            if(leftParent != null && leftParent.GetChildCount() < 2)
            {
                blockedCardCount--;
            }

            if(rightParent != null && rightParent.GetChildCount() < 2)
            {
                blockedCardCount--;
            }

            return blockedCardCount;
        }

        public PyramidCard[] GetBlockedCards()
        {
            int blockedCardCount = GetBlockedCardCount();

            if(blockedCardCount == 0)
            {
                return null;
            }
            else
            {
                PyramidCard[] blockedCards = new PyramidCard[blockedCardCount];

                if(blockedCardCount == 2)
                {
                    blockedCards[0] = leftParent;
                    blockedCards[1] = rightParent;
                }
                else if(leftParent != null && leftParent.GetChildCount() < 2)
                {
                    blockedCards[0] = leftParent;
                }
                else
                {
                    blockedCards[0] = rightParent;
                }

                return blockedCards;
            }
        }

        public int GetChildCount()
        {
            int childCount = 2;

            if(leftChild == null)
            {
                childCount--;
            }

            if(rightChild == null)
            {
                childCount--;
            }

            return childCount;
        }

        public int GetParentCount()
        {
            int parentCount = 2;

            if(leftParent == null)
            {
                parentCount--;
            }

            if(rightParent == null)
            {
                parentCount--;
            }

            return parentCount;
        }

        public void RemoveChild(PyramidCard child)
        {
            if(leftChild == child)
            {
                leftChild = null;
            }
            else
            {
                rightChild = null;
            }

            if(child.leftParent == this)
            {
                child.leftParent = null;
            }
            else
            {
                child.rightParent = null;
            }
        }

        #region Operators
        public override bool Equals(PyramidCard pyramidCard)
        {
            return (pyramidCard.suit == suit && pyramidCard.cardValue == cardValue);
        }

        public static bool operator ==(PyramidCard left, PyramidCard right)
        {
            return (left.suit == right.suit && left.cardValue == right.cardValue);
        }

        public static bool operator !=(PyramidCard left, PyramidCard right)
        {
            return !(left.suit == right.suit && left.cardValue == right.cardValue);
        }

        public static explicit operator int(PyramidCard card)
        {
            return card.cardValue;
        }

        public static explicit operator Suit(PyramidCard card)
        {
            return card.suit;
        }
        #endregion
    }

    public class Pyramid
    {
        public PyramidCard head;
        public PyramidCard[] openCards;

        public Suit[] GetOpenSuits()
        {
            Suit[] suits = new Suit[openCards.Length];

            for(int i = 0; i < suits.Length; i++)
            {
                suits[i] = openCards[i].suit;
            }

            return suits;
        }

        public int[] GetOpenValues()
        {
            int[] values = new int[openCards.Length];

            for(int i = 0; i < values.Length; i++)
            {
                values[i] = openCards[i].suit;
            }

            return values;
        }

        public bool RemoveCard(int index)
        {
            if(openCards[index].leftParent == null && openCards[index].rightParent == null)
            {
                return true;
            }

            int freedCardCount = CountBlockedCards(index);

            PyramidCard parent = openCards[index].parent;

            if(parent.hasChildren)
            {
                PyramidCard[] newOpenCards = new PyramidCard[openCards.Length - 1];

                if(index > 0)
                {
                    Array.Copy(openCards, newOpenCards, index);
                }

                if(index < openCards.Length - 1)
                {
                    Array.Copy(openCards, index + 1, newOpenCards, index + 1,
                        index - openCards.Length - 1);
                }

                openCards = newOpenCards;
            }
            else
            {
                openCards[index] = parent;
            }

            return false;
        }
    }

    public class Deck : CardInterface, DeckInterface
    {
        public Card[] cards;

        public int cardValue
        {
            get
            {
                return cards[place].cardValue;
            }
            set
            {
                //TODO: Debug Warning - can not change value from deck
            }
        }

        public Suit suit
        {
            get
            {
                return cards[place].suit;
            }
            set
            {
                //TODO: Debug Warning - can not change suit from deck
            }
        }

        /// <summary>The current player position, in the deck. Automatically rolls back to the
        /// start.</summary>
        public int place
        {
            get
            {
                return cardValue;
            }
            set
            {
                place = value % cards.Length;
            }
        }

        #region Constructors
        public Deck()
        {
            cards = new Card[CardAI.CARD_COUNT];
            place = 0;

            for(int i = 0; i < CardAI.CARD_COUNT; i++)
            {
                cards[i] = new Card((i + 1) % CardAI.VALUE_COUNT,
                    CardAI.DEFAULT_SUITS[i / CardAI.VALUE_COUNT]);
            }
        }

        public Deck(int cardCount)
        {
            cards = new Card[cardCount];
            place = 0;

            for(int i = 0; i < cardCount; i++)
            {
                cards[i] = new Card();
            }
        }

        public Deck(int[] cardValues, Suit suit = Suit.Unspecified)
        {
            cards = new Card[cardValues.Length];
            place = 0;

            for(int i = 0; i < cardValues.Length; i++)
            {
                cards[i] = new Card(cardValues[i], suit);
            }
        }

        public Deck(int[] cardValues, Suit[] suits)
        {
            cards = new Card[cardValues.Length];
            place = 0;

            if(cardValues.Length > suits.Length)
            {
                suits = CardAI.ExpandSuitArray(suits, cardValues.Length);
            }

            for(int i = 0; i < cardValues.Length; i++)
            {
                cards [i] = new Card(cardValues[i], suits[i]);
            }
        }

        public Deck(string cardValues, Suit suit = Suit.Unspecified)
        {
            int cardCount = cardValues.Length;
            place = 0;

            for(int i = 0; i < cardValues.Length; i++)
            {
                if (cardValues[i] == '0')
                {
                    // For each character, in the values string; if that character is a 0, it does 
                    // not represent a single card, as it is either an invalid value, or a 10.
                    // Adjust the card count to reflect this.
                    cardCount--;
                }
            }

            cards = new Card[cardCount];

            for(int cardIndex = 0, valueIndex = 0; valueIndex < cardValues.Length;
                cardIndex++, valueIndex++)
            {
                if(char.IsDigit(cardValues[valueIndex]))
                {
                    if(cardIndex != 0 && cardValues[valueIndex] == '0')
                    {
                        cards[cardIndex - 1].cardValue = 10;
                        cardIndex--;
                    }
                    else
                    {
                        cards[cardIndex]
                            = new Card(int.Parse(cardValues[valueIndex].ToString()), suit);
                    }
                }
                else
                {
                    cards[cardIndex] = new Card(CardAI.CharToValue(cardValues[valueIndex]), suit);
                }
            }
        }

        public Deck(string cardValues, Suit[] suits)
        {
            int cardCount = cardValues.Length;
            place = 0;

            for(int i = 0; i < cardValues.Length; i++)
            {
                if (cardValues[i] == '0')
                {
                    // For each character, in the values string; if that character is a 0, it does 
                    // not represent a single card, as it is either an invalid value, or a 10.
                    // Adjust the card count to reflect this.
                    cardCount--;
                }
            }

            if(cardCount > suits.Length)
            {
                suits = CardAI.ExpandSuitArray(suits, cardCount);
            }

            cards = new Card[cardCount];

            for(int cardIndex = 0, valueIndex = 0; valueIndex < cardValues.Length;
                cardIndex++, valueIndex++)
            {
                if(char.IsDigit(cardValues[valueIndex]))
                {
                    if(cardIndex != 0 && cardValues[valueIndex] == '0')
                    {
                        cards[cardIndex - 1].cardValue = 10;
                        cardIndex--;
                    }
                    else
                    {
                        cards[cardIndex] = new Card(int.Parse(cardValues[valueIndex].ToString()),
                            suits[cardIndex]);
                    }
                }
                else
                {
                    cards[cardIndex]
                        = new Card(CardAI.CharToValue(cardValues[valueIndex]), suits[cardIndex]);
                }
            }
        }
        #endregion

        public Card RemoveCard()
        {
            Card[] newCards = new Card[cards.Length - 1];

            if(place > 0)
            {
                Array.Copy(cards, newCards, place);
            }

            if(place < cards.Length - 1)
            {
                Array.Copy(cards, place + 1, newCards, place + 1, place - cards.Length - 1);
            }

            if(place == newCards.Length)
            {
                place--;
            }

            cards = newCards;

            return cards[place];
        }
    }

    public enum Suit
    {
        Unspecified,
        Clubs,
        Diamonds,
        Hearts,
        Spades
    };

    #region Interfaces
    public interface CardInterface
    {
        int cardValue { get; set; }
        Suit suit { get; set; }
    }

    public interface DeckInterface
    {
        int place { get; set; }
    }
    #endregion
}

namespace CardAI.Utility
{
    #if UNITY_EDITOR
    // Strings used to generate tooltips for the editor.
    public static class CardAITooltips
    {
    }

    // Colours for use in displaying custom editor GUI.
    public static class CardAIColours
    {
    }

    // Dimensions for use in displaying custom editor GUI.
    public static class CardAIDimensions
    {
    }
    #endif

    // Strings containing format for string interface display.
    public static partial class CardAIStringFormats
    {
    }
    
    // Strings used for general labelling.
    public static partial class CardAILabels
    {
    }
    
    // Provides debug functionality, including methods and customised string messages.
    public static partial class CardAIDebug
    {
    }
    
    // Strings used for tag or name comparison
    public static partial class CardAITags
    {
    }
}