using Auth.Domain.Entities;
using Auth.Domain.MongoDb.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Auth.Domain;

namespace Auth.Domain.MongoDb.DbModels
{
    public class DbUser : DbEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string HashedPassword { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc, DateOnly = false)]
        public DateTime? LastLogin { get; set; }
        public string[] Roles { get; set; }

        public DbUser(){ }

        public DbUser(User user)
        {
            Id = user.Id.ToObjectId();
            Name = user.Name;
            Email = user.Email?.Address;
            HashedPassword = user.HashedPassword;
            Roles = user.Roles?.ToArray();
            LastLogin = user.LastLogin;
        }

    }
}
