using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Test.BL;
using Test.BL.Providers;
using Test.Core.Domain;
using Test.Core.Interfaces;

namespace TestingBisnessLayer
{
    [TestClass]
    public class TestingTestProvider
    {
        [TestMethod]
        public void CreateTestResult_EnterGoodDatas_MethodCreateWasCalledNoExceptionWasRaised()
        {
            //Arange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetById(It.IsAny<int>())).Returns(new User());

            var mockTestRep = new Mock<IRepository<Exercise>>();
            mockTestRep.Setup(m => m.GetById(It.IsAny<int>())).Returns(new Exercise(){ Questions = new Collection<Question>() });

            var mockResultRep = new Mock<IRepository<TestResult>>();
            mockResultRep.Setup(m => m.Create(It.IsAny<TestResult>())).Verifiable();

            var mockQuestiontRep = new Mock<IRepository<Question>>();

            var mockAnswerRep = new Mock<IRepository<Answer>>();
            mockAnswerRep.Setup(m => m.GetAll()).Returns(new List<Answer>());

            var testProvider = new TestProvider(mockUserRep.Object,
                mockTestRep.Object,
                mockResultRep.Object,
                mockQuestiontRep.Object,
                mockAnswerRep.Object);
            //Act
            testProvider.CreateTestResult("1,0,2,3,4");
            //Assert
            mockResultRep.Verify();
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void CreateTestResult_EnterWrongDatas_ExceptionWasRaised()
        {
            //Arange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetById(It.Is<int>(s => s == -1))).Returns((User)null);

            var mockTestRep = new Mock<IRepository<Exercise>>();
            mockTestRep.Setup(m => m.GetById(It.IsAny<int>())).Returns(new Exercise() { Questions = new Collection<Question>() });

            var mockResultRep = new Mock<IRepository<TestResult>>();
            mockResultRep.Setup(m => m.Create(It.IsAny<TestResult>())).Verifiable();

            var mockQuestiontRep = new Mock<IRepository<Question>>();

            var mockAnswerRep = new Mock<IRepository<Answer>>();
            mockAnswerRep.Setup(m => m.GetAll()).Returns(new List<Answer>());

            var testProvider = new TestProvider(mockUserRep.Object,
                mockTestRep.Object,
                mockResultRep.Object,
                mockQuestiontRep.Object,
                mockAnswerRep.Object);
            //Act
            testProvider.CreateTestResult("-1,0,2,3,4");
        }

        [TestMethod]
        public void HasRightAnswer_QuestionWithRightAnswer_True()
        {
            //Arange
            var question = new Question() { Answers = new Collection<Answer>() { new Answer() { IsRight = true }, new Answer() { IsRight = false } } };
            var testProvider = new TestProvider(null, null, null, null, null);

            //Act
            var result = testProvider.HasRightAnswer(question);

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasRightAnswer_QuestionWithNoRightAnswer_False()
        {
            //Arange
            var question = new Question() { Answers = new Collection<Answer>() { new Answer() { IsRight = false } } };
            var testProvider = new TestProvider(null, null, null, null, null);

            //Act
            var result = testProvider.HasRightAnswer(question);

            //Assert
            Assert.IsFalse(result);
        }
    }
}
