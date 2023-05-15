namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        public Validator IsNull(object obj, string property, string message)
        {
            if (obj != null)
                AddNotification(property, message);

            return this;
        }

        public Validator IsNotNull(object obj, string property, string message)
        {
            if (obj == null)
                AddNotification(property, message);

            return this;
        }

        public Validator AreEquals(object obj, object comparer, string property, string message)
        {
            if (!obj.Equals(comparer))
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(object obj, object comparer, string property, string message)
        {
            if (obj.Equals(comparer))
                AddNotification(property, message);

            return this;
        }
    }
}