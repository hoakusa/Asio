using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Asio.Models;
using IdentitySample.Models;
using Microsoft.AspNet.Identity;

namespace Asio.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Course
        public ActionResult Index()
        {
            if (User.IsInRole("Teacher")) {
                string currentUserId = User.Identity.GetUserId();
                var courses = db.Courses.Where(c => c.Teacher.User.Id == currentUserId).ToList();
                return View(courses);
            }

            if (User.IsInRole("Student"))
            {
                string currentUserId = User.Identity.GetUserId();

                Student student = db.Students.FirstOrDefault(s => s.UserId == currentUserId);
                if (student != null) { // Check student is existed or not

                    // Select Passed Courses from Enrollment table
                    var enrollments = db.Enrollments.Where(e => e.StudentId == student.Id && e.IsPassed == true).ToList();
                    List<Course> courses = new List<Course>();

                    if (enrollments == null) {
                        return View(courses);
                    }

                    foreach (Enrollment enrollment in enrollments) {
                        Course course = db.Courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
                        if (course != null) {
                            courses.Add(course);
                        }
                    }
                    return View(courses.ToList());

                }

            }

            if (User.IsInRole("Admin"))
            {
                var courses = db.Courses.Include(c => c.Semester);
                return View(courses.ToList());
            }

            return View();
        }

        // GET: Course/Details
        public ActionResult Details(int? id, int? enrollmentId)
        {
            if (id != null)
            {
                Course course = db.Courses.Find(id);
                if (course == null)
                {
                    return HttpNotFound();
                }
            }

            if (enrollmentId != null && (User.IsInRole("Teacher") || User.IsInRole("Admin"))) {
                Enrollment enrollment = db.Enrollments.Find(enrollmentId);
                db.Enrollments.Remove(enrollment);
                db.SaveChanges();
            }         

            var viewModel = new CourseViewModel();
            viewModel.Course = db.Courses.Find(id);
            if (User.IsInRole("Teacher") || User.IsInRole("Admin"))
            {
                viewModel.Enrollments = db.Enrollments.Where(s => s.CourseId == id).ToList();
            }

            return View(viewModel);
        }

        // GET: Course/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.SemesterId = new SelectList(db.Semesters, "Id", "Name");
            //ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "UserId");

            var teacher = db.Teachers.Include(c => c.User).Select(c => new SelectListItem {
                Value = c.Id.ToString(),
                Text = c.User.FirstName + " " + c.User.LastName + " ( " + c.User.UserName + " ) "
            });
            try
            {
                ViewBag.TeacherId = new SelectList(teacher, "Value", "Text");
            }
            catch (Exception e) {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return View();
        }

        // POST: Course/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Code,Name,Credits,TeacherId,SemesterId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SemesterId = new SelectList(db.Semesters, "Id", "Name", course.SemesterId);
            //ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "UserId", course.TeacherId);
            ViewBag.TeacherId = new SelectList(db.Teachers.Include(c => c.User), "Value", "Text", course.TeacherId);
            return View(course);
        }

        [Authorize(Roles = "Admin")]
        // GET: Course/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.SemesterId = new SelectList(db.Semesters, "Id", "Name", course.SemesterId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "UserId", course.TeacherId);
            return View(course);
        }

        // POST: Course/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,Name,Credits,TeacherId,SemesterId,CreatedTime")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SemesterId = new SelectList(db.Semesters, "Id", "Name", course.SemesterId);
            ViewBag.TeacherId = new SelectList(db.Teachers, "Id", "UserId", course.TeacherId);
            return View(course);
        }

        // GET: Course/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Course/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
