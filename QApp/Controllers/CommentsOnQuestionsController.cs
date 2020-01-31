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
    public class CommentsOnQuestionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CommentsOnQuestions
        public ActionResult Index()
        {
            var commentsOnQuestions = db.CommentsOnQuestions.Include(c => c.Question).Include(c => c.User);
            return View(commentsOnQuestions.ToList());
        }

        // GET: CommentsOnQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentsOnQuestion commentsOnQuestion = db.CommentsOnQuestions.Find(id);
            if (commentsOnQuestion == null)
            {
                return HttpNotFound();
            }
            return View(commentsOnQuestion);
        }

        // GET: CommentsOnQuestions/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: CommentsOnQuestions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,Created")] CommentsOnQuestion commentsOnQuestion, int? Qid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Questions");
            }
            if (Qid == null)
            {
                return RedirectToAction("Index", "Questions");
            }
            commentsOnQuestion.QuestionId = (int)Qid;
            commentsOnQuestion.UserId = User.Identity.GetUserId();
            commentsOnQuestion.Created = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.CommentsOnQuestions.Add(commentsOnQuestion);
                db.SaveChanges();
                return RedirectToAction("Index", "Questions");
            }

            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", commentsOnQuestion.QuestionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", commentsOnQuestion.UserId);
            return View(commentsOnQuestion);
        }

        // GET: CommentsOnQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentsOnQuestion commentsOnQuestion = db.CommentsOnQuestions.Find(id);
            if (commentsOnQuestion == null)
            {
                return HttpNotFound();
            }
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", commentsOnQuestion.QuestionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", commentsOnQuestion.UserId);
            return View(commentsOnQuestion);
        }

        // POST: CommentsOnQuestions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,QuestionId,Text,Created,UserId")] CommentsOnQuestion commentsOnQuestion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentsOnQuestion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.QuestionId = new SelectList(db.Questions, "Id", "Title", commentsOnQuestion.QuestionId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", commentsOnQuestion.UserId);
            return View(commentsOnQuestion);
        }

        // GET: CommentsOnQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentsOnQuestion commentsOnQuestion = db.CommentsOnQuestions.Find(id);
            if (commentsOnQuestion == null)
            {
                return HttpNotFound();
            }
            return View(commentsOnQuestion);
        }

        // POST: CommentsOnQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommentsOnQuestion commentsOnQuestion = db.CommentsOnQuestions.Find(id);
            db.CommentsOnQuestions.Remove(commentsOnQuestion);
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
