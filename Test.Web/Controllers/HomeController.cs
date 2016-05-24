using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using Common.Logging;
using Test.BL;
using Test.Core.Domain;
using Test.Web.Models;
using Test.Web.Models.Home;

namespace Test.Web.Controllers
{
    [Ban]
    public class HomeController : Controller
    {
        #region ctor and main page
        private readonly ITestProvider testProvider;

        public HomeController(ITestProvider testProvider)
        {
            this.testProvider = testProvider;
            Mapper.CreateMap<TestModel, Exercise>();
            Mapper.CreateMap<Exercise, TestModel>();
            Mapper.CreateMap<QuestionModel, Question>();
            Mapper.CreateMap<Question, QuestionModel>();
        }

        [Authorize(Roles = "Administrator, Tutor, Student, AdvancedStudent, AdvancedTutor")]
        public ActionResult Index(int page = 1)
        {
            var tests = this.testProvider.GetTests(page);
            return this.View(tests);
        }
        #endregion

        #region CreateTest
        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult CreateTest()
        {
            var model = new TestModel() { CreatorLogin = User.Identity.Name };
            return this.View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Tutor, AdvancedTutor")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTest(TestModel model)
        {
            var test = Mapper.Map<Exercise>(model);
            test.Creator = this.testProvider.GetUser(model.CreatorLogin);
            this.testProvider.CreateTest(test);

            return this.RedirectToAction("TestInfo", new { id = test.Id });
        }
        #endregion

        #region test info
        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult TestInfo(int id)
        {
            var test = this.testProvider.GetTest(id);
            return this.View(test);
        }

        [Authorize(Roles = "Tutor, AdvancedTutor")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult TestInfo(Exercise model)
        {
            this.testProvider.UpdateTest(model);
            return this.RedirectToAction("Index");
        }

        [Authorize(Roles = "Tutor, Advanced Tutor")]
        public PartialViewResult CreateQuestion(QuestionModel model)
        {
            var question = Mapper.Map<Question>(model);
            question.Test = this.testProvider.GetTest(model.TestId);
            this.testProvider.CreateQuestion(question);
            var result = this.testProvider.GetQuestions(model.TestId);
            return this.PartialView("_Questions", result);
        }
        #endregion

        #region answer info
        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult AnswerInfo(int id)
        {
            var question = this.testProvider.GetQuestion(id);

            ViewBag.HasRightAnswer = this.testProvider.HasRightAnswer(question);
            ViewBag.QuestionId = id;

            return this.View(question);
        }

        [Authorize(Roles = "Tutor, AdvancedTutor")]
        [HttpPost]
        public ActionResult AnswerInfo(Question model)
        {
            this.testProvider.UpdateQuestion(model);
            return this.RedirectToAction("Index");
        }

        [Authorize(Roles = "Tutor, Advanced Tutor")]
        public PartialViewResult CreateAnswer(AnswerModel model, string isRight)
        {
            var answer = new Answer()
            {
                Text = model.Text,
                Question = this.testProvider.GetQuestion(model.QuestionId),
                IsRight = isRight == "on" ? true : false
            };

            ViewBag.QuestionId = model.QuestionId;

            this.testProvider.CreateAnswer(answer);
            var result = this.testProvider.GetAnswers(model.QuestionId);
            return this.PartialView("_Answers", result);
        }
        #endregion

        #region Offline
        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult OfflinePass()
        {
            var model = new OfflineModel()
            {
                Students = this.testProvider.GetUsers().ToList().Where(u => Roles.IsUserInRole(u.Login, "Student")),
                Tests = this.testProvider.GetTests()
            };
            return this.View(model);
        }

        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult Print(int id)
        {
            var test = this.testProvider.GetTest(id);
            return this.View("OfflinePrint", test);
        }

        [HttpPost]
        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult GetQuestions(int id)
        {
            var result = this.testProvider.GetQuestions(id).ToList().Select(u => new
            {
                u.Id,
                u.Text,
                Answers = u.Answers.Select(a => new { a.Id, a.Text })
            });
            return this.Json(result);
        }

        [Authorize(Roles = "Tutor, AdvancedTutor")]
        public ActionResult OfflineResults(string answers)
        {
            this.testProvider.CreateTestResult(answers);
            return this.View();
        }
        #endregion

        #region Online pass
        public ActionResult PassTest(int id)
        {
            this.Session["answers"] = string.Format("{0},{1},", this.testProvider.GetUser(User.Identity.Name).Id, id);
            this.Session["count"] = 1;
            this.Session["testId"] = id;

            ViewBag.Count = this.testProvider.GetTest(id).Questions.Count();

            var q = this.testProvider.GetTest(id).Questions.Take(1).First();

            return this.View(q);
        }

        [HttpPost]
        public ActionResult PassTest(string test)
        {
            var logger = LogManager.GetLogger("PassTestLogger");
            this.Session["answers"] += string.Format("{0},", test);
            var num = (int)this.Session["count"];
            var id = (int)this.Session["testId"];

            if (this.testProvider.GetTest(id).Questions.Count() == num)
            {
                this.testProvider.CreateTestResult(this.Session["answers"].ToString());
                logger.Info(i => i("User {0} pass test {1}", User.Identity.Name, this.testProvider.GetTest(id).Name));
                return this.PartialView("TestComplete");
            }

            var q = this.testProvider.GetTest(id).Questions.Skip(num).Take(1).First();
            this.Session["count"] = ++num;
            return this.PartialView("_Online", q);
        }
        #endregion

        public void Error()
        {
            throw new Exception("this page is throws an error");
        }

        public ActionResult TestDetails(int id)
        {
            var model = new TestDetailsModel()
            {
                Name = this.testProvider.GetTest(id).Name,
                Description = this.testProvider.GetTest(id).Description,
                Results = this.testProvider.GetTest(id).Participants,
                Participants = this.testProvider.GetTest(id).Participants.Select(m => m.User)
            };

            return this.View(model);
        }

        public ActionResult TestDelete(int id)
        {
            this.testProvider.DeleteTest(id);
            return this.RedirectToAction("Index");
        }

        public ActionResult QuestionDelete(int id)
        {
            var testId = this.testProvider.GetQuestion(id).TestId;
            this.testProvider.DeleteQuestion(id);
            return this.RedirectToAction("TestInfo", new { id = testId });
        }
    }
}