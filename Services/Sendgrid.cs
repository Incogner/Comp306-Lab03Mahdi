using Lab03Mahdi.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab03Mahdi.Services
{
    public class Sendgrid : IEmailSenderExtended
    {
        private readonly IConfiguration _configuration;

        public Sendgrid(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<SendGrid.Response> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mmoradi5@my.centennialcollege.ca", "MyFi");
            List<EmailAddress> tos = new List<EmailAddress>
              {
                  new EmailAddress(email, "MyFi User")
              };

            var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlMessage, displayRecipients);
            var response = await client.SendEmailAsync(msg);
            return response;
        }

        public async Task<SendGrid.Response> SendEmailAsync(AppUser user, string subject, string htmlMessage)
        {
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("mmoradi5@my.centennialcollege.ca", "MyFi");
            List<EmailAddress> tos = new List<EmailAddress>
              {
                  new EmailAddress(user.Email, user.UserName)
              };

            var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlMessage, displayRecipients);
            var response = await client.SendEmailAsync(msg);
            return response;
        }

        async Task IEmailSender.SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _configuration.GetSection("SENDGRID_API_KEY").Value;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("no-reply@myfi.com", "MyFi");
            List<EmailAddress> tos = new List<EmailAddress>
              {
                  new EmailAddress(email, "MyFi User")
              };

            var displayRecipients = false; // set this to true if you want recipients to see each others mail id 
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlMessage, displayRecipients);
            var response = await client.SendEmailAsync(msg);
        }
    }

    public interface IEmailSenderExtended : IEmailSender
    {
        //
        // Summary:
        //     This interface is the extended version of IEmailSender to include extra methods for Sending Emails
        //     
        Task<SendGrid.Response> SendEmailAsync(AppUser user, string subject, string htmlMessage);
        new Task<SendGrid.Response> SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
