using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Models;
using FakeItEasy;
using Microsoft.AspNetCore.Identity;
using Siccar.Platform;
using Siccar.Platform.Tenants.Core;
using TenantRepository;
using Xunit;

namespace TenantUnitTests.Repositories
{
    public class UserMongoRepositoryTests
    {
        private readonly UserMongoRepository _sut;
        private readonly UserManager<ApplicationUser> _fakeUserManager;
        private readonly Guid _fakeUserId;
        private readonly ApplicationUser _fakeUser;

        public UserMongoRepositoryTests()
        {
            _fakeUserManager = A.Fake<UserManager<ApplicationUser>>();
            _fakeUserId = Guid.NewGuid();
            _fakeUser = new ApplicationUser
            {
                Id = _fakeUserId,
                Claims = new List<MongoClaim>
                {
                    new()
                    {
                        Value = "TestTenant",
                        Type = "tenant"
                    },
                    new()
                    {
                        Value = "Test User",
                        Type = "name"
                    }
                },
                UserName = "TestUser"
            };
            var fakeUser2 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Claims = new List<MongoClaim>
                {
                    new()
                    {
                        Value = "OtherTenant",
                        Type = "tenant"
                    },
                    new()
                    {
                        Value = "Test User Two",
                        Type = "name"
                    }
                },
                UserName = "TestUser2"
            };

            A.CallTo(() => _fakeUserManager.FindByIdAsync(_fakeUserId.ToString())).Returns(_fakeUser);
            A.CallTo(() => _fakeUserManager.GetRolesAsync(_fakeUser)).
                Returns(new List<string> { "wallet.admin" });

            var fakeUserQueryable = new List<ApplicationUser>{_fakeUser, fakeUser2}.AsQueryable();
            A.CallTo(() => _fakeUserManager.Users).Returns(fakeUserQueryable);

            _sut = new UserMongoRepository(_fakeUserManager);
        }

        [Fact]
        public async Task UpdateUserRole()
        {
            // Arrange
            var user = new User
            {
                Id = _fakeUserId,
                Roles = new List<string>
                {
                    "wallet.user"
                }
            };

            // Act
            await _sut.Update(user);

            // Assert
            A.CallTo(() => _fakeUserManager.RemoveFromRolesAsync(_fakeUser, A<List<string>>.That.Matches(role => role.Count == 1 && role.First() == "wallet.admin")))
                .MustHaveHappenedOnceExactly();

            A.CallTo(() => _fakeUserManager.AddToRolesAsync(_fakeUser, A<List<string>>.That.Matches(role => role.Count == 1 && role.First() == "wallet.user")))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task GetUser()
        {
            // Act
            var user = await _sut.Get(_fakeUserId);

            // Assert
            AssertUserIsValid(user);
        }

        [Fact]
        public async Task DeleteUser()
        {
            // Act
            await _sut.Delete(_fakeUserId);

            // Assert
            A.CallTo(() => _fakeUserManager.DeleteAsync(_fakeUser))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task UpdateUserRole_NullRoles()
        {
            // Arrange
            var user = new User
            {
                Id = _fakeUserId,
                Roles = null
            };

            // Act
            await _sut.Update(user);

            // Assert
            A.CallTo(() => _fakeUserManager.RemoveFromRolesAsync(_fakeUser, A<List<string>>.That.Matches(role => role.Count == 1 && role.First() == "wallet.admin")))
                .MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task ListByTenant()
        {
            // Act
            var users = await _sut.ListByTenant("TestTenant");

            // Assert
            A.CallTo(() => _fakeUserManager.Users)
                .MustHaveHappenedOnceExactly();

            Assert.Single(users);
            var user = users.First();
            AssertUserIsValid(user);
        }

        private void AssertUserIsValid(User user)
        {
            Assert.Equal("TestTenant", user.Tenant);
            Assert.Equal(_fakeUser.Claims.First(c => c.Type == "name").Value, user.UserName);
            Assert.Equal(_fakeUserId, user.Id);
            Assert.Single(user.Roles);
            Assert.Equal("wallet.admin", user.Roles.First());
        }
    }
}
