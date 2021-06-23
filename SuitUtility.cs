namespace blackjack_simple_obj{
    public class SuitUtility
    {
        public static int MaxSuits()
        { return 4; }

        public static string SuitToString(int _suit)
        {
            switch(_suit)
            {
                case 1:
                    return "S";
                case 2:
                    return "H";
                case 3:
                    return "C";
                case 4:
                    return "D";
                default:
                    return "";
            }
        }


    }
}
