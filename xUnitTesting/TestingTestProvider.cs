using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using Test.BL.Providers;
using Test.Core.Domain;
using Test.Core.Interfaces;
using Xunit;

namespace xUnitTesting
{
    public class TestingTestProvider
    {
        [Fact]
        public void CreateTestResult_EnterGoodDatas_MethodCreateWasCalledNoExceptionWasRaised()
        {
            //Arange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetById(It.IsAny<int>())).Returns(new User());

            var mockTestRep = new Mock<IRepository<Exercise>>();
            mockTestRep.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns(new Exercise() {Questions = new Collection<Question>()});

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

        [Fact]
        public void CreateTestResult_EnterWrongDatas_ExceptionWasRaised()
        {
            //Arange
            var mockUserRep = new Mock<IRepository<User>>();
            mockUserRep.Setup(m => m.GetById(It.Is<int>(s => s == -1))).Returns((User) null);

            var mockTestRep = new Mock<IRepository<Exercise>>();
            mockTestRep.Setup(m => m.GetById(It.IsAny<int>()))
                .Returns(new Exercise() {Questions = new Collection<Question>()});

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
            var exception = Assert.Throws<NullReferenceException>(() => testProvider.CreateTestResult("-1,0,2,3,4"));

            //Assert
            Assert.Equal("User or test was uncorrect, can't find any in db", exception.Message);
        }

        [Fact]
        public void HasRightAnswer_QuestionWithRightAnswer_True()
        {
            //Arange
            var question = new Question()
            {
                Answers = new Collection<Answer>() { new Answer() { IsRight = true }, new Answer() { IsRight = false } }
            };
            var testProvider = new TestProvider(null, null, null, null, null);

            //Act
            var result = testProvider.HasRightAnswer(question);

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void HasRightAnswer_QuestionWithNoRightAnswer_False()
        {
            //Arange
            var question = new Question() {Answers = new Collection<Answer>() { new Answer() { IsRight = false } } };
            var testProvider = new TestProvider(null, null, null, null, null);

            //Act
            var result = testProvider.HasRightAnswer(question);

            //Assert
            Assert.False(result);
        }
    }
}