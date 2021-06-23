using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using blackjack_obj;
using blackjack_simple_obj;

namespace blackjack
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Game _game;
        public MainWindow()
        {
            InitializeComponent();
            _game = new Game(2);
            // Console.WriteLine(g.Decks.ToString());
            _game.Init_Decks(1);

            //TextBlock tblk_dealer = new TextBlock();
            //Binding dealer_cards = new Binding(_game.Players[0].CardsOnHandToString);
            //tblk_dealer.SetBinding(TextBlock.TextProperty, dealer_cards);
            //wrap_dealer.Children.Add(tblk_dealer);

            //TextBlock tblk_player = new TextBlock();
            //Binding player_cards = new Binding();
            //player_cards.Source = _game.Players[1].CardsOnHandToString;
            //player_cards.Path = new PropertyPath(_game.Players[1].CardsOnHandToString);
            //tblk_player.SetBinding(TextBlock.TextProperty, player_cards);
            //wrap_dealer.Children.Add(tblk_player);

            //Binding player_score = new Binding(_game.Players[1].Score.ToString());
            //tblk_score.SetBinding(TextBlock.TextProperty, player_score);

            //Binding card_remaining = new Binding(_game.Multi_Deck_Cards_Remaining.ToString());
            //tblk_card_remaining.SetBinding(TextBlock.TextProperty, card_remaining);
            /*tblk_text.Text = _game.Decks.First().ToString();*/
            /*Debug.WriteLine();*/
            _game.PropertyChanged += _game_PropertyChanged;
            _game.LogHandler += _game_LogHandler;
            _game.Deal_Cards();
            UpdateTexts();
        }

        private void _game_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "GameState")
            {
                Debug.WriteLine("Game state changed");
                if (_game.LastGameWinners.Any(x => x.Seat_Pos == 1))
                {
                    tblk_status.Text = "Win";
                }
                else
                {
                    tblk_status.Text = _game.LastGameWinners.Count == 0 ? "Draw" : "Lose";
                }
                tblk_dealer_hv.Text = _game.Players[0].Hand_Value.ToString();
                tblk_player_hv.Text = _game.Players[1].Hand_Value.ToString();
                UpdateStatusText();
                grid_StatusBody.Visibility = Visibility.Visible;
            }
            if (e.PropertyName == "Current_Player_Pos")
            {
                if (_game.Current_Player_Pos == 1)
                {
                    btn_Pass.IsEnabled = true;
                    btn_Hit.IsEnabled = true;
                    bord_player.BorderBrush = Brushes.Yellow;
                    bord_dealer.BorderBrush = Brushes.Gray;
                }
                else
                {
                    btn_Pass.IsEnabled = false;
                    btn_Hit.IsEnabled = false;
                    bord_player.BorderBrush = Brushes.Gray;
                    bord_dealer.BorderBrush = Brushes.Yellow;
                }
            }
        }

        private void btn_Hit_Click(object sender, RoutedEventArgs e)
        {
            _game.Player_Draws_Card();
            UpdateTexts();
        }

        private void btn_Pass_Click(object sender, RoutedEventArgs e)
        {
            _game.End_Turn();
            UpdateTexts();
        }

        private void UpdateTexts()
        {
            //tblk_dealer.Text = _game.Players[0].CardsOnHandToString;
            //tblk_player.Text = _game.Players[1].CardsOnHandToString;
            sp_dealer_cards.Children.Clear();
            sp_player_cards.Children.Clear();

            foreach(Card c in _game.Players[0].Cards_On_Hand)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("/Images/" + c.ToString() + ".png", UriKind.Relative));
                sp_dealer_cards.Children.Add(img);
            }

            foreach (Card c in _game.Players[1].Cards_On_Hand)
            {
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("/Images/" + c.ToString() + ".png", UriKind.Relative));
                sp_player_cards.Children.Add(img);
            }

            tblk_score.Text = _game.Players[1].Score.ToString();
            tblk_card_remaining.Text = _game.Multi_Deck_Cards_Remaining_ToString;
            tblk_player_hand_value.Text = _game.Players[1].Hand_Value.ToString();
            sp_dealer_cards.UpdateLayout();
            sp_player_cards.UpdateLayout();
            tblk_score.UpdateLayout();
            tblk_card_remaining.UpdateLayout();
            tblk_player_hand_value.UpdateLayout();
        }

        private void UpdateStatusText()
        {
            grid_StatusBody.UpdateLayout();
            tblk_status.UpdateLayout();
            tblk_player_hv.UpdateLayout();
            tblk_dealer_hv.UpdateLayout();
        }

        private void btn_Next_Click(object sender, RoutedEventArgs e)
        {
            grid_StatusBody.Visibility = Visibility.Collapsed;
            _game.Deal_Cards();
            UpdateTexts();
        }

        private void _game_LogHandler(string e)
        {
            tblk_Log.Text += e + "\n";
            tblk_Log.UpdateLayout();
            sv_Log.ScrollToVerticalOffset(sv_Log.ExtentHeight); 
        }
    }
}