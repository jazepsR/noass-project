using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

public class sendEmail : MonoBehaviour
{
    public string myEmail;
    public string myPassword;
    public string targetEmail;
    [TextArea(2,4)]
    public string subject;
    [TextArea(5, 10)]
    public string body;
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            Task sendMailTask = SendMail();
        }
    }



    private async Task SendMail()
    {
        MailMessage mail = new MailMessage();

        mail.From = new MailAddress(myEmail);
        mail.To.Add(targetEmail);
        mail.Subject = subject;
        mail.Body = body;
        //Attachment code
        mail.Attachments.Add(new Attachment( Application.dataPath+ "/Textures/DIPLOMI/DIPLOMS_Bio_ENG.png"));
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(myEmail, myPassword) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };

        await Task.Delay(5000);
        smtpServer.Send(mail);
        Debug.Log("success");
    }   
}
