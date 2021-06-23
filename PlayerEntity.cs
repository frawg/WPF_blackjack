using System.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace blackjack_simple_obj{
    public class PlayerEntity : IEquatable<PlayerEntity>, INotifyPropertyChanged
    {
        private List<Card> _cards_on_hand;
        private int _seat_pos;
        private int _score;
        private int _id;

        public List<Card> Cards_On_Hand
        {
            get => _cards_on_hand;
        }
        private int _Hand_Value()
        {
            return _cards_on_hand.Sum(x => x.Card_Value > 10 ? 10 : x.Card_Value);
        }

        public int Hand_Value
        {
            get => _cards_on_hand.Sum(x => Determine_Value(x));
        }

        private int _No_Of_Aces()
        {
            return _cards_on_hand.Count(x => x.Card_Value == 1);
        }

        public PlayerEntity()
        {
            _id = CustomRandomizer.GetNextInt();
            _cards_on_hand = new List<Card>();
        }

        private int Determine_Value(Card c)
        {
            if (c.Card_Value == 1 && 
                _cards_on_hand.IndexOf(c) == _cards_on_hand.FindIndex(x => x.Card_Value == 1) &&
                 _Hand_Value() - 1 <= 10)
                return 11;
            else
                return c.Card_Value > 10 ? 10 : c.Card_Value;
        }

        public int Seat_Pos
        {
            get => _seat_pos;
            set
            {
                if (value >= 0)
                {
                    _seat_pos = value;
                    OnPropertyChanged(nameof(Seat_Pos));
                }
            }
        }

        public int Score
        {
            get => _score;
            set
            {
                if (value >= 0)
                {
                    _score = value;
                }
                else 
                {
                    _score = 0;
                }
                OnPropertyChanged(nameof(Score));
            }
        }

        public void AddCardToHand(Card c)
        {
            _cards_on_hand.Add(c);
            OnPropertyChanged(nameof(Cards_On_Hand));
            OnPropertyChanged(nameof(CardsOnHandToString));
            OnPropertyChanged(nameof(Hand_Value));
        }

        public int ID
        {
            get => _id;
        }

        public PlayerEntity(int seat, int score)
        {
            this._id = CustomRandomizer.GetNextInt();
            this._seat_pos = seat;
            this._score = score;
            _cards_on_hand = new List<Card>();
        }
        
        public string CardsOnHandToString
        {
            get
            {
                string res = "";
                foreach (Card c in Cards_On_Hand)
                {
                    res += c.ToString() + " ";
                }
                return res.Trim();
            }
        }

        public override int GetHashCode()
        {
            return _id;
        }
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            PlayerEntity objAsPlayer = obj as PlayerEntity;
            if (objAsPlayer == null) return false;
            else return Equals(objAsPlayer);
        }

        public bool Equals(PlayerEntity other)
        {
            if (other == null) return false;
            return (this.ID.Equals(other.ID));
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