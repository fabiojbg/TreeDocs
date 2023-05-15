namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        public Validator IsFalse(bool val, string property, string message)
        {
            if (val)
                AddNotification(property, message);

            return this;
        }

        public Validator IsTrue(bool val, string property, string message) =>
            IsFalse(!val, property, message);
    }
}