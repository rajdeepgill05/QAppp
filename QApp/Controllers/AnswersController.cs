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

namespace QApp.Controllers
{
    public class AnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Answers
        public ActionResult Index()
        {
            var answers = db.Answers.Include(a => a.Question).Include(a => a.User);
            return View(answers.ToList());
        }

        //Upvote
        public ActionResult Upvote(int? aid)
        {
            if (aid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(aid);
            if (answer == null)
            {
                return HttpNotFound();
            }
            if (answer.UserId != User.Identity.GetUserId())
            {
                answer.Votes++;
                var user = db.Users.Find(answer.UserId);
                user.ReputationCount += 5;
                db.SaveChanges();

            }


            return RedirectToAction("Index");
        }

        //DownVote
        public ActionResult Downvote(int? aid)
        {
            if (aid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(aid);
            if (answer == null)
            {
                return HttpNotFound();
            }
            if (answer.UserId != User.Identity.GetUserId())
            {
                answer.Votes--;
                var user = db.Users.Find(answer.UserId);
                user.ReputationCount -= 5;
                db.SaveChanges();

            }

            return RedirectToAction("Index");
        }

        //GET: Comments on a particular Answer
        public ActionResult DisplayCommentonAnswer(int? aid)
        {
            if (aid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comments = db.CommentOnAnswers.Where(x => x.AnswerId == aid).ToList();
            if (comments == null)
            {
                comments = new List<CommentOnAnswer>();
            }
            return View(comments);

        }

            // GET: Answers/Details/5
            public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // GET: Answers/Create
        public ActionResult Create()
        {
           
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: Answers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,Created,QuestionId,Votes")] Answer answer, int? Qid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Questions");
            }
            if (Qid == null)
            {
                return RedirectToAction("Index", "Questions");
            }
            answer.QuestionId = (int)Qid;
            answer.UserId = User.Identity.GetUserId();
            answer.Created = DateTime.Now;
            answer.Votes = 0;
            if (ModelState.IsValid)
            {
                db.Answers.Add(answer);
                db.SaveChanges();
                return RedirectToAction("Index", "Questions");
            }

            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", answer.QuestionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", answer.UserId);
            return RedirectToAction("Index", "Questions");
        }

        // GET: Answers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", answer.QuestionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", answer.UserId);
            return View(answer);
        }

        // POST: Answers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Text,Created,UserId,QuestionId,Votes")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(answer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", answer.QuestionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", answer.UserId);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Answer answer = db.Answers.Find(id);
            if (answer == null)
            {
                return HttpNotFound();
            }
            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Answer answer = db.Answers.Find(id);
            db.Answers.Remove(answer);
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
