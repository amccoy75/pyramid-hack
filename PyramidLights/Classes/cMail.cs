using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;

namespace PyramidLights.Classes
{
    public class cMail
    {
        public static void SendEmail(cConfigurationHelper pConfigHelper, string pBody)
        {
            SmtpClient clientSMTP = new SmtpClient("smtp.gmail.com", 587);
            clientSMTP.EnableSsl = true;
            clientSMTP.DeliveryMethod = SmtpDeliveryMethod.Network;
            clientSMTP.UseDefaultCredentials = false;
            clientSMTP.Credentials = new NetworkCredential("ascpyramid@gmail.com", "EGYpt6!5");

            try
            {
                cConfigurationHelper configHelper = pConfigHelper;

                using (MailMessage message = new MailMessage())
                {
                    message.From = new MailAddress("ascpyramid@gmail.com");
                    message.Subject = "Twitter Bot Error";
                    message.IsBodyHtml = true;
                    message.To.Add(configHelper.eMail());

                    message.Body = "Hi," + "<br><br>" + "Welcome to Pyramid Lights!<br><br>"
                        + "There is an error running the Twitter Bot. Please fix the following error:<br><br>"
                        + pBody
                        + "<br><br>Thanks!";

                    clientSMTP.Send(message);
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                clientSMTP.Dispose();
            }
        }
    }
}
