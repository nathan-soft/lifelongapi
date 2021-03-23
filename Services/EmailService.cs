using Humanizer;
using LifeLongApi.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LifeLongApi.Services
{
    public interface IEmailService
    {
        Task SendAppointmentEmailAsync(AppUser mentor, AppUser mentee, Appointment appointmentDetail);
        Task SendAppointmentReminderEmailAsync(AppUser mentor, AppUser mentee, string message);
        Task SendMentorshipApprovalEmailAsync(AppUser mentor, AppUser mentee, string topicName);
        Task SendMentorshipRequestMailAsync(AppUser mentor, AppUser mentee, string topicName);
        Task SendTestMailAsync();
        Task SendUpdatedAppointmentEmailAsync(AppUser mentor, AppUser mentee, Appointment appointmentDetail, bool isRescheduled);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private string SMTPServer;
        private int SMTPPort;
        private string SMTPUser;
        private string SMTPPass;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            SMTPServer = _configuration.GetSection("Smtp").GetSection("Server").Value;
            SMTPPort = int.Parse(_configuration.GetSection("Smtp").GetSection("Port").Value);
            SMTPUser = _configuration.GetSection("Smtp").GetSection("Username").Value;
            SMTPPass = _configuration.GetSection("Smtp").GetSection("Password").Value;
        }

        private async Task SendMailAsync(MimeMessage email)
        {
            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(SMTPServer, SMTPPort, SecureSocketOptions.StartTls);
            smtp.Authenticate(SMTPUser, SMTPPass);
            await smtp.SendAsync(email).ConfigureAwait(false);
            await smtp.DisconnectAsync(true).ConfigureAwait(true);
        }

        public async Task SendTestMailAsync()
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Nkuzi", SMTPUser));
            email.To.Add(new MailboxAddress($"Nathan Omomowo", "firstnate0@gmail.com"));

            //CONSTRUCT THE MESSAGE BODY.
            string messageBody = "";
            messageBody += GetEmailHeader("Mail From Nkuzi App", email);
            messageBody += "This is a test mail from the Nkuzi api.";
            messageBody += GetEmailFooter("firstnate0@gmail.com");

            //var builder = new BodyBuilder
            //{
            //    TextBody = bodyPlaintext,
            //    HtmlBody = bodyHtml,
            //};
            //message.Body = builder.ToMessageBody();



            email.Body = new TextPart(TextFormat.Plain) { Text = messageBody };

            // send email
            await SendMailAsync(email);
        }

        private string GetEmailHeader(string caption, MimeMessage mail)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            caption = textInfo.ToTitleCase(caption.ToLower());
            mail.Subject = caption;

            string T = "";
            T += "--------------------------------------------------------------------------------" + Environment.NewLine;
            T += "Lifelong.com\n";
            T += caption + "\n";
            T += "Date: " + string.Format("{0:dd/MM/yyyy hh:mm:ss tt} UTC", DateTime.UtcNow) + "\n";
            T += "--------------------------------------------------------------------------------\n";
            T += "\n";
            return T;
        }

        public async Task SendMentorshipRequestMailAsync(AppUser mentor, AppUser mentee, string topicName)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Nkuzi", SMTPUser));
            email.To.Add(new MailboxAddress($"{mentor.FirstName} {mentor.LastName}", mentor.Email));

            //CONSTRUCT THE MESSAGE BODY.
            string messageBody = "";
            messageBody += GetEmailHeader($"{mentee.FirstName} {mentee.LastName} Requested for Mentorship", email);
            messageBody += $"<b>{mentee.FirstName} {mentee.LastName}</b> has requested for your Mentorship on {topicName}";
            messageBody += GetEmailFooter(mentor.Email);

            email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

            // send email
            await SendMailAsync(email).ConfigureAwait(false);
        }

        public async Task SendMentorshipApprovalEmailAsync(AppUser mentor, AppUser mentee, string topicName)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Nkuzi", SMTPUser));
            email.To.Add(new MailboxAddress($"{mentee.FirstName} {mentee.LastName}", mentee.Email));

            //CONSTRUCT THE MESSAGE BODY.
            string messageBody = "";
            messageBody += GetEmailHeader($"{mentor.FirstName} {mentor.LastName} accepted your mentorship request", email);
            messageBody += $"Your request for mentorship from <b>{mentee.FirstName} {mentee.LastName}</b>,\n";
            messageBody += $"on <b>{topicName}</b>, has been accepted!";
            messageBody += GetEmailFooter(mentor.Email);

            email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

            // send email
            await SendMailAsync(email).ConfigureAwait(false);
        }

        public async Task SendAppointmentEmailAsync(AppUser mentor, AppUser mentee, Appointment appointmentDetail)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Nkuzi", SMTPUser));
            email.To.Add(new MailboxAddress($"{mentee.FirstName} {mentee.LastName}", mentee.Email));

            //CONSTRUCT THE MESSAGE BODY.
            string messageBody = "";
            messageBody += GetEmailHeader($"New appointment with {mentor.FirstName} {mentor.LastName}", email);
            messageBody += $"You have a new appointment with <b>{mentor.FirstName} {mentor.LastName}</b> on <b>";
            messageBody += TimeZoneInfo.ConvertTimeFromUtc(appointmentDetail.DateAndTime,
                                                           TimeZoneInfo.FindSystemTimeZoneById(mentee.TimeZone)).ToOrdinalWords();
            messageBody += ".</b>\n";
            messageBody += $"Time is : <b>";
            messageBody += TimeZoneInfo.ConvertTimeFromUtc(appointmentDetail.DateAndTime,
                                                           TimeZoneInfo.FindSystemTimeZoneById(mentee.TimeZone)).ToShortTimeString();
            messageBody += "</b>";
            messageBody += GetEmailFooter(mentor.Email);

            email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

            // send email
            await SendMailAsync(email).ConfigureAwait(false);
        }

        public async Task SendAppointmentReminderEmailAsync(AppUser mentor, AppUser mentee, string message)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Nkuzi", SMTPUser));
            email.To.Add(new MailboxAddress($"{mentee.FirstName} {mentee.LastName}", mentee.Email));

            //CONSTRUCT THE MESSAGE BODY.
            string messageBody = "";
            messageBody += GetEmailHeader($"Appointment Reminder", email);
            messageBody += message;
            messageBody += GetEmailFooter(mentor.Email);

            email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

            // send email
            await SendMailAsync(email).ConfigureAwait(false);
        }

        public async Task SendUpdatedAppointmentEmailAsync(AppUser mentor, AppUser mentee, Appointment appointmentDetail, bool isRescheduled)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Nkuzi", SMTPUser));
            email.To.Add(new MailboxAddress($"{mentee.FirstName} {mentee.LastName}", mentee.Email));

            //CONSTRUCT THE MESSAGE BODY.
            string messageBody = "";
            messageBody += GetEmailHeader($"Rescheduled appointment with {mentor.FirstName} {mentor.LastName}", email);
            if (isRescheduled)
            {
                messageBody += $"Your missed appointment with <b>{mentor.FirstName} {mentor.LastName}</b> has been rescheduled to <b>";
            }
            else
            {
                messageBody += $"Your appointment with <b>{mentor.FirstName} {mentor.LastName}</b> has been moved to <b>";
            }
            messageBody += TimeZoneInfo.ConvertTimeFromUtc(appointmentDetail.DateAndTime,
                                                           TimeZoneInfo.FindSystemTimeZoneById(mentee.TimeZone)).ToOrdinalWords();
            messageBody += ".</b><br/>";
            messageBody += $"Time is : <b>";
            messageBody += TimeZoneInfo.ConvertTimeFromUtc(appointmentDetail.DateAndTime,
                                                           TimeZoneInfo.FindSystemTimeZoneById(mentee.TimeZone)).ToShortTimeString();
            messageBody += "</b>";
            messageBody += GetEmailFooter(mentor.Email);

            //because method will be sent in html format.
            messageBody.Replace("\n", "<br/>");

            email.Body = new TextPart(TextFormat.Html) { Text = messageBody };

            // send email
            await SendMailAsync(email).ConfigureAwait(false);
        }

        private string GetEmailFooter(string emailTo)
        {
            string messageBody = "\n\nIf you have any questions or need technical assitance, ";
            //messageBody += "support is available here: http://portal.smslive247.com/support/ticket.aspx\n";
            messageBody += "send a mail to support@nkuzi.com\n";
            messageBody += "\n";
            messageBody += "Or mail to:\n";
            messageBody += "Nkuzi Solutions Ltd\n";
            messageBody += "Ikeja, Lagos.\n";
            messageBody += "\n";
            messageBody += "Thank you for using Nkuzi!\n";
            messageBody += "\n";
            messageBody += "Nkuzi.com Team\n";
            messageBody += "\n";
            messageBody += "NOTE:\n";
            messageBody += "Please do not reply to this message, which was sent from an unmonitored ";
            messageBody += "e-mail address.\n";
            messageBody += "Mail sent to this address cannot be answered.\n";
            messageBody += "\n";
            messageBody += "This message was sent to " + emailTo + ".\n";
            //messageBody += "Remove yourself from future email here:\n";
            //messageBody += "http://portal.smslive247.com/account/remove.aspx?me=" + emailTo + "\n";
            messageBody += "--------------------------------------------------------------------------------\n";
            messageBody += "© Copyright " + DateTime.Today.Year + " Nkuzi.com. All Rights Reserved.\n";
            messageBody += "\n";


            return messageBody;
            //_mailMessage.Body = message;
            //_mailMessage.To.Add(emailTo);

            //return SendUserEmail(_smtpClient, _mailMessage);
        }

        private bool VerifyEmailAddress(string emailAddress)
        {
            try
            {
                MailboxAddress.Parse(emailAddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
