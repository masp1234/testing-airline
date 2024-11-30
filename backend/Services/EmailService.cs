using backend.Models;
using backend.Enums;
using MailKit.Net.Smtp;
using MimeKit;
using dotenv.net;

namespace backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _emailSender;
        private readonly string _emailPassword;

        public EmailService()
        {
            _emailSender = Environment.GetEnvironmentVariable("SENDER_EMAIL") ?? throw new Exception("SENDER_EMAIL environment variable not set.");
            _emailPassword = Environment.GetEnvironmentVariable("SENDER_PASSWORD") ?? throw new Exception("SENDER_PASSWORD environment variable not set.");
        }

        public async Task SendFlightEmailAsync(List<Passenger> passengers, FlightStatus status)
        {
            if (passengers == null || passengers.Count == 0)
            {
                throw new Exception("No passengers to send email to");
            }

            // Define the message subject and body template based on status
            string subject = "Icarus Airlines - Flight Status Update";
            string bodyTemplate = status switch
            {
                FlightStatus.Cancelled => 
                    "<h1>Flight Status Update</h1><p>Your flight has been <b>cancelled</b>.<br>You will receive a refund and need to reschedule if you want to travel.</p>",
                FlightStatus.Changed => 
                    "<h1>Flight Status Update</h1><p>Your flight schedule has been <b>changed</b>. Please review the updated details.</p>",
                _ => throw new Exception("Invalid flight status")
            };

            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSender, _emailPassword);

                foreach (var passenger in passengers)
                {
                    if (string.IsNullOrWhiteSpace(passenger.Email))
                    {
                        Console.WriteLine($"Skipping invalid email for passenger ID: {passenger.Id}");
                        continue;
                    }

                    var message = CreateEmailMessage(passenger.Email, subject, bodyTemplate);
                    await client.SendAsync(message);
                    Console.WriteLine($"Email sent to: {passenger.Email}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending emails: {ex.Message}");
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }

        private MimeMessage CreateEmailMessage(string recipientEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Flight Booking System", _emailSender));
            message.To.Add(new MailboxAddress("", recipientEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
            message.Body = bodyBuilder.ToMessageBody();

            return message;
        }
    }
}
