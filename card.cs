using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace blackjack_simple_obj
{
    public class Card : IEquatable<Card>, INotifyPropertyChanged
    {
        public static int MaxValue()
        { return 13; }

        int _card_value;
        public int Card_Value
        { 
            get => _card_value;
            private set
            { 
                if (value > 0 && value <= MaxValue())
                { _card_value = value; }
            }
        }

        public int _id;

        public int ID
        { get => _id; }

        int _suit;
        public int Suit
        {
            get => _suit;
            set
            {
                _suit = value;
            }
        }

        private bool _status;

        public bool Is_Face_Up
        {
            get => _status;
            set
            {
                _status = value;
            }
        }

        public Card(int suit, int card)
        {
            _suit = suit;
            _card_value = card;
            _status = false;
            _id = CustomRandomizer.GetNextInt();
        }
        public Card(int suit, int card, bool status)
        {
            _suit = suit;
            _card_value = card;
            _status = status;
        }
        
        public override int GetHashCode()
        {
            return _id;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Card objAsCard = obj as Card;
            if (objAsCard == null) return false;
            else return Equals(objAsCard);
        }

        public bool Equals(Card other)
        {
            if (other == null) return false;
            return (this._id.Equals(other.ID));
        }

        public bool Equals(int other)
        {
            return (this.Card_Value.Equals(other));
        }

        public override string ToString()
        {
            /*return Is_Face_Up ? _card_value.ToString() : "*";*/
            return Is_Face_Up ? Card_Value.ToString() + SuitUtility.SuitToString(_suit) : "back";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}