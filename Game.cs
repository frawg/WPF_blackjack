using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using blackjack_simple_obj;

namespace blackjack_obj{

    public delegate void Notify(string e);
    public class Game : INotifyPropertyChanged
    {
        private Deck[] _decks;

        public Deck[] Decks
        {
            get => _decks;
            set
            {
                _decks = value;
            }
        }

        private PlayerEntity[] _players;

        public PlayerEntity[] Players
        {
            get => _players;
        }

        private int _curr_multi_deck_pos;

        public int Multi_Deck_Curr_Pos
        {
            get => _curr_multi_deck_pos;
            set
            {
                _curr_multi_deck_pos = value;
                OnPropertyChanged(nameof(Multi_Deck_Curr_Pos));
                OnPropertyChanged(nameof(Multi_Deck_Cards_Remaining));
                OnPropertyChanged(nameof(Multi_Deck_Cards_Remaining_ToString));
            }
        }

        public List<PlayerEntity> LastGameWinners
        { get; set; }

        private int _curr_player_pos;

        public int Current_Player_Pos
        {
            get => _curr_player_pos;
            set
            {
                _curr_player_pos = value;
                OnPropertyChanged(nameof(Current_Player_Pos));
                if (_players.Length <= _curr_player_pos)
                {
                    _curr_player_pos = 0;
                    RunDealerLogic();
                }
            }
        }

        public Game(int players)
        {
            _players = new PlayerEntity[players];
            _curr_player_pos = 1;

            for (int i = 0; i < players; i++)
            {
                _players[i] = new PlayerEntity(i, 1000);
            }
        }

        public void Init_Decks(int decks)
        {
            _decks = new Deck[decks];
            Multi_Deck_Curr_Pos = 0;
            for (int i = 0; i < decks; i++)
            {
                _decks[i] = new Deck();
            }

            Card[] multi_deck = _decks[0].Cards;
            if (decks > 1)
            {
                for (int i = 1; i < decks; i++)
                {
                    multi_deck = multi_deck.Concat(_decks[i].Cards).ToArray();
                }
            }
            ShufflDecks(multi_deck);
        }
        
        private Card[] ShufflDecks(Card[] multi_deck)
        {
            Card[] res = Deck.Shuffle_Deck(multi_deck);

            for (int i = 0; i < _decks.Length; i++)
            {
                Array.Copy(res, i * Deck.MaxValue(), _decks[i].Cards, 0, Deck.MaxValue());
                _decks[i].Curr_pos = 0;
            }
            
            Multi_Deck_Curr_Pos = 0;

            return res;
        }
        private void ShufflDecks()
        {
            Card[] res = _decks[0].Cards;

            for (int i = 1; i < _decks.Length; i++)
            {
                res = res.Concat(_decks[i].Cards).ToArray();
            }

            res = Deck.Shuffle_Deck(res);

            for (int i = 0; i < _decks.Length; i++)
            {
                Array.Copy(res, i * Deck.MaxValue(), _decks[i].Cards, 0, Deck.MaxValue());
                _decks[i].Curr_pos = 0;
            }

            Multi_Deck_Curr_Pos = 0;
        }

        public Card Draw_Card()
        {
            if (_decks[Multi_Deck_Curr_Pos].Curr_pos == 52)
                Multi_Deck_Curr_Pos += 1;
            Card c = _decks[Multi_Deck_Curr_Pos].Next_card();
            return c;
        }

        public void Deal_Cards()
        {
            if (Multi_Deck_Cards_Remaining <= 15)
            {
                ShufflDecks();
                OnActionLog("Shuffling deck due to low cards...");
            }
            for (int i = 1; i <= _players.Length; i++)
            {
                Players[i % _players.Length].Cards_On_Hand.Clear();
                for (int r = 0; r < 2; r++)
                {
                    Card drawn = Draw_Card();
                    if (i % _players.Length == 0 && r == 1)
                    {
                        drawn.Is_Face_Up = false;
                    }
                    else
                    {
                        drawn.Is_Face_Up = true;
                    }
                    _players[i % _players.Length].AddCardToHand(drawn);
                }
            }
            Current_Player_Pos = 1;
            OnActionLog("Dealing cards...");
            if (Players.Any(x => x.Hand_Value == 21))
            {
                Calc_Winners();
            }
            OnPropertyChanged(nameof(Players));
            OnPropertyChanged(nameof(Multi_Deck_Cards_Remaining));
            OnPropertyChanged(nameof(Multi_Deck_Cards_Remaining_ToString));
            OnPropertyChanged(nameof(Multi_Deck_Curr_Pos));
        }

        public void Player_Draws_Card()
        {
            Card drawn = Draw_Card();
            drawn.Is_Face_Up = true;
            _players[_curr_player_pos].AddCardToHand(drawn);
            OnActionLog((_curr_player_pos == 0 ? "Dealer " : "Player " + _curr_player_pos.ToString() + " ") + " draws a card.");
            if (_players[_curr_player_pos].Hand_Value >= 21)
            {
                End_Turn();
            }
            OnPropertyChanged(nameof(Players));
            OnPropertyChanged(nameof(Multi_Deck_Cards_Remaining));
            OnPropertyChanged(nameof(Multi_Deck_Cards_Remaining_ToString));
            OnPropertyChanged(nameof(Multi_Deck_Curr_Pos));
        }

        public int Multi_Deck_Cards_Remaining
        {
            get
            {
                return _decks.Sum(x => x.Card_Remaining);
            }
        }

        public string Multi_Deck_Cards_Remaining_ToString
        {
            get
            {
                return Multi_Deck_Cards_Remaining.ToString();
            }
        }

        public void End_Turn()
        {
            OnActionLog((_curr_player_pos == 0 ? "Dealer " : "Player " + _curr_player_pos.ToString() + " ") + " ends his/her turn.");
            if (Current_Player_Pos == 0)
            {
                Calc_Winners();
            }
            Current_Player_Pos += 1;
            if (Current_Player_Pos >= _players.Length)
            {
                Current_Player_Pos = 0;
            }
        }

        public List<PlayerEntity> Calc_Winners()
        {
            List<PlayerEntity> winners = new List<PlayerEntity>();
            PlayerEntity dealer = _players[0];
            dealer.Cards_On_Hand[1].Is_Face_Up = true;
            for (int i = 1; i < _players.Length; i++)
            {
                PlayerEntity curr_pl = _players[i];
                if (curr_pl.Hand_Value < 22 && (curr_pl.Hand_Value > dealer.Hand_Value || dealer.Hand_Value > 21) && !curr_pl.Equals(dealer))
                {
                    int c_score = 0;
                    winners.Add(curr_pl);
                    if (curr_pl.Hand_Value == 21 && curr_pl.Cards_On_Hand.Count == 2)
                        c_score += 15;
                    else
                        c_score += 10;
                    curr_pl.Score += c_score;
                    dealer.Score -= c_score;
                }
            }
            if (_players.Any(x => ((x.Hand_Value > 21 || dealer.Hand_Value > x.Hand_Value) && dealer.Hand_Value < 22 && !x.Equals(dealer)) /*|| dealer.Hand_Value == 21*/))
            {
                int d_score = 0;
                winners.Add(dealer);
                if (dealer.Hand_Value == 21 && dealer.Cards_On_Hand.Count == 2)
                    d_score += 15;
                else
                    d_score += 10;
                foreach (PlayerEntity pl in _players)
                {
                    if (!pl.Equals(dealer) && winners.Any(x => !x.Equals(pl)))
                        pl.Score -= d_score;
                }
            }
            LastGameWinners = winners;
            OnActionLog("Round ended...");
            OnPropertyChanged("GameState");
            return winners;
        }

        public void RunDealerLogic()
        {
            if (Current_Player_Pos == 0)
            {
                Players[Current_Player_Pos].Cards_On_Hand[1].Is_Face_Up = true;
                while (Players[Current_Player_Pos].Hand_Value <= 17)
                {
                    Player_Draws_Card();
                }
                if (Players[Current_Player_Pos].Hand_Value < 21)
                {
                    End_Turn();
                }
            }
        }

        public void CheckGameState()
        {
            if (_players.Any(x => x.Hand_Value == 21))
                Calc_Winners();
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

        public event Notify LogHandler;

        protected virtual void OnActionLog(string e)
        {
            LogHandler?.Invoke(e);
        }
    }
}