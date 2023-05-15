using Domain.Shared.Validations;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeDocs.Domain.Entities
{
    public abstract class Entity : Notifiable
    {
        public string Id { get; set; }

    }
}
