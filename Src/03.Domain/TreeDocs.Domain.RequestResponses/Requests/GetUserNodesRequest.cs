﻿using Domain.Shared;
using Domain.Shared.Validations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using TreeDocs.Domain.RequestResponses;

namespace TreeDocs.Domain.RequestsResponses
{
    public class GetUserNodesRequest : ValidatableRequest, IRequest<RequestResult<GetUserNodesResponse>>
    {
        public string UserId { get; set; }

        public override bool Validate()
        {
            AddNotifications(new Validator().IsNotNullOrWhiteSpace(UserId, "UserId", DomainResources.ErrUserNodeIdRequired));

            return Valid;
        }
    }
}
