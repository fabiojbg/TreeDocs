namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        #region IsGreaterThan

        public Validator IsGreaterThan(decimal val, decimal comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(double val, decimal comparer, string property, string message)
        {
            if (val <= (double)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(float val, decimal comparer, string property, string message)
        {
            if (val <= (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(long val, decimal comparer, string property, string message)
        {
            if (val <= (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(int val, decimal comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsGreaterThan

        #region IsGreaterOrEqualsThan

        public Validator IsGreaterOrEqualsThan(decimal val, decimal comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(double val, decimal comparer, string property, string message)
        {
            if (val < (double)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(float val, decimal comparer, string property, string message)
        {
            if (val < (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(long val, decimal comparer, string property, string message)
        {
            if (val < (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(int val, decimal comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsGreaterOrEqualsThan

        #region IsLowerThan

        public Validator IsLowerThan(decimal val, decimal comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(double val, decimal comparer, string property, string message)
        {
            if (val >= (double)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(float val, decimal comparer, string property, string message)
        {
            if (val >= (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(long val, decimal comparer, string property, string message)
        {
            if (val >= (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(int val, decimal comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsLowerThan

        #region IsLowerOrEqualsThan

        public Validator IsLowerOrEqualsThan(decimal val, decimal comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(double val, decimal comparer, string property, string message)
        {
            if (val > (double)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(float val, decimal comparer, string property, string message)
        {
            if (val > (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(long val, decimal comparer, string property, string message)
        {
            if (val > (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(int val, decimal comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsLowerOrEqualsThan

        #region AreEquals

        public Validator AreEquals(decimal val, decimal comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(double val, decimal comparer, string property, string message)
        {
            if (val != (double)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(float val, decimal comparer, string property, string message)
        {
            if (val != (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(long val, decimal comparer, string property, string message)
        {
            if (val != (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(int val, decimal comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion AreEquals

        #region AreNotEquals

        public Validator AreNotEquals(decimal val, decimal comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(double val, decimal comparer, string property, string message)
        {
            if (val == (double)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(float val, decimal comparer, string property, string message)
        {
            if (val == (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(long val, decimal comparer, string property, string message)
        {
            if (val == (float)comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(int val, decimal comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion AreNotEquals

        #region Between

        public Validator IsBetween(decimal val, decimal from, decimal to, string property, string message)
        {
            if (!(val >= from && val <= to))
                AddNotification(property, message);

            return this;
        }

        #endregion Between

        public Validator IsNullOrNullable(decimal? val, string property, string message)
        {
            if (val == null || !val.HasValue)
                AddNotification(property, message);

            return this;
        }
    }
}