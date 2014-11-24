#region Imports

using System;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.CheckDigits
{
    /// <summary>
    /// Performs a Luhn algorithm check on a number.
    /// 
    /// The Luhn algorithm or Luhn formula, also known as the "modulus 10" or "mod 10" algorithm, was developed in the 1960s as a method of validating identification numbers.
    /// It is a simple checksum formula used to validate a variety of account numbers, such as credit card numbers and Canadian Social Insurance Numbers.
    /// It was created in the 1960s by IBM scientist Hans Peter Luhn (1896–1964).
    /// The algorithm is in the public domain and is in wide use today.
    /// It is not intended to be a cryptographically secure hash function; it protects against accidental error, not malicious attack.
    /// Most credit cards and many government identification numbers use the algorithm as a simple method of distinguishing valid numbers from collections of random digits.
    ///
    /// The formula generates a check digit, which is usually appended to a partial account number to generate the full account number.
    /// This account number must pass the following algorithm (and the check digit chosen and placed so that the full account number will):
    ///
    /// Starting with the second to last digit and moving left, double the value of all the alternating digits. For any digits that thus become 10 or more, add their digits together.
    /// For example, 1111 becomes 2121, while 8763 becomes 7733 (from 2x8=16 -> 1+6=7 and 2x6=12 -> 1+2=3). 
    /// Add all these digits together. For example, 1111 becomes 2121, then 2+1+2+1 is 6; while 8763 becomes 7733, then 7+7+3+3 is 20. 
    /// If the total ends in 0 (put another way, if the total modulus 10 is congruent to 0), then the number is valid according to the Luhn formula, else it is not valid.
    /// So, 1111 is not valid (as shown above, it comes out to 6), while 8763 is valid (as shown above, it comes out to 20). 
    /// In the two examples above, if a check digit was to be added to the front of these numbers, then 4 might be added to 1111 to make 41111 (ie 4+ 2+1+2+1 =10), while 0 would be added to 8763 to make 08763 (0+ 7+7+3+3 = 20).
    /// It is usually the case that check digits are added to the end, although this requires a simple modification to the algorithm to determine an ending check digit given the rest of the account number.
    /// A weakness in this algorithm is that it can be bypassed by using all zero digits as the number.
    /// </summary>
    /// <remarks>
    /// Source: http://en.wikipedia.org/wiki/Luhn_formula
    /// </remarks>
    public class LuhnMod10CheckDigitAlgorithm : CheckDigitAlgorithm
    {
        #region Constants and Enums
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        #endregion

        #region Constructors
        public LuhnMod10CheckDigitAlgorithm(Enums.CheckDigitPosition checkDigitPosition)
            : base(checkDigitPosition, 1)
        { }
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        private int ComputeWeightedSumFromRightToLeft(string data)
        {
            int sum = 0;
            bool alt = true;
            for (int i = data.Length - 1; i >= 0; i--)
            {
                //Parse.
                if (!Char.IsNumber(data[i])) throw new FormatException();
                int digit = Int32.Parse(data.Substring(i, 1));

                if (alt)
                {
                    digit *= 2;
                    if (digit > 9) digit -= 9; //Collapse.
                }
                sum += digit;
                alt = !alt;
            }
            return sum;
        }
        #endregion

        #region Public Methods
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        protected override string ComputeCheckDigitOverride(string data)
        {
            int sum = ComputeWeightedSumFromRightToLeft(data);
            int mod = 10 - (sum % 10);
            string check = mod == 10 ? "0" : mod.ToString();

            return check;
        }
        #endregion
    }
}