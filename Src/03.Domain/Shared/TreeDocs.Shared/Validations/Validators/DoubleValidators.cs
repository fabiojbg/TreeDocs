namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        #region IsGreaterThan

        public Validator IsGreaterThan(decimal val, double comparer, string property, string message)
        {
            if ((double)val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(double val, double comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(float val, double comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(long val, double comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterThan(int val, double comparer, string property, string message)
        {
            if (val <= comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsGreaterThan

        #region IsGreaterOrEqualsThan

        public Validator IsGreaterOrEqualsThan(decimal val, double comparer, string property, string message)
        {
            if ((double)val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(double val, double comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(float val, double comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(long val, double comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsGreaterOrEqualsThan(int val, double comparer, string property, string message)
        {
            if (val < comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsGreaterOrEqualsThan

        #region IsLowerThan

        public Validator IsLowerThan(decimal val, double comparer, string property, string message)
        {
            if ((double)val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(double val, double comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(float val, double comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(long val, double comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerThan(int val, double comparer, string property, string message)
        {
            if (val >= comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsLowerThan

        #region IsLowerOrEqualsThan

        public Validator IsLowerOrEqualsThan(decimal val, double comparer, string property, string message)
        {
            if ((double)val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(double val, double comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(float val, double comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(long val, double comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator IsLowerOrEqualsThan(int val, double comparer, string property, string message)
        {
            if (val > comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion IsLowerOrEqualsThan

        #region AreEquals

        public Validator AreEquals(decimal val, double comparer, string property, string message)
        {
            if ((double)val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(double val, double comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(float val, double comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(long val, double comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(int val, double comparer, string property, string message)
        {
            if (val != comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion AreEquals

        #region AreNotEquals

        public Validator AreNotEquals(decimal val, double comparer, string property, string message)
        {
            if ((double)val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(double val, double comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(float val, double comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(long val, double comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(int val, double comparer, string property, string message)
        {
            if (val == comparer)
                AddNotification(property, message);

            return this;
        }

        #endregion AreNotEquals

        #region Between

        public Validator IsBetween(double val, double from, double to, string property, string message)
        {
            if (!(val >= from && val <= to))
                AddNotification(property, message);

            return this;
        }

        #endregion Between

        public Validator IsNullOrNullable(double? val, string property, string message)
        {
            if (val == null || !val.HasValue)
                AddNotification(property, message);

            return this;
        }
    }
}