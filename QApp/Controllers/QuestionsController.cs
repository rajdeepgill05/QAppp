using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QApp.Models;
using PagedList;
using PagedList.Mvc;

namespace QApp.Controllers
{
    public class QuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        
        // GET: Questions
        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
           
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var questions = db.Questions.Include(q => q.User);

            if (!String.IsNullOrEmpty(searchString))
            {
                questions = questions.Where(s => s.Description.Contains(searchString));                               
            }
            switch (sortOrder)
            {
                
                case "date_desc":
                    questions = questions.OrderByDescending(s => s.Created);
                    break;
                default:
                    questions = questions.OrderBy(s => s.Title);
                    break;
            }


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(questions.ToPagedList(pageNumber, pageSize));

        }   

        // GET: All the Answers attached to a question
        public ActionResult DisplayAnswerofQuestion(int? qid)
        {
            if (qid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var answers = db.Answers.Where(x => x.QuestionId == qid).ToList();

            return View(answers);
        }

        // GET: All the comments attached to aquestion
        public ActionResult DisplayCommentofQuestion(int? qid)
        {
            if (qid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comments = db.CommentsOnQuestions.Where(x => x.QuestionId == qid).ToList();

            return View(comments);
        }

        //Upvote
        public ActionResult Upvote(int? qid)
        {
            if (qid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(qid);
            if (question == null)
            {
                return HttpNotFound();
            }
            if (question.UserId != User.Identity.GetUserId())
            {
                question.Votes++;
                var user = db.Users.Find(question.UserId);
                user.ReputationCount += 5;
                db.SaveChanges();

            }


            return RedirectToAction("Index");
        }

        //DownVote
        public ActionResult Downvote(int? qid)
        {
            if (qid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(qid);
            if (question == null)
            {
                return HttpNotFound();
            }
            if (question.UserId != User.Identity.GetUserId())
            {
                question.Votes--;
                var user = db.Users.Find(question.UserId);
                user.ReputationCount -= 5;
                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }

        // GET: Questions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: Questions/Create
        public ActionResult Create()
        { 
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description")] Question question)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            question.UserId = User.Identity.GetUserId();
            question.Created = DateTime.Now;
            question.Updated = DateTime.Now;
            question.Votes = 0;

            if (ModelState.IsValid)
            {
                db.Questions.Add(question);
                db.SaveChanges();
                int qid = db.Questions.ToList().Where(x => x == question).FirstOrDefault().Id;
                return RedirectToAction("AssignTag", new { questionId = qid});
            }
                       
            return View(question);
        }

        //Assign Tags to Questions
        public ActionResult AssignTag(int? questionId)
        {
            if(questionId == null)
            {
                return RedirectToAction("Index");
            }
            var tags = db.Tags.ToList();
            var question = db.Questions.Find(questionId);
            var AllSelectedTags = new List<string>();
            ViewBag.Tags = tags;
            db.QuestionTags.ToList().Where(x => x.QuestionId == question.Id).Select(x => x.TagId).ToList().ForEach(x => 
            AllSelectedTags.Add(db.Tags.Find(x).Category));
            ViewBag.SelectedTags = AllSelectedTags;

            return View(question);
        }

        //Assign Tags POST method
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AssignTagConfirm(int? Id, int? TagId)
        {
            var question = db.Questions.Find(Id);
            var tag = db.Tags.Find(TagId);
            QuestionTag qt = new QuestionTag();
            qt.TagId = tag.Id;
            qt.QuestionId = question.Id;
            db.QuestionTags.Add(qt);
            db.SaveChanges();
            return RedirectToAction("AssignTag", new { questionId = question.Id });
        }


        // GET: Questions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", question.UserId);
            return View(question);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,UserId,Created,Updated,Votes")] Question question)
        {
            if (ModelState.IsValid)
            {
                db.Entry(question).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", question.UserId);
            return View(question);
        }

        // GET: Questions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = db.Questions.Find(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = db.Questions.Find(id);
            db.Questions.Remove(question);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
