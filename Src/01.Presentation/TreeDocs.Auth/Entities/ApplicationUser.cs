using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TreeDocs.Auth.Entities
{
	[CollectionName("Users")]
	public class ApplicationUser : MongoIdentityUser<Guid>
	{
		public string FullName { get; set; }
		public ApplicationUser() : base()
		{
		}

		public ApplicationUser(string userName, string email) : base(userName, email)
		{
		}
	}

	public class ApplicationRole : MongoIdentityRole<Guid>
	{
		public ApplicationRole() : base()
		{
		}

		public ApplicationRole(string roleName) : base(roleName)
		{
		}
	}
}
