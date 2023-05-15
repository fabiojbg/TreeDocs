using Auth.Domain.RequestsResponses;
using Domain.Shared;
using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Auth.Domain.Tests
{
    public class Requests_Tests
    {
        [Theory]
        [InlineData("admin@admin.com", "Ad1ministr@t0r")]
        public void AuthenticationRequest_Valid(string userEmail, string password)
        {
            var request = new AuthenticateUserRequest()
            {
                UserEmail = userEmail,
                Password = password
            };
            Assert.True(request.Validate());
            Assert.Empty(request.Notifications);
        }

        [Theory]
        [InlineData("admin!admin.com", "Ad1ministr@t0r", "UserEmail")]
        [InlineData("admin@admin.com", null, "Password")]
        [InlineData("admin@admin.com", "TooBigPasswordForThisTest", "Password")]
        [InlineData("admin!admin.com", "TooBigPasswordForThisTest", "UserEmail, Password")]
        public void AuthenticationRequest_Invalid(string userEmail, string password, string invalidFields)
        {
            var request = new AuthenticateUserRequest()
            {
                UserEmail = userEmail,
                Password = password
            };
            var isValid = request.Validate();
            var expectedFailedFieldsExists = invalidFields.Split(',').All(x => request.Notifications.Any(y => y.Property == x.Trim()));

            Assert.False(isValid);
            Assert.True(expectedFailedFieldsExists);
        }


        [Theory]
        [InlineData("Administrator", "admin@admin.com", "Ad1ministr@t0r", null)]
        public void CreateUserRequest_Valid(string name, string email, string password, string[] roles)
        {
            var request = new CreateUserRequest()
            {
                Name = name,
                Email = email,
                Password = password,
                Roles = roles
            };

            Assert.True(request.Validate());
            Assert.Empty(request.Notifications);
        }

        [Theory]
        [InlineData(null, "admin@admin.com", "Ad1ministr@t0r", null, "Name")]
        [InlineData(null, null, "Ad1ministr@t0r", null, "Name, Email")]
        [InlineData(null, null, null, null, "Name, Email, Password")]
        [InlineData(null, "admin.admin.com", "Ad1ministr@t0r", null, "Name, Email")]
        public void CreateUserRequest_Invalid(string name, string email, string password, string[] roles, string invalidFields)
        {
            var request = new CreateUserRequest()
            {
                Name = name,
                Email = email,
                Password = password,
                Roles = roles
            };
            var isValid = request.Validate();
            var expectedFailedFieldsExists = invalidFields.Split(',').All(x => request.Notifications.Any(y => y.Property == x.Trim()));

            Assert.False(isValid);
            Assert.True(expectedFailedFieldsExists);
        }

        [Theory]
        [InlineData("admin@admin.com")]
        public void GetUserRequest_Valid(string email)
        {
            var request = new GetUserRequest()
            {
                Email = email
            };

            Assert.True(request.Validate());
            Assert.Empty(request.Notifications);
        }

        [Theory]
        [InlineData("admin!admin.com", "Email")]
        [InlineData("admin@admin", "Email")]
        [InlineData("admin.admin@", "Email")]
        public void GetUserRequest_Invalid(string email, string invalidFields)
        {
            var request = new GetUserRequest()
            {
                Email = email
            };
            var isValid = request.Validate();
            var expectedFailedFieldsExists = invalidFields.Split(',').All(x => request.Notifications.Any(y => y.Property == x.Trim()));

            Assert.False(isValid);
            Assert.True(expectedFailedFieldsExists);
        }

        [Theory]
        [InlineData("12345", "admin@admin.com", "Administrator")]
        public void UpdateUserDataRequest_Valid(string userId, string email, string name)
        {
            var request = new UpdateUserDataRequest()
            {
                UserId = userId,
                Email = email,
                Name = name,
            };

            var isValid = request.Validate();

            Assert.True(isValid);
            Assert.Empty(request.Notifications);
        }

        [Theory]
        [InlineData(null, "admin@admin.com", "Administrator", "UserId")]
        [InlineData(null, null, "Administrator", "UserId, Email")]
        [InlineData(null, null, null, "UserId, Email, Name")]
        public void UpdateUserDataRequest_Invalid(string userId, string email, string name, string invalidFields)
        {
            var request = new UpdateUserDataRequest()
            {
                UserId = userId,
                Email = email,
                Name = name,
            };
            var isValid = request.Validate();
            var expectedFailedFieldsExists = invalidFields.Split(',').All(x => request.Notifications.Any(y => y.Property == x.Trim()));

            Assert.False(isValid);
            Assert.True(expectedFailedFieldsExists);
        }

        [Theory]
        [InlineData("12345", "admin@admin.com", "Teste", "Administrator")]
        [InlineData("12345", null, "Teste", "Administrator")]
        [InlineData( null, "admin@admin.com", "Teste", "Administrator")]
        public void UpdateUserPasswordRequest_Valid(string userId, string userEmail, string oldPassword, string newPassword)
        {
            var request = new UpdateUserPasswordRequest()
            {
                UserId = userId,
                UserEmail = userEmail,
                OldPassword = oldPassword,
                NewPassword = newPassword,
            };

            var isValid = request.Validate();

            Assert.True(isValid);
            Assert.Empty(request.Notifications);
        }

        [Theory]
        [InlineData(null, null, "Teste", "Administrator", "UserId/UserEmail")]
        [InlineData("12345", null, "Teste", "Teste", "NewPassword")]
        [InlineData(null, "teste@teste.com", "Teste", "Teste", "NewPassword")]
        [InlineData("12345", null, "Teste", null, "NewPassword")]
        [InlineData("12345", null, "Teste", "!23", "NewPassword")]
        [InlineData("12345", null, "Teste", "TooBigPasswordToFit", "NewPassword")]
        public void UpdateUserPasswordRequest_Invalid(string userId, string userEmail, string oldPassword, string newPassword, string invalidFields)
        {
            var request = new UpdateUserPasswordRequest()
            {
                UserId = userId,
                UserEmail = userEmail,
                OldPassword = oldPassword,
                NewPassword = newPassword
            };
            var isValid = request.Validate();
            var expectedFailedFieldsExists = invalidFields.Split(',').All(x => request.Notifications.Any(y => y.Property == x.Trim()));

            Assert.False(isValid);
            Assert.True(expectedFailedFieldsExists);
        }

    }
}
