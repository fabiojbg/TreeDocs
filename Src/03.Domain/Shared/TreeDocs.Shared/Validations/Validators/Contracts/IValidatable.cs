namespace Domain.Shared.Validations
{
    public interface IValidatable
    {
        bool Validate();
    }

    public interface IValidatable<TObj>
    {
        bool Validate(TObj instance);
    }

    public abstract class AbstractValidator<TObj> : Notifiable, IValidatable
    {
        protected TObj obj;

        public AbstractValidator(TObj objToValidate)
        {
            obj = objToValidate;
        }
        public abstract bool Validate();
    }

}