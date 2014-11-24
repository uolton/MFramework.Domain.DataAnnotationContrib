#region Imports

using System;
using System.Text.RegularExpressions;
using MFramework.Domain.DataAnnotationsContrib.CheckDigits;
using MFramework.Domain.DataAnnotationsContrib.Internal;

#endregion

namespace MFramework.Domain.DataAnnotationsContrib.Finance
{
    /// <summary>
    /// Validates a 'payment' (debit/credit) card number.
    /// </summary>
    /// <remarks>
    /// See http://en.wikipedia.org/wiki/Credit_card_number#Credit_card_numbering
    /// </remarks>
    public class PaymentCardNumberAlgorithm : StringValidationAlgorithm
    {
        #region Constants and Enums
        public enum IndustryIdentifier
        {
            Airlines = 1,
            AirlinesAndIndustry = 2,
            BankingAndFinancial1 = 4,
            BankingAndFinancial2 = 5,
            ISOTC68 = 0,
            MerchandisingAndBanking = 6,
            National = 9,
            Petroleum = 7,
            TelecommunicationsAndIndustry = 8,
            TravelAndEntertainment = 3
        }
        [Flags]
        public enum IssuerIdentifier
        {
            Amex = 1,
            DinersClub = 2,
            Discover = 4,
            JCB = 8,
            Maestro = 16,
            MasterCard = 32,
            Solo = 2048,
            Switch = 64,
            Unknown = 128,
            VISA = 256,
            VISAElectron = 512,
            Bankcard = 1024,
            EnRoute = 4096,
            ChinaUnionPay = 8192,
            All = IssuerIdentifier.Amex | IssuerIdentifier.DinersClub | IssuerIdentifier.Discover | IssuerIdentifier.Discover
                | IssuerIdentifier.EnRoute | IssuerIdentifier.JCB | IssuerIdentifier.MasterCard | IssuerIdentifier.VISA
                | IssuerIdentifier.Bankcard | IssuerIdentifier.Maestro | IssuerIdentifier.Solo | IssuerIdentifier.Switch
                | IssuerIdentifier.VISAElectron | IssuerIdentifier.ChinaUnionPay
        }

        private const string REGEX_FORMAT = @"^\d{13,16}$";

        //This array corresponds with the first digit of the number.
        private static readonly IndustryIdentifier[] PrefixIndustryIdentifiers = new IndustryIdentifier[] {
            IndustryIdentifier.ISOTC68,
            IndustryIdentifier.Airlines,
            IndustryIdentifier.AirlinesAndIndustry,
            IndustryIdentifier.TravelAndEntertainment,
            IndustryIdentifier.BankingAndFinancial1,
            IndustryIdentifier.BankingAndFinancial2,
            IndustryIdentifier.MerchandisingAndBanking,
            IndustryIdentifier.Petroleum,
            IndustryIdentifier.TelecommunicationsAndIndustry,
            IndustryIdentifier.National,
        };
        #endregion

        #region Inner Classes and Structures
        #endregion

        #region Delegates and Events
        #endregion

        #region Instance and Shared Fields
        private IssuerIdentifier _acceptedCardIssuers;
        #endregion

        #region Constructors
        public PaymentCardNumberAlgorithm()
            : this(IssuerIdentifier.All)
        { }
        public PaymentCardNumberAlgorithm(IssuerIdentifier acceptedCardIssuers)
            : base(13, 16, new Regex(REGEX_FORMAT), new LuhnMod10CheckDigitAlgorithm(Enums.CheckDigitPosition.End))
        {
            //Store accepted types.
            _acceptedCardIssuers = acceptedCardIssuers;
        }
        #endregion

        #region Properties
        #endregion

        #region Private and Protected Methods
        #endregion

        #region Public Methods
        /// <summary>
        /// Determines the industry type based on the number passed.
        /// </summary>
        public static IndustryIdentifier ExtractIndustry(string cardNumber)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(cardNumber, "cardNumber");
            ArgumentValidation.CheckStringForRegexMatch(cardNumber, REGEX_FORMAT, "cardNumber");

            try
            {
                //Remove all non alphanumeric characters.
                string strippedNumber = TextHelper.StripNotNumeric(cardNumber);

                //Get prefix index.
                string prefixCode = strippedNumber.Substring(0, 1);
                int prefixIndex = Int32.Parse(prefixCode);

                //Get type for prefix.
                return PrefixIndustryIdentifiers[prefixIndex];
            }
            catch (Exception)
            {
                //Couldn't parse the required digits.
                throw new FormatException(String.Format(SR.Validator_ExtractFailure, "Industry"));
            }
        }

        /// <summary>
        /// Determines the issuer type based on the number passed.
        /// </summary>
        /// <remarks>
        /// FUTURE: See http://en.wikipedia.org/wiki/List_of_Bank_Identification_Numbers
        /// for a way to add actual issuing banks.
        /// </remarks>
        public static IssuerIdentifier ExtractIssuer(string cardNumber)
        {
            //Check params.
            ArgumentValidation.CheckStringForEmpty(cardNumber, "cardNumber");
            ArgumentValidation.CheckStringForRegexMatch(cardNumber, REGEX_FORMAT, "cardNumber");

            //Remove all non numeric characters.
            string strippedNumber = TextHelper.StripNotNumeric(cardNumber);

            switch (strippedNumber.Length)
            {
                case 13:
                    //VISA -- 4 -- 13 and 16 length.
                    //if (Regex.IsMatch(cardNumber, "^(4)")) return CardIssuerType.VISA;
                    if (strippedNumber.StartsWith("4", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.VISA;
                    //Not recognized.
                    throw new FormatException(SR.PaymentCardValidator_CardTypeNotRecognized);

                case 14:
                    //Diners Club -- 300-305, 36 or 38 -- 14 length.
                    //if (Regex.IsMatch(cardNumber, "^(300|301|302|303|304|305|36|38)")) return CardIssuerType.DinersClub;
                    if (strippedNumber.StartsWith("300", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("301", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("302", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("303", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("304", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("305", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("36", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;
                    if (strippedNumber.StartsWith("38", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.DinersClub;

                    //Not recognized.
                    throw new FormatException(SR.PaymentCardValidator_CardTypeNotRecognized);

                case 15:
                    //Bankcard -- 5610, 560221-560225 -- 15 length
                    //if (Regex.IsMatch(cardNumber, "^(5610|560221|560222|560223|560224|560225)")) return CardIssuerType.BankCard;
                    if (strippedNumber.StartsWith("560225", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Bankcard;
                    if (strippedNumber.StartsWith("560224", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Bankcard;
                    if (strippedNumber.StartsWith("560223", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Bankcard;
                    if (strippedNumber.StartsWith("560222", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Bankcard;
                    if (strippedNumber.StartsWith("560221", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Bankcard;
                    if (strippedNumber.StartsWith("5610", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Bankcard;

                    //AMEX -- 34 or 37 -- 15 length.
                    //if (Regex.IsMatch(cardNumber, "^(34|37)")) return CardIssuerType.Amex;
                    if (strippedNumber.StartsWith("34", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Amex;
                    if (strippedNumber.StartsWith("37", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Amex;

                    //enRoute -- 2014,2149 -- 15 length.
                    //if (Regex.IsMatch(cardNumber, "^(2014|2149)")) return CardIssuerType.enRoute;
                    if (strippedNumber.StartsWith("2014", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.EnRoute;
                    if (strippedNumber.StartsWith("2149", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.EnRoute;

                    //JCB -- 2131, 1800 -- 15 length
                    //if (Regex.IsMatch(cardNumber, "^(2131|1800)")) return CardIssuerType.JCB;
                    if (strippedNumber.StartsWith("2131", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.JCB;
                    if (strippedNumber.StartsWith("1800", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.JCB;

                    //Not recognized.
                    throw new FormatException(SR.PaymentCardValidator_CardTypeNotRecognized);

                case 16:

                    if (strippedNumber.StartsWith("417500", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.VISAElectron;
                    if (strippedNumber.StartsWith("633110", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("564182", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4903", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4905", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4911", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4936", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("6333", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("6759", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("6767", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Solo;
                    if (strippedNumber.StartsWith("63", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Solo;

                    //Discover -- 6011 -- 16 length
                    //if (Regex.IsMatch(cardNumber, "^(6011)")) return CardIssuerType.Discover;
                    if (strippedNumber.StartsWith("6011", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Discover;

                    if (strippedNumber.StartsWith("5020", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Maestro;

                    //JCB -- 3 -- 16 length
                    //if (Regex.IsMatch(cardNumber, "^(3)")) return CardIssuerType.JCB;
                    if (strippedNumber.StartsWith("35", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.JCB;

                    //China Union Pay -- (622126-622925) -- 16-19 length
                    if (strippedNumber.StartsWith("622", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.ChinaUnionPay;

                    //MasterCard -- 51 through 55 -- 16 length.
                    //if (Regex.IsMatch(cardNumber, "^(51|52|53|54|55)")) return CardIssuerType.MasterCard;
                    if (strippedNumber.StartsWith("51", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.MasterCard;
                    if (strippedNumber.StartsWith("52", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.MasterCard;
                    if (strippedNumber.StartsWith("53", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.MasterCard;
                    if (strippedNumber.StartsWith("54", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.MasterCard;
                    if (strippedNumber.StartsWith("55", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.MasterCard;
                    if (strippedNumber.StartsWith("6", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Maestro;
                    if (strippedNumber.StartsWith("4", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.VISA;

                    //Not recognized.
                    throw new FormatException(SR.PaymentCardValidator_CardTypeNotRecognized);

                case 18:
                case 19:
                    //China Union Pay -- (622126-622925) -- 16-19 length
                    if (strippedNumber.StartsWith("622", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.ChinaUnionPay;

                    if (strippedNumber.StartsWith("564182", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("633110", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("6333", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("6759", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4903", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4905", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4911", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("4936", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Switch;
                    if (strippedNumber.StartsWith("6767", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Solo;
                    if (strippedNumber.StartsWith("63", StringComparison.OrdinalIgnoreCase))
                        return IssuerIdentifier.Solo;

                    //Not recognized.
                    throw new FormatException(SR.PaymentCardValidator_CardTypeNotRecognized);
                default:
                    //Not recognized.
                    throw new FormatException(SR.PaymentCardValidator_CardTypeNotRecognized);
            }
        }
        #endregion

        #region Event Handlers
        #endregion

        #region Base Class Overrides
        protected override string Canononicalize(string rawValue)
        {
            return TextHelper.StripNotNumeric(rawValue);
        }

        protected override bool ValidateCustom(string rawValue, out string validationMessage)
        {
            //Remove all non numeric characters.
            string strippedNumber = Canononicalize(rawValue);

            //Try to ascertain card type.
            IssuerIdentifier cardType;
            try
            {
                cardType = ExtractIssuer(strippedNumber);
            }
            catch (FormatException ex)
            {
                //Unrecognized number.
                validationMessage = ex.Message;
                return false;
            }

            //Card specific rules (usually just length).
            int valueLength = strippedNumber.Length;
            bool lengthValid = true;
            switch (cardType)
            {
                case IssuerIdentifier.DinersClub:
                    lengthValid = (valueLength == 14);

                    break;
                case IssuerIdentifier.Amex:
                case IssuerIdentifier.EnRoute:
                case IssuerIdentifier.Bankcard:
                    lengthValid = (valueLength == 15);

                    break;
                case IssuerIdentifier.MasterCard:
                case IssuerIdentifier.Discover:
                    lengthValid = (valueLength == 16);

                    break;
                case IssuerIdentifier.VISA:
                    lengthValid = (valueLength == 13 || valueLength == 16);

                    break;
                case IssuerIdentifier.JCB:
                    if (strippedNumber.StartsWith("3", StringComparison.OrdinalIgnoreCase)) lengthValid = (valueLength == 16);
                    if (strippedNumber.StartsWith("2131", StringComparison.OrdinalIgnoreCase)) lengthValid = (valueLength == 15);
                    if (strippedNumber.StartsWith("1800", StringComparison.OrdinalIgnoreCase)) lengthValid = (valueLength == 15);

                    break;
            }
            if (!lengthValid)
            {
                validationMessage = SR.Validator_LengthFailure;
                return false;
            }

            //Finally (now we know it is valid) check card type is accepted.
            if ((cardType & _acceptedCardIssuers) != cardType)
            {
                //Unaccepted issuer.
                validationMessage = SR.PaymentCardValidator_CardTypeNotAccepted;
                return false;
            }

            validationMessage = "";
            return true;
        }
        #endregion
    }
}