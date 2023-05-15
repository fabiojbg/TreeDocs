namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        #region IsGreaterThan

        public Validator IsGreaterThan(decimal val, long comparer, string property, string message)
        {
            if ((double)val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(double val, long comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(float val, long comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(int val, long comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(long val, long comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsGreaterThan

        #region IsGreaterOrEqualsThan

        public Validator IsGreaterOrEqualsThan(decimal val, long comparer, string property, string message)
        {
            if ((double)val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(double val, long comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(float val, long comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(int val, long comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(long val, long comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsGreaterOrEqualsThan

        #region IsLowerThan

        public Validator IsLowerThan(decimal val, long comparer, string property, string message)
        {
            if ((double)val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(double val, long comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(float val, long comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(int val, long comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(long val, long comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsLowerThan

        #region IsLowerOrEqualsThan

        public Validator IsLowerOrEqualsThan(decimal val, long comparer, string property, string message)
        {
            if ((double)val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(double val, long comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(float val, long comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(int val, long comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(long val, long comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsLowerOrEqualsThan

        #region AreEquals

        public Validator AreEquals(decimal val, long comparer, string property, string message)
        {
            if ((double)val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(double val, long comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(float val, long comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(int val, long comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(long val, long comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion AreEquals

        #region AreNotEquals

        public Validator AreNotEquals(decimal val, long comparer, string property, string message)
        {
            if ((double)val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(double val, long comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(float val, long comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(int val, long comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(long val, long comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion AreNotEquals

        #region Between

        public Validator IsBetween(long val, long from, long to, string property, string message)
        {
            if (!(val >= from && val <= to))
                AddNotification(property, message);

            return this;
        }

        #endregion Between
    }
}