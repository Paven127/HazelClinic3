using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using HazelClinic3.Models;
using Newtonsoft.Json;

namespace HazelClinic3.Controllers
{
    public class VolunteersController : Controller
    {
        private DataContext db = new DataContext();

        // GET: Volunteers
     

        public ActionResult VolunteerInfo()
        {
            return View("VolunteerInfo");
        }

       public ActionResult VolunteerSuccess()
        {
            // Retrieve the email from the session variable
            var email = Session["VolunteerEmail"] as string;

            if (!string.IsNullOrEmpty(email))
            {
                // Send the confirmation email
                SendConfirmationEmail(email);

                // Clear the session variable to avoid resending the email if the user refreshes the page
                Session["VolunteerEmail"] = null;
            }

            return View();
        }



        // GET: Volunteers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            return View(volunteer);
        }

        // GET: Volunteers/Create
        public ActionResult Create()
        {
            InitializeSelectLists();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VolunteerId,FullName,Surname,Email,CellNo,EmergencyContactName,EmergencyContactCellNo,Experience,Availability,VolunteerType")] Volunteer volunteer)
        {
            // Check if the volunteer with the same email already exists
            var existingVolunteer = db.Volunteers.FirstOrDefault(v => v.Email == volunteer.Email);
            if (existingVolunteer != null)
            {
                ModelState.AddModelError("Email", "This email is already registered as a volunteer.");
            }

            if (ModelState.IsValid)
            {
                db.Volunteers.Add(volunteer);
                db.SaveChanges();

                // Store the email in the session variable
                Session["VolunteerEmail"] = volunteer.Email;

                return RedirectToAction("VolunteerSuccess");
            }

            InitializeSelectLists();
            return View(volunteer);
        }

        // Method to initialize SelectLists
        private void InitializeSelectLists()
        {
            ViewBag.Experience = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Experienced", Value = "Experienced" },
                new SelectListItem { Text = "Somewhat", Value = "Somewhat" },
                new SelectListItem { Text = "Little to none", Value = "Little to none" }
            }, "Value", "Text");

            ViewBag.Availability = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Weekdays", Value = "Weekdays" },
                new SelectListItem { Text = "Weekends", Value = "Weekends" },
                new SelectListItem { Text = "Both", Value = "Both" }
            }, "Value", "Text");

            ViewBag.VolunteerType = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Dog Walking", Value = "Dog Walking" },
                new SelectListItem { Text = "Cat Cuddling", Value = "Cat Cuddling" },
                new SelectListItem { Text = "Puppy Training", Value = "Puppy Training" },
                new SelectListItem { Text = "No Preference", Value = "No Preference" }
            }, "Value", "Text");
        }

        // Method to send confirmation email
        private void SendConfirmationEmail(string userEmail)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Volunteer Registration Confirmation";

            mail.Body = "Dear Volunteer,\n\n" +
                        "Thank you for registering to volunteer with SPCA Durban.\n\n" +
                        "We appreciate your support and will notify you pending the outcome of your request.\n\n" +
                        "Warm regards,\nSPCA Durban Team";

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }


// GET: Volunteers/Edit/5
public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VolunteerId,FullName,Surname,Email,CellNo,EmergencyContactName,EmergencyContactCellNo,Experience,Availability,VolunteerType")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(volunteer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(volunteer);
        }

        // GET: Volunteers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Volunteer volunteer = db.Volunteers.Find(id);
            if (volunteer == null)
            {
                return HttpNotFound();
            }
            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Volunteer volunteer = db.Volunteers.Find(id);
            db.Volunteers.Remove(volunteer);
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

        private void SendConfirmationEmail2(string userEmail, string subject, string body)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress("spcadurbanza@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Credentials = new System.Net.NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
            smtp.EnableSsl = true;

            smtp.Send(mail);
        }

        public ActionResult Approve(int id)
        {
            var volunteer = db.Volunteers.Find(id);
            if (volunteer != null)
            {
                volunteer.Status = "Approved";
                db.SaveChanges();

                // Determine orientation date based on availability
                string orientationDate = DetermineOrientationDate(volunteer.Availability);

                string subject = "Volunteer Application Approved";
                string body = $"Dear {volunteer.FullName},\n\n" +
                              "We are pleased to inform you that your volunteer application has been approved.\n\n" +
                              $"Your orientation is scheduled for {orientationDate}.\n\n";

                if (!string.IsNullOrEmpty(volunteer.VolunteerType) && volunteer.VolunteerType != "No Preference")
                {
                    body += $"You have been allocated to {volunteer.VolunteerType}.\n\n";
                }

                body += "Thank you for your willingness to support SPCA Durban.\n\n" +
                        "Warm regards,\nSPCA Durban Team";

                SendConfirmationEmail2(volunteer.Email, subject, body);
            }
            return RedirectToAction("ViewVolunteerRequests");
        }


        private string DetermineOrientationDate(string availability)
        {
            DateTime orientationDate;

            if (availability == "Weekends")
            {
                // Schedule for next Saturday
                orientationDate = DateTime.Today.AddDays((6 - (int)DateTime.Today.DayOfWeek + 7) % 7);
            }
            else if (availability == "Weekdays")
            {
                // Schedule for 2 days after the current day if it's a weekday
                // or the next Monday if it's currently a weekend
                orientationDate = DateTime.Today;
                if (orientationDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    orientationDate = orientationDate.AddDays(2);
                }
                else if (orientationDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    orientationDate = orientationDate.AddDays(1);
                }
                else
                {
                    orientationDate = orientationDate.AddDays(2);
                }
            }
            else if (availability == "Both")
            {
                // Schedule for 2 days after the current day
                orientationDate = DateTime.Today.AddDays(2);
            }
            else
            {
                // Default to 2 days after the current day if availability is unknown
                orientationDate = DateTime.Today.AddDays(2);
            }

            return orientationDate.ToString("dddd, MMMM dd, yyyy");
        }


        public ActionResult Decline(int id)
        {
            var volunteer = db.Volunteers.Find(id);
            if (volunteer != null)
            {
                // Create a new DeclinedVolunteer object
                var declinedVolunteer = new DeclinedVolunteer
                {
                    VolunteerId = volunteer.VolunteerId,
                    FullName = volunteer.FullName,
                    Surname = volunteer.Surname,
                    Email = volunteer.Email,
                    CellNo = volunteer.CellNo,
                    EmergencyContactName = volunteer.EmergencyContactName,
                    EmergencyContactCellNo = volunteer.EmergencyContactCellNo,
                    Experience = volunteer.Experience,
                    Availability = volunteer.Availability,
                    VolunteerType = volunteer.VolunteerType,
                    Status = "Declined"
                };

                // Add to DeclinedVolunteers table
                db.DeclinedVolunteers.Add(declinedVolunteer);

                // Remove from Volunteers table
                db.Volunteers.Remove(volunteer);

                // Save changes
                db.SaveChanges();

                // Send decline email
                string subject = "Volunteer Application Declined";
                string body = $"Dear {volunteer.FullName},\n\n" +
                              "We regret to inform you that your volunteer application has been declined as the type of volunteer work you are interested is already full.\n\n" +
                              "Thank you for your interest in supporting SPCA Durban.\n\n" +
                              "Warm regards,\nSPCA Durban Team";

                SendConfirmationEmail2(volunteer.Email, subject, body);
            }
            return RedirectToAction("ViewVolunteerRequests");
        }


        public ActionResult ViewVolunteerRequests()
        {
            var volunteers = db.Volunteers.Where(v => v.Status == null).ToList();
            return View(volunteers);
        }

        public ActionResult ApprovedRequests()
        {
            var approvedVolunteers = db.Volunteers.Where(v => v.Status == "Approved").ToList();
            return View(approvedVolunteers);
        }


        public ActionResult DeclinedRequests()
        {
            var declinedVolunteers = db.DeclinedVolunteers.ToList();
            return View(declinedVolunteers);
        }


        [HttpPost]
        public ActionResult AllocateVolunteer(int id, string newVolunteerType)
        {
            var volunteer = db.Volunteers.Find(id);
            if (volunteer != null && volunteer.VolunteerType == "No Preference")
            {
                volunteer.VolunteerType = newVolunteerType;
                db.SaveChanges();

                UpdateVolunteerTypeChart(); // Method to update the volunteer type chart
            }
            return RedirectToAction("ApprovedRequests");
        }

        private void UpdateVolunteerTypeChart()
        {
            var volunteerCounts = db.Volunteers
             .Where(v => v.Status == "Approved")
             .GroupBy(v => v.VolunteerType)
             .Select(group => new { Type = group.Key, Count = group.Count() })
             .ToList();

            var labels = volunteerCounts.Select(vc => vc.Type).ToArray();
            var counts = volunteerCounts.Select(vc => vc.Count).ToArray();

            ViewBag.ChartLabels = JsonConvert.SerializeObject(labels);
            ViewBag.ChartCounts = JsonConvert.SerializeObject(counts);
        }

        public ActionResult VolunteerTypeChart()
        {
            UpdateVolunteerTypeChart();
            return View();
        }

    }
}
