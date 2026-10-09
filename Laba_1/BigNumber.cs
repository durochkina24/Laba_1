using System;
using System.Text;

namespace Laba_1
{
    public class BigNumber
    {
        private const int Base = 1000;
        private int[] number; // младший разряд в конце

        public BigNumber(string value)
        {
            value = value.TrimStart('0');
            if (string.IsNullOrEmpty(value)) value = "0";

            int blockCount = (value.Length + 2) / 3;
            number = new int[blockCount];

            int strIndex = value.Length;
            int arrIndex = blockCount - 1;

            while (strIndex > 0)
            {
                int take = Math.Min(3, strIndex);
                string block = value.Substring(strIndex - take, take);
                number[arrIndex] = int.Parse(block);
                strIndex -= take;
                arrIndex--;
            }
        }

        private BigNumber(int[] blocks)
        {
            number = TrimLeadingZeros(blocks);
        }

        public static BigNumber Zero => new BigNumber("0");

        public BigNumber Clone() => new BigNumber((int[])number.Clone());

        private static int[] TrimLeadingZeros(int[] arr)
        {
            int start = 0;
            while (start < arr.Length - 1 && arr[start] == 0) start++;
            int[] result = new int[arr.Length - start];
            Array.Copy(arr, start, result, 0, result.Length);
            return result;
        }

        private BigNumber Add(BigNumber other)
        {
            int maxLen = Math.Max(number.Length, other.number.Length);
            int[] result = new int[maxLen + 1];
            int carry = 0;

            for (int i = 0; i < maxLen; i++)
            {
                int a = i < number.Length ? number[number.Length - 1 - i] : 0;
                int b = i < other.number.Length ? other.number[other.number.Length - 1 - i] : 0;
                int sum = a + b + carry;
                result[result.Length - 1 - i] = sum % Base;
                carry = sum / Base;
            }
            result[0] = carry;
            return new BigNumber(result);
        }

        private BigNumber Subtract(BigNumber other)
        {
            int[] result = new int[number.Length];
            int borrow = 0;

            for (int i = 0; i < number.Length; i++)
            {
                int a = number[number.Length - 1 - i];
                int b = i < other.number.Length ? other.number[other.number.Length - 1 - i] : 0;
                int diff = a - b - borrow;
                if (diff < 0) { diff += Base; borrow = 1; }
                else borrow = 0;
                result[result.Length - 1 - i] = diff;
            }
            return new BigNumber(result);
        }

        // Сколько блоков займёт число multiplier в системе с основанием Base
        private static int CountBlocks(long value)
        {
            if (value == 0) return 1;
            int count = 0;
            while (value > 0)
            {
                count++;
                value /= Base;
            }
            return count;
        }

        private BigNumber MultiplyLong(long multiplier)
        {
            if (multiplier == 0) return BigNumber.Zero;

            // Заранее считаем, сколько блоков понадобится:
            // длина числа + длина множителя + 1 запас под перенос
            int extraBlocks = CountBlocks(multiplier);
            int[] result = new int[number.Length + extraBlocks + 1];
            long carry = 0;

            for (int i = 0; i < number.Length; i++)
            {
                long current = (long)number[number.Length - 1 - i] * multiplier + carry;
                result[result.Length - 1 - i] = (int)(current % Base);
                carry = current / Base;
            }

            int idx = number.Length;
            while (carry > 0)
            {
                if (idx >= result.Length) break; // страховка от выхода за границу
                result[result.Length - 1 - idx] = (int)(carry % Base);
                carry /= Base;
                idx++;
            }
            return new BigNumber(result);
        }

        private BigNumber Multiply(double multiplier)
        {
            long scaled = (long)Math.Round(multiplier * 1000);
            if (scaled == 0) return BigNumber.Zero;
            return MultiplyLong(scaled).DivideLong(1000);
        }

        private BigNumber DivideLong(long divisor)
        {
            if (divisor == 0) throw new DivideByZeroException();
            int[] result = new int[number.Length];
            long remainder = 0;

            for (int i = 0; i < number.Length; i++)
            {
                long current = remainder * Base + number[i];
                result[i] = (int)(current / divisor);
                remainder = current % divisor;
            }
            return new BigNumber(result);
        }

        private BigNumber Divide(double divisor)
        {
            long scaled = (long)Math.Round(divisor * 1000);
            if (scaled == 0) throw new DivideByZeroException();
            return MultiplyLong(1000).DivideLong(scaled);
        }

        private int CompareTo(BigNumber other)
        {
            if (number.Length != other.number.Length)
                return number.Length.CompareTo(other.number.Length);
            for (int i = 0; i < number.Length; i++)
                if (number[i] != other.number[i])
                    return number[i].CompareTo(other.number[i]);
            return 0;
        }

        public bool IsZero()
        {
            foreach (int b in number) if (b != 0) return false;
            return true;
        }

        public static BigNumber operator +(BigNumber a, BigNumber b) => a.Add(b);
        public static BigNumber operator -(BigNumber a, BigNumber b) => a.Subtract(b);
        public static BigNumber operator *(BigNumber a, double b) => a.Multiply(b);
        public static BigNumber operator /(BigNumber a, double b) => a.Divide(b);
        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
        public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;
        public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(number[0]);
            for (int i = 1; i < number.Length; i++)
                sb.Append(number[i].ToString("D3"));
            return sb.ToString();
        }
    }
}