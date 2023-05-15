using System;

namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        public Validator IsGreaterThan(DateTime val, DateTime comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(DateTime val, DateTime comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(DateTime val, DateTime comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(DateTime val, DateTime comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsBetween(DateTime val, DateTime from, DateTime to, string property, string message)
        {
            if (!(val >= from && val <= to))
                AddNotification(property, message);

            return this;
        }

        public Validator IsNullOrNullable(DateTime? val, string property, string message)
        {
            if (val == null || !val.HasValue)
                AddNotification(property, message);

            return this;
        }
    }
}