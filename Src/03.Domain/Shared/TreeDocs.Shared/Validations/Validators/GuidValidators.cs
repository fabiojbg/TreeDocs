using System;

namespace Domain.Shared.Validations
{
    public partial class Validator
    {
        public Validator AreEquals(Guid val, Guid comparer, string property, string message)
        {
            if( !val.Equals(comparer))
                AddNotification(property, message);

            return this;
        }

        public Validator AreNotEquals(Guid val, Guid comparer, string property, string message)
        {
            if (val.Equals(comparer))
                AddNotification(property, message);

            return this;
        }

        public Validator IsEmpty(Guid val, string property, string message)
        {
            if (val != Guid.Empty)
                AddNotification(property, message);

            return this;
        }

        public Validator IsNotEmpty(Guid val, string property, string message)
        {
            if (val == Guid.Empty)
                AddNotification(property, message);

            return this;
        }
    }
}