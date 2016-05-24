using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using Test.BL.Providers;
using Test.Core.Domain;
using Test.Core.Domain.Enum;
using Test.Core.Interfaces;
using Xunit;

namespace xUnitTesting
{
    public class TestingRegistrationProvider
    {
        [Fact]
        public void HashFunc_CorrectData_HashedString()
        {
            //Arrange
            var hashedString = "fd186dd49a16b1bf2bd2f44e495e14c9";

            //Act
            var hashedStringByRegProvider = RegistrationProvider.Hash("hello");

            //Assert
            Assert.Equal(hashedString, hashedStringByRegProvider);
        }

        [Fact]
        public void HashFunc_WrongData_HashedString()
        {
            //Act
            Assert.Throws<ArgumentNullException>(() => RegistrationProvider.Hash(null));
        }

        [Fact]
        public void GetAuthenticationTicket_CorrectData_FormAuthTicket()
        {
            //Arrange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { new User() { Login = "user1", Id = 1 } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, null);

            string userLogin = "user1";

            //Act
            var result = regProvider.GetAuthenticationTicket(userLogin);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetAuthenticationTicket_WrongLogin_ExceptionWasRaised()
        {
            //Arrange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { new User() { Login = "user1", Id = 1 } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, null);

            string wrongUserLogin = "wronglogin";

            //Act
            var ex = Assert.Throws<Exception>(() => regProvider.GetAuthenticationTicket(wrongUserLogin));

            //Assert
            Assert.Equal("No user with this login was found", ex.Message);
        }

        [Fact]
        public void IsLoginFree_LoginFree_True()
        {
            //Arrange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { new User() { Login = "user1", Id = 1 } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, null);

            string freeUserLogin = "wronglogin";

            //Act
            var result = regProvider.IsLoginFree(freeUserLogin);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void IsEmailFree_EmailFree_True()
        {
            //Arrange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { new User() { Email = "user1", Id = 1 } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, null);

            string freeUserEmail = "wronglogin";

            //Act
            var result = regProvider.IsEmailFree(freeUserEmail);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ConfirmEmail_CorrectData_UserRolesCountPlusOne()
        {
            //Arrange
            var user = new User() { Login = "user1", Email = "user1", Id = 1, Roles = new Collection<Role>() };
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { user });

            var mockRoleRep = new Mock<IRepository<Role>>();
            mockRoleRep.Setup(m => m.GetAll()).Returns(new List<Role>() { new Role() { Name = RolesName.Student } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, mockRoleRep.Object);

            //Act
            regProvider.ConfirmEmail(user.Login, user.Email);

            //Assert
            Assert.Equal(1, user.Roles.Count);
        }

        [Fact]
        public void ConfirmEmail_WrongData_UserRolesCountTheSame()
        {
            //Arrange
            var user = new User() { Login = "user1", Email = "user1", Id = 1, Roles = new Collection<Role>() };
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { user });

            var mockRoleRep = new Mock<IRepository<Role>>();
            mockRoleRep.Setup(m => m.GetAll()).Returns(new List<Role>() { new Role() { Name = RolesName.Student } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, mockRoleRep.Object);

            //Act
            regProvider.ConfirmEmail(user.Login, "mail");

            //Assert
            Assert.Equal(0, user.Roles.Count);
        }

        [Fact]
        public void CheckUser_CorrectData_True()
        {
            //Arrange
            var user = new User() { Login = "user1", Password = "ba28b76588fd590d8a853087ecf7a6c0" };
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { user });

            var unhashedPassword = "h";

            var regProvider = new RegistrationProvider(mockUserRep.Object, null);
            //Act
            var result = regProvider.CheckUser(user.Login, unhashedPassword);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void CreateUser_CorrectData_CollectionCountPlusOne()
        {
            //Arrange
            var user = new User() { Login = "user1", Password = "h", Roles = new List<Role>() };
            var userCollection = new List<User>();

            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.Create(It.IsAny<User>())).Callback<User>(m => userCollection.Add(user));

            var mockRoleRep = new Mock<IRepository<Role>>();
            mockRoleRep.Setup(m => m.GetAll()).Returns(new List<Role>() { new Role() { Name = RolesName.RegisteredUser } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, mockRoleRep.Object);
            //Act
            regProvider.CreateUser(user);

            //Assert
            Assert.Equal(1, userCollection.Count);
        }
    }
}
