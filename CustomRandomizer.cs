using System;
using System.Security.Cryptography;

namespace blackjack_simple_obj{

    public class CustomRandomizer : RandomNumberGenerator
    {
        private static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        
        public static int GetNextInt()
        {
            var data = new byte[sizeof(int)];
            rng.GetBytes(data);
            return BitConverter.ToInt32(data, 0) & (int.MaxValue - 1);
        }

        public static int GetNextInt(int maxValue)
        {
            return GetNextInt(0, maxValue);
        }

        public static int GetNextInt(int minValue, int maxValue)
        {
            if (minValue > maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }
            return (int)Math.Floor((minValue + ((double)maxValue - minValue) * GetNextDouble()));
        }

        public static double GetNextDouble()
        {
            var data = new byte[sizeof(uint)];
            rng.GetBytes(data);
            var randUint = BitConverter.ToUInt32(data, 0);
            return randUint / (uint.MaxValue + 1.0);
        }

        public override void GetBytes(byte[] data)
        { rng.GetBytes(data); }

        public override void GetNonZeroBytes(byte[] data)
        { rng.GetNonZeroBytes(data); }
    }
}