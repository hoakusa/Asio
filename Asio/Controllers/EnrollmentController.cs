using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Asio.Models;
using IdentitySample.Models;
using System.Web.UI;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;

namespace Asio.Controllers
{
    [Authorize]
    public class EnrollmentController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Enrollment
        [Authorize(Roles = "Student")]
        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();

            Student student = db.Students.FirstOrDefault(s => s.UserId == currentUserId);
            if (student == null) // Check student is existed or not
            {
                return HttpNotFound();
            }

            // Select Passed Courses from Enrollment table
            var enrollments = db.Enrollments.Where(e => e.StudentId == student.Id).ToList();
            return View(enrollments);
        }

        // GET: Enrollment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollment/Create
        public ActionResult Create()
        {
            var viewModel = new EnrollmentViewModel();

            // Teacher can enroll to his course only
            if (User.IsInRole("Teacher"))
            {
                string currentUserId = User.Identity.GetUserId();
                var courses = db.Courses.Where(c => c.Teacher.User.Id == currentUserId).ToList();
                viewModel.Courses = courses;
            }

            // Student can enroll to courses he has not joined
            if (User.IsInRole("Student"))
            {
                string currentUserId = User.Identity.GetUserId();

                Student student = db.Students.FirstOrDefault(s => s.UserId == currentUserId);
                if (student == null) // Check student is existed or not
                {
                    return HttpNotFound();
                }
                // Select Passed course from Enrollment table
                var enrollments = db.Enrollments.Where(e => e.StudentId == student.Id).ToList();
                var courses = db.Courses.ToList();

                if (enrollments == null)
                {
                    return View(courses);
                }

                foreach (Enrollment enrollment in enrollments)
                {
                    Course course = db.Courses.FirstOrDefault(c => c.Id == enrollment.CourseId);
                    if (course == null)
                    {
                        return HttpNotFound();
                    }
                    courses.Remove(course);
                }
                viewModel.Courses = courses;
                viewModel.UserName = User.Identity.GetUserName();
           }

            if (User.IsInRole("Admin"))
            {
                var courses = db.Courses.Include(c => c.Semester).ToList();
                viewModel.Courses = courses;
            }
            //viewModel.Courses = db.Courses.ToList();
            ViewData["Error"] = null;
            ViewData["Success"] = null;
            return View(viewModel);
        }

        // POST: Enrollment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,SelectedCourses")] EnrollmentViewModel enrollModel)
        {
            if (ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine(enrollModel.UserName);

                ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName == enrollModel.UserName);
                if (user != null) //check user is existed or not
                {
                    Student student = db.Students.FirstOrDefault(s => s.UserId == user.Id);
                    if (student != null) //check student is existed or not
                    {
                        EnrollmentViewModel enrollment = new EnrollmentViewModel();
                        enrollment.SelectedCourses = enrollModel.SelectedCourses;
                        foreach (var item in enrollModel.SelectedCourses)
                        {
                            //check Enrollment is exited or not
                            Enrollment enroll = db.Enrollments.FirstOrDefault(e => e.StudentId == student.Id && e.CourseId == item);
                            if (enroll == null)
                            {
                                enroll = new Enrollment { StudentId = student.Id, CourseId = item };
                                db.Enrollments.Add(enroll);
                                db.SaveChanges();
                            }
                            else {
                                ViewData["Error"] += db.Courses.Find(item).Name + ", ";
                                enrollModel.Courses = db.Courses.ToList();
                                return View(enrollModel);
                            }
                        }
                    }
                    else {
                        ViewData["Error"] = "Student is not existed! Please input other!";
                        enrollModel.Courses = db.Courses.ToList();
                        return View(enrollModel);
                    }
                }
                else {
                    ViewData["Error"] = "User is not existed! Please input other!";
                    enrollModel.Courses = db.Courses.ToList();
                    return View(enrollModel);
                }               

            }

            //Enroll successfully!
            TempData["Success"] = "Enrollment successfully!";
            return RedirectToAction("Create");
        }

        // GET: Enrollment/Edit/5
        [Authorize(Roles = "Teacher")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.Course = enrollment.Course;
            ViewBag.Student = enrollment.Student;
            ViewData["Success"] = null;
            return View(enrollment);
        }

        // POST: Enrollment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StudentId,CourseId,Grade,IsPassed")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                db.SaveChanges();
            }

            enrollment = db.Enrollments.FirstOrDefault(e => e.StudentId == enrollment.StudentId);
            ViewBag.Course = enrollment.Course;
            ViewBag.Student = enrollment.Student;
            TempData["Success"] = "Enrollment successfully!";
            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        // GET: Enrollment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = db.Enrollments.Find(id);

            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        [Authorize(Roles = "Admin")]
        // POST: Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Enrollment enrollment = db.Enrollments.Find(id);
            db.Enrollments.Remove(enrollment);
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
