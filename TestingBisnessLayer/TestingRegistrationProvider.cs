using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
using System.Web.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Test.BL.Providers;
using Test.Core.Domain;
using Test.Core.Domain.Enum;
using Test.Core.Interfaces;

namespace TestingBisnessLayer
{
    [TestClass]
    public class TestingRegistrationProvider
    {
        [TestMethod]
        public void HashFunc_CorrectData_HashedString()
        {
            //Arrange
            var hashedString = "fd186dd49a16b1bf2bd2f44e495e14c9";

            //Act
            var hashedStringByRegProvider = RegistrationProvider.Hash("hello");

            //Assert
            Assert.AreEqual(hashedString, hashedStringByRegProvider);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HashFunc_WrongData_HashedString()
        {
            //Act
            RegistrationProvider.Hash(null);
        }

        [TestMethod]
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
            Assert.IsNotNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetAuthenticationTicket_WrongLogin_ExceptionWasRaised()
        {
            //Arrange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetAll()).Returns(new List<User>() { new User() { Login = "user1", Id = 1 } });

            var regProvider = new RegistrationProvider(mockUserRep.Object, null);

            string wrongUserLogin = "wronglogin";

            //Act
            var result = regProvider.GetAuthenticationTicket(wrongUserLogin);
        }

        [TestMethod]
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
            Assert.IsTrue(result);
        }

        [TestMethod]
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
            Assert.IsTrue(result);
        }

        [TestMethod]
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
            Assert.AreEqual(1, user.Roles.Count);
        }

        [TestMethod]
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
            Assert.AreEqual(0, user.Roles.Count);
        }

        [TestMethod]
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
            Assert.IsTrue(result);
        }

        [TestMethod]
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
            Assert.AreEqual(1, userCollection.Count);
        }
    }
}
