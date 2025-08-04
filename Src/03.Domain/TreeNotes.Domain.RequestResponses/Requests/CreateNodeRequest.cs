using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Domain.Shared;
using TreeNotes.Domain.Enums;

namespace TreeNotes.Domain.RequestsResponses
{
    public class CreateNodeRequest : Notifiable, IRequest<RequestResult<CreateNodeResponse>>
    {
        public string ParentId { get; set; }
        public string Name { get; set; }
        public NodeType NodeType { get; set; }
        public string Contents { get; set; }

    }

}
