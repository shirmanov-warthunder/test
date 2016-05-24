using System.Collections.Generic;
using PagedList;
using Test.Core.Domain;

namespace Test.BL
{
    public interface ITestProvider
    {
        #region getters
        User GetUser(string name);

        IEnumerable<User> GetUsers();

        Exercise GetTest(int id);

        IPagedList<Exercise> GetTests(int page);

        IEnumerable<Exercise> GetTests();

        Question GetQuestion(int id);

        ICollection<Question> GetQuestions(int id);

        Answer GetAnswer(int id);

        IEnumerable<Answer> GetAnswers(int id);
        #endregion

        #region create items
        void CreateTest(Exercise test);

        void CreateQuestion(Question question);

        void CreateAnswer(Answer answer);

        void CreateTestResult(string datas);
        #endregion

        #region update items
        void UpdateTest(Exercise model);

        void UpdateQuestion(Question model);
        #endregion

        bool HasRightAnswer(Question question);

        void DeleteTest(int id);

        void DeleteQuestion(int id);
    }
}
