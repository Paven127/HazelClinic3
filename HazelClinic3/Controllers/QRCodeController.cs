using HazelClinic3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HazelClinic3.Controllers
{
    public class QRCodeController : Controller
    {
        private DataContext db = new DataContext();
        // GET: QRCode
        public ActionResult ViewQRCode()
        {
            string email = (string)Session["GlobalEmail"];
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home"); // Redirect to home if session email is missing
            }

            // Assuming you have a method to get the latest appointment for the user
            var appointment = GetLatestAppointmentForUser(email);

            if (appointment == null)
            {
                return HttpNotFound(); // Or handle the case where no appointment is found
            }

            string qrCodeUrl = GenerateQRCodeUrl(appointment.AppointmentId.ToString()); // Ensure the ID is a string

            ViewBag.QRCodeUrl = qrCodeUrl;
            return View("~/Views/Home/ViewQRCode.cshtml");
        }

        private Appointment GetLatestAppointmentForUser(string email)
        {
            // Implement your logic to fetch the latest appointment based on email
            return db.AppTbl.FirstOrDefault(a => a.Email == email); // Adjust as necessary
        }

        private string GenerateQRCodeUrl(string appointmentId)
        {
            string placidApiToken = "placid-7p5t2vnvqly9z0rm-ujyko8ux7lxfrusw";
            string qrCodeTemplateUrl = "https://api.placid.app/u/cqo8lh3asj9tg"; // Your Placid QR Code template URL

            // Create the API request URL
            string requestUrl = $"{qrCodeTemplateUrl}?appointmentId={appointmentId}&token={placidApiToken}";

            return requestUrl;
        }

        public ActionResult Details(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home"); // Redirect to home if email is missing
            }

            var user = GetUserDetails(email);

            if (user == null)
            {
                return HttpNotFound(); // Or handle the case where no user is found
            }

            return View(user);
        }

        public ActionResult QRDetails(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home"); // Redirect to home if email is missing
            }

            return RedirectToAction("Details", new { email = email });
        }

        private User1 GetUserDetails(string email)
        {
            return db.Users2.FirstOrDefault(u => u.Email == email); // Adjust as necessary
        }
    }
}
