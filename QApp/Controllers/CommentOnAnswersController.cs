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
    public class CommentOnAnswersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CommentOnAnswers
        public ActionResult Index()
        {
            var commentOnAnswers = db.CommentOnAnswers.Include(c => c.Answer).Include(c => c.User);
            return View(commentOnAnswers.ToList());
        }

        // GET: CommentOnAnswers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentOnAnswer commentOnAnswer = db.CommentOnAnswers.Find(id);
            if (commentOnAnswer == null)
            {
                return HttpNotFound();
            }
            return View(commentOnAnswer);
        }

        // GET: CommentOnAnswers/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email");
            return View();
        }

        // POST: CommentOnAnswers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,Created")] CommentOnAnswer commentOnAnswer, int? Aid)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Questions");
            }
            if (Aid == null)
            {
                return RedirectToAction("Index", "Questions");
            }
            commentOnAnswer.AnswerId = (int)Aid;
            commentOnAnswer.UserId = User.Identity.GetUserId();
            commentOnAnswer.Created = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.CommentOnAnswers.Add(commentOnAnswer);
                db.SaveChanges();
                return RedirectToAction("Index","Questions");
            }

            ViewBag.AnswerId = new SelectList(db.Answers, "Id", "Text", commentOnAnswer.AnswerId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", commentOnAnswer.UserId);
            return View(commentOnAnswer);
        }

        // GET: CommentOnAnswers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentOnAnswer commentOnAnswer = db.CommentOnAnswers.Find(id);
            if (commentOnAnswer == null)
            {
                return HttpNotFound();
            }
            ViewBag.AnswerId = new SelectList(db.Answers, "Id", "Text", commentOnAnswer.AnswerId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", commentOnAnswer.UserId);
            return View(commentOnAnswer);
        }

        // POST: CommentOnAnswers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AnswerId,Text,Created,UserId")] CommentOnAnswer commentOnAnswer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentOnAnswer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AnswerId = new SelectList(db.Answers, "Id", "Text", commentOnAnswer.AnswerId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "Email", commentOnAnswer.UserId);
            return View(commentOnAnswer);
        }

        // GET: CommentOnAnswers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentOnAnswer commentOnAnswer = db.CommentOnAnswers.Find(id);
            if (commentOnAnswer == null)
            {
                return HttpNotFound();
            }
            return View(commentOnAnswer);
        }

        // POST: CommentOnAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommentOnAnswer commentOnAnswer = db.CommentOnAnswers.Find(id);
            db.CommentOnAnswers.Remove(commentOnAnswer);
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
