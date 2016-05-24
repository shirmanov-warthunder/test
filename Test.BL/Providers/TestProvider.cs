using System;
using System.Collections.Generic;
using System.Linq;
using PagedList;
using Test.Core.Domain;
using Test.Core.Domain.Enum;
using Test.Core.Interfaces;

namespace Test.BL.Providers
{
    public class TestProvider : ITestProvider
    {
        private const int BadResult = 40;
        private const int StillBadResult = 65;
        private const int MiddleResult = 75;
        private const int GoodResult = 90;
        private const int VeryGoodResult = 95;
        private const int amountOnPage = 10;

        #region prop and ctors

        private IRepository<User> userRepository;
        private IRepository<Exercise> testRepository;
        private IRepository<TestResult> resultRepository;
        private IRepository<Question> questionRepository;
        private IRepository<Answer> answerRepository;

        public TestProvider(IRepository<User> userRepository, 
            IRepository<Exercise> testRepository, 
            IRepository<TestResult> resultRepository, 
            IRepository<Question> questionRepository, 
            IRepository<Answer> answerRepository)
        {
            this.userRepository = userRepository;
            this.testRepository = testRepository;
            this.resultRepository = resultRepository;
            this.questionRepository = questionRepository;
            this.answerRepository = answerRepository;
        }

        #endregion

        #region getters
        public User GetUser(string name)
        {
            return this.userRepository.GetAll().FirstOrDefault(m => m.Login == name);
        }

        public IEnumerable<User> GetUsers()
        {
            return this.userRepository.GetAll();
        }

        public Exercise GetTest(int id)
        {
            return this.testRepository.GetById(id);
        }

        public IPagedList<Exercise> GetTests(int page)
        {
            return this.testRepository.GetAll().OrderByDescending(m => m.Id).ToPagedList(page, amountOnPage);
        }

        public IEnumerable<Exercise> GetTests()
        {
            return this.testRepository.GetAll();
        }

        public Question GetQuestion(int id)
        {
            return this.questionRepository.GetById(id);
        }

        public ICollection<Question> GetQuestions(int id)
        {
            return this.GetTest(id).Questions;
        }

        public Answer GetAnswer(int id)
        {
            return this.answerRepository.GetById(id);
        }

        public IEnumerable<Answer> GetAnswers(int id)
        {
            return this.questionRepository.GetById(id).Answers;
        }

        #endregion

        #region create new items
        public void CreateTest(Exercise test)
        {
            this.testRepository.Create(test);
            this.testRepository.Save();
        }

        public void CreateQuestion(Question question)
        {
            this.questionRepository.Create(question);
            this.questionRepository.Save();
        }

        public void CreateAnswer(Answer answer)
        {
            this.answerRepository.Create(answer);
            this.answerRepository.Save();
        }

        #endregion

        #region update items
        public void UpdateTest(Exercise model)
        {
            this.testRepository.Update(model);
            this.testRepository.Save();
        }

        public void UpdateQuestion(Question model)
        {
            this.questionRepository.Update(model);
            this.questionRepository.Save();
        }
        #endregion

        public bool HasRightAnswer(Question question)
        {
            return question.Answers.Any(answer => answer.IsRight);
        }

        public void CreateTestResult(string datas)
        {
            var answersSplit = datas.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var ids = new int[answersSplit.Length];
            for (var i = 0; i < answersSplit.Length; i++)
            {
                ids[i] = int.Parse(answersSplit[i]);
            }

            var user = this.userRepository.GetById(ids[0]);
            var test = this.GetTest(ids[1]);

            if (user == null || test == null)
            {
                throw new NullReferenceException("User or test was uncorrect, can't find any in db");
            }

            int j = 2;

            int rightAnswers = 0;

            foreach (var q in test.Questions)
            {
                if (ids[j] == -1)
                {
                    j++;
                }
                else if (this.GetAnswer(ids[j++]).IsRight)
                {
                    rightAnswers++;
                }
            }

            int result = (int)(((double)rightAnswers / test.Questions.Count()) * 100);

            Grade grade = this.CalcGrade(result);

            var testResult = new TestResult()
            {
                Exercise = test, 
                User = user, 
                Grade = grade, 
                TestDate = DateTime.Now
            };

            this.resultRepository.Create(testResult);
            this.resultRepository.Save();
        }

        private Grade CalcGrade(int result)
        {
            if (result < BadResult)
            {
                return Grade.F;
            }

            if (result >= BadResult && result < StillBadResult)
            {
                return Grade.E;
            }

            if (result >= StillBadResult && result < MiddleResult)
            {
                return Grade.D;
            }

            if (result >= MiddleResult && result < GoodResult)
            {
                return Grade.C;
            }

            if (result >= GoodResult && result < VeryGoodResult)
            {
                return Grade.B;
            }

            return Grade.A;
        }


        public void DeleteTest(int id)
        {
            var test = this.testRepository.GetById(id);

            this.testRepository.Delete(test);

            this.testRepository.Save();
        }


        public void DeleteQuestion(int id)
        {
            var answer = this.questionRepository.GetById(id);

            this.questionRepository.Delete(answer);

            this.questionRepository.Save();
        }
    }
}
