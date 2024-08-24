using HazelClinic3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using Stripe;
using Stripe.Checkout;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.IO.Font.Constants;
using iText.IO.Font;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Image;



namespace Events.Controllers
{
    public class EventsController : Controller
    {
        private DataContext _context;

        public EventsController()
        {
            _context = new DataContext();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EventName,EventDate,EventTime,LimitOfAttendees,EventPrice,ArePetsAllowed,EventLocation")] HazelClinic3.Models.Event @event, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(image.InputStream))
                    {
                        @event.Image = reader.ReadBytes(image.ContentLength);
                    }
                }

                _context.Events.Add(@event);
                _context.SaveChanges();
                return RedirectToAction("AdminEvents");
            }

            return View(@event);
        }

        public ActionResult UserEvent()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        public ActionResult AdminEvents()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        public ActionResult GetEventImage(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event != null && @event.Image != null)
            {
                return File(@event.Image, "image/jpeg");
            }
            return HttpNotFound();
        }

        public ActionResult AddAuctionItem(int eventId)
        {
            var auctionItem = new AuctionItem { Event_Id = eventId };
            return View(auctionItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuctionItem([Bind(Include = "Id,Event_Id,Name")] AuctionItem auctionItem, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null && image.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(image.InputStream))
                    {
                        auctionItem.Image = reader.ReadBytes(image.ContentLength);
                    }
                }

                _context.AuctionItems.Add(auctionItem);
                _context.SaveChanges();
                return RedirectToAction("AdminEvents");
            }

            return View(auctionItem);
        }

        public ActionResult AddDocument(int eventId)
        {
            var document = new EventDocument { Event_Id = eventId };
            return View(document);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddDocument([Bind(Include = "Id,Event_Id,FileName")] EventDocument document, HttpPostedFileBase file)
        {
            if (ModelState.IsValid && file != null && file.ContentLength > 0)
            {
                using (var reader = new BinaryReader(file.InputStream))
                {
                    document.FileContent = reader.ReadBytes(file.ContentLength);
                    document.FileName = Path.GetFileName(file.FileName);
                }
                _context.EventDocuments.Add(document);
                _context.SaveChanges();
                return RedirectToAction("AdminEvents");
            }

            return View(document);
        }

        public ActionResult Content(int id)
        {
            var auctionItems = _context.AuctionItems.Where(ai => ai.Event_Id == id).ToList();
            var documents = _context.EventDocuments.Where(ed => ed.Event_Id == id).ToList();

            var model = new EventContentViewModel
            {
                AuctionItems = auctionItems,
                Documents = documents
            };

            return View(model);
        }

        public ActionResult GetAuctionItemImage(int id)
        {
            var item = _context.AuctionItems.Find(id);
            if (item != null && item.Image != null)
            {
                return File(item.Image, "image/jpeg");
            }
            return HttpNotFound();
        }

        public ActionResult DownloadDocument(int id)
        {
            var document = _context.EventDocuments.Find(id);
            if (document != null && document.FileContent != null)
            {
                return File(document.FileContent, "application/pdf", document.FileName);
            }
            return HttpNotFound();
        }

        public ActionResult Edit(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EventName,EventDate,EventTime,LimitOfAttendees,EventPrice,ArePetsAllowed,EventLocation")] HazelClinic3.Models.Event @event, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var eventInDb = _context.Events.Find(@event.Id);
                if (eventInDb == null)
                {
                    return HttpNotFound();
                }

                // Update event fields
                eventInDb.EventName = @event.EventName;
                eventInDb.EventDate = @event.EventDate;
                eventInDb.EventTime = @event.EventTime;
                eventInDb.LimitOfAttendees = @event.LimitOfAttendees;
                eventInDb.EventPrice = @event.EventPrice;
                eventInDb.ArePetsAllowed = @event.ArePetsAllowed;
                eventInDb.EventLocation = @event.EventLocation;

                // Update image if a new one is uploaded
                if (image != null && image.ContentLength > 0)
                {
                    using (var reader = new BinaryReader(image.InputStream))
                    {
                        eventInDb.Image = reader.ReadBytes(image.ContentLength);
                    }
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            HazelClinic3.Models.Event @event = _context.Events.Find(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                _context.SaveChanges();
            }
            return RedirectToAction("AdminEvents", "Events");
        }

        public ActionResult BookEvent(int id)
        {
            var @event = _context.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }

            var model = new EventReg
            {
                EventId = @event.Id,
                TotalCost = @event.EventPrice,
                Quantity = 1
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult BookEvent(EventReg eventReg)
        {
            if (ModelState.IsValid)
            {
                var @event = _context.Events.Find(eventReg.EventId);
                if (@event == null)
                {
                    return HttpNotFound();
                }

                eventReg.TotalCost = eventReg.Quantity * @event.EventPrice;

                _context.EventRegs.Add(eventReg);
                _context.SaveChanges();

                return RedirectToAction("CreateCheckoutSession", new { eventRegId = eventReg.Id, EventId = eventReg.EventId, TotalCost = eventReg.TotalCost, Quantity = eventReg.Quantity });
            }

            return View(eventReg);
        }


        [HttpGet]
        public async Task<ActionResult> CreateCheckoutSession(int eventRegId, int EventId, decimal TotalCost, int Quantity)
        {
            StripeConfiguration.ApiKey = "sk_test_51P1F4HKgNOMzGBDh6junKYp3kCPW3zvipfkiuPzCd2TAYjBgx72OR2OoyalAJ9lmLOSPMuVYQhCbOyWTQZO0M4tG000sJOeAP2";

            var @event = _context.Events.Find(EventId);
            if (@event == null)
            {
                return HttpNotFound();
            }

            var pricePerTicket = (long)(@event.EventPrice * 100);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
        {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = pricePerTicket,
                    Currency = "zar",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Event Ticket",
                    },
                },
                Quantity = Quantity,
            },
        },
                Mode = "payment",
                SuccessUrl = Url.Action("ThankYou", "Events", new { eventRegId = eventRegId }, Request.Url.Scheme),
                CancelUrl = Url.Action("BookEvent", "Events", new { id = EventId }, Request.Url.Scheme),
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }




        public ActionResult ThankYou(int? eventRegId)
        {
            if (eventRegId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventReg = _context.EventRegs.Find(eventRegId);
            var eventEntity = _context.Events.Find(eventReg.EventId);

            if (eventReg == null || eventEntity == null)
            {
                return HttpNotFound();
            }

            var pdfBytes = GenerateTicketPdf(eventReg, eventEntity);

            SendConfirmationEmail(eventReg, eventEntity, pdfBytes);

            Session["HasPurchasedTicket"] = true;

            return View();
        }



        private byte[] GenerateTicketPdf(EventReg eventReg, HazelClinic3.Models.Event eventEntity)
        {
            using (var stream = new MemoryStream())
            {
                var writerProperties = new WriterProperties();
                using (var writer = new PdfWriter(stream, writerProperties))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);

                        var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

                        document.Add(new Paragraph("TICKET")
                            .SetFont(font)
                            .SetFontSize(24)
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetBold()
                            .SetMarginBottom(10)
                            .SetBackgroundColor(new DeviceRgb(0, 0, 128), 0.1f)
                            .SetFontColor(DeviceRgb.WHITE));

                        document.Add(new Paragraph($"Event Name: {eventEntity.EventName}")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetMarginBottom(5));

                        document.Add(new Paragraph($"Date: {eventEntity.EventDate}")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetMarginBottom(5));

                        document.Add(new Paragraph($"Time: {eventEntity.EventTime}")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetMarginBottom(5));

                        document.Add(new Paragraph($"Venue: {eventEntity.EventLocation}")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetMarginBottom(10));

                        document.Add(new Paragraph($"Ticket Holder: {eventReg.FullName}")
                            .SetFont(font)
                            .SetFontSize(14)
                            .SetMarginBottom(5));

                        // Generate the QR code URL specific to this user's appointment
                        string qrCodeUrl = GenerateQRCodeUrl(eventReg.Email);

                        // Add the QR code image to the PDF
                        var qrImageData = ImageDataFactory.Create(qrCodeUrl);
                        var qrImage = new iText.Layout.Element.Image(qrImageData);
                        qrImage.ScaleToFit(150, 150); // Scale the QR code image
                        qrImage.SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER); // Align the image as needed
                        document.Add(qrImage);
                    }
                }

                return stream.ToArray();
            }
        }



        private string GenerateQRCodeUrl(string email)
        {
            string placidApiToken = "placid-7p5t2vnvqly9z0rm-ujyko8ux7lxfrusw";
            string qrCodeTemplateUrl = "https://api.placid.app/u/cqo8lh3asj9tg"; // Your Placid QR Code template URL

            // Create the API request URL for the QR code with the email as a parameter
            string requestUrl = $"{qrCodeTemplateUrl}?email={HttpUtility.UrlEncode(email)}&token={placidApiToken}";

            return requestUrl;
        }


        private void SendConfirmationEmail(EventReg eventReg, HazelClinic3.Models.Event eventEntity, byte[] pdfBytes)
        {
            string subject = "Your Ticket Purchase Confirmation";
            string body = $@"
              Dear {eventReg.FullName},

              Thank you for your purchase!
 
              We are pleased to confirm your ticket purchase for the upcoming event hosted by SPCA Durban. 
              Your support helps us continue our mission of protecting and caring for animals in need.

             Event Details:

             Event Name: {eventEntity.EventName}
             Date: {eventEntity.EventDate}
             Time: {eventEntity.EventTime}
             Venue: {eventEntity.EventLocation}
             Ticket Quantity: {eventReg.Quantity}
             Please find your ticket(s) attached to this email. Kindly print them out or have them available on your mobile device for entry to the event.
             Below is your QR code, attached to the pdf. Please scan this code before entering the event.

             If you have any questions or need further assistance. 
             We look forward to seeing you at the event and thank you once again for your generous support.

             Best regards,
             SPCA Durban";

            using (var message = new MailMessage("spcadurbanza@gmail.com", eventReg.Email))
            {
                message.Subject = subject;
                message.Body = body;

               
                var attachment = new Attachment(new MemoryStream(pdfBytes), "Ticket.pdf", "application/pdf");
                message.Attachments.Add(attachment);

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("spcadurbanza@gmail.com", "urpc bsvq bdmd wpda");
                    smtpClient.EnableSsl = true;
                    smtpClient.Send(message);
                }
            }
        }


    }
}
