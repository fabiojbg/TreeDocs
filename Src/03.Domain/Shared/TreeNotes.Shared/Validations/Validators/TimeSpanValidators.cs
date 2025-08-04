using System;

namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        public Validator IsGreaterThan(TimeSpan val, TimeSpan comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(TimeSpan val, TimeSpan comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(TimeSpan val, TimeSpan comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(TimeSpan val, TimeSpan comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsBetween(TimeSpan val, TimeSpan from, TimeSpan to, string property, string message)
        {
            if (!(val >= from && val <= to))
                AddNotification(property, message);

            return this;
        }
    }
}