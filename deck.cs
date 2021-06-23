using System;
using System.ComponentModel;
using System.Linq;

namespace blackjack_simple_obj{
    public class Deck : INotifyPropertyChanged
    {
        public static int MaxValue()
        { return 52; }
        private Card[] _cards;

        public Card[] Cards
        { 
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged(nameof(Cards));
            }
        }

        int _curr_pos;

        public int Curr_pos
        { 
            get => _curr_pos; 
            set
            {
                _curr_pos = value;
                OnPropertyChanged(nameof(Curr_pos));
                OnPropertyChanged(nameof(Card_Remaining));
            }
        }

        public Deck()
        {
            _curr_pos = 0;
            _cards = new Card[MaxValue()];
            for(int card = 0; card < Card.MaxValue(); card++)
            {
                for(int suit = 0; suit < blackjack_simple_obj.SuitUtility.MaxSuits(); suit++)
                {
                    _cards[card * 4 + suit] = new Card(suit + 1, card + 1);
                }
            }
        }

        public static Card[] Shuffle_Deck(Card[] cards)
        {
            return cards.OrderBy(x => CustomRandomizer.GetNextInt()).ToArray();
        }

        public Card Next_card()
        {
            Card res = _cards[Curr_pos];
            Curr_pos += 1;
            return res;
        }

        public int Card_Remaining
        {
            get
            {
                return MaxValue() - _curr_pos;
            }
        }

        public override string ToString()
        {
            string res = "[";
            foreach (Card c in _cards)
            {
                res += (c.ToString() + ", ");
            }
            res.TrimEnd();
            return res + "]";
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