using System;
using System.Linq.Expressions;


namespace Domain.Shared.Validations
{
    //baseado no projeto https://github.com/andrebaltieri/flunt/wiki
    // sendo que alterei os seguintes nomes
    //class Contract = class Validator

    public partial class Validator : Notifiable
    {
        public Validator Requires()
        {
            return this;
        }

        public Validator Join(params Notifiable[] items)
        {
            if (items != null)
            {
                foreach (var notifiable in items)
                {
                    if (notifiable.Invalid)
                        AddNotifications(notifiable.Notifications);
                }
            }

            return this;
        }
        
        public Validator IfNotNull(object parameterType, Expression<Func<Validator, Validator>> contractExpression)
        {
            if (parameterType != null)
            {
                contractExpression.Compile().Invoke(this);
            }
            
            return this;
        }
    }
}
