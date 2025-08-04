using System;
using System.Linq;
using System.Text.RegularExpressions;	

namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        public Validator IsNotNullOrEmpty(string val, string property, string message)
        {
            if (string.IsNullOrEmpty(val))
                AddNotification(property, message);

            return this;
        }

        public Validator IsNotNullOrWhiteSpace(string val, string property, string message)
        {
            if (string.IsNullOrWhiteSpace(val))
                AddNotification(property, message);

            return this;
        }


        public Validator IsNullOrEmpty(string val, string property, string message)
        {
            if (!string.IsNullOrEmpty(val))
                AddNotification(property, message);

            return this;
        }

        public Validator HasMinLen(string val, int min, string property, string message)
        {
            if (string.IsNullOrEmpty(val) || val.Length < min)
                AddNotification(property, message);

            return this;
        }

        public Validator HasMaxLen(string val, int max, string property, string message)
        {
            if (!string.IsNullOrEmpty(val) && val.Length > max)
                AddNotification(property, message);

            return this;
        }

        public Validator HasLen(string val, int len, string property, string message)
        {
            if (string.IsNullOrEmpty(val) || val.Length != len)
                AddNotification(property, message);

            return this;
        }

        public Validator Contains(string val, string text, string property, string message)
        {
            if (!val.Contains(text))
                AddNotification(property, message);

            return this;
        }

        public Validator ContainsIgnoringCase(string val, string text, string property, string message)
        {
            if (!val.Contains(text, StringComparison.InvariantCultureIgnoreCase))
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(string val, string text, string property, string message, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (!val.Equals(text, comparisonType))
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(string val, string text, string property, string message, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            if (val.Equals(text, comparisonType))
                AddNotification(property, message);

            return this;
        }

        public Validator IsEmail(string email, string property, string message)
        {
            const string pattern = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            return Matchs(email, pattern, property, message);
        }

        public Validator IsEmailOrEmpty(string email, string property, string message)
        {
            if (string.IsNullOrEmpty(email))
                return this;

            return IsEmail(email, property, message);
        }

        public Validator IsUrl(string url, string property, string message)
        {
            const string pattern = @"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$";
            return Matchs(url, pattern, property, message);
        }

        public Validator IsUrlOrEmpty(string url, string property, string message)
        {
            if (string.IsNullOrEmpty(url))
                return this;

            return IsUrl(url, property, message);
        }

        public Validator Matchs(string text, string pattern, string property, string message)
        {
            if (!Regex.IsMatch(text ?? "", pattern))
                AddNotification(property, message);

            return this;
        }

        public Validator IsDigit(string text, string property, string message)
        {
            const string pattern = @"^\d+$";
            return Matchs(text, pattern, property, message);
        }

        public Validator HasMinLengthIfNotNullOrEmpty(string text, int min, string property, string message)
        {
            if (!string.IsNullOrEmpty(text) && text.Length < min)
                AddNotification(property, message);

            return this;
        }

        public Validator HasMaxLengthIfNotNullOrEmpty(string text, int max, string property, string message)
        {
            if (!string.IsNullOrEmpty(text) && text.Length > max)
                AddNotification(property, message);

            return this;
        }

        public Validator HasExactLengthIfNotNullOrEmpty(string text, int len, string property, string message)
        {
            if (!string.IsNullOrEmpty(text) && text.Length != len)
                AddNotification(property, message);

            return this;
        }
    }
}
