using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.ComponentModel;

public class sendEmail : MonoBehaviour
{
    public string myEmail;
    public string myPassword;
    public string diplomsEN;
    public string diplomsLV;
    [TextArea(2,4)]
    public string subject;
    public static sendEmail instance;
    public void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {     
        if(Input.GetKeyDown(KeyCode.M))
            {
            SendMail("jazepsrutkis93@gmail.com", "Jazeps", "Rutkis", 23);
        }
    }
    public void sendEmailMethod()
    {
        SimpleEmailSender.emailSettings.STMPClient = "smtp.gmail.com";
        SimpleEmailSender.emailSettings.SMTPPort = 587;
        SimpleEmailSender.emailSettings.UserName = myEmail.Trim();
        SimpleEmailSender.emailSettings.UserPass = myPassword.Trim();
    }

    private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
    {
        if (e.Cancelled || e.Error != null)
        {
            print("Email not sent: " + e.Error.ToString());
        }
        else
        {
            print("Email successfully sent.");
        }
    }
    public void SendMail(string userEmail, string firstName, string lastName, int playerScore)
    {
        sendEmailMethod();
        string diplomaString = System.IO.Directory.GetParent(Application.dataPath) + "/DIPLOMI/" + (Helpers.isLatvian() ? diplomsLV : diplomsEN);
        Debug.LogError("diploma string: " + diplomaString);
        string body = GenerateBody(firstName + " " + lastName, playerScore.ToString());

        SimpleEmailSender.Send(userEmail, subject, body, diplomaString, SendCompletedCallback);

        /*
        MailMessage mail = new MailMessage();
        Debug.LogError("email_Starting");
        mail.From = new MailAddress(myEmail);
        mail.To.Add(userEmail);
        mail.Subject = subject;
        mail.Body = body;
        //Attachment code
        mail.Attachments.Add(new Attachment( diplomaString));
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new System.Net.NetworkCredential(myEmail, myPassword) as ICredentialsByHost;
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        smtpServer.Send(mail);
        Debug.Log("success");*/
    }   


    public string GenerateBody(string playerName, string playerScore)
    {
        if(Helpers.isLatvian())
        {
          return  "Sveiks!\nPaldies, ka apmeklēji Getliņi EKO \"Vides izglītības centru\".\nTavs Getliņi EKO poligonu procesu izziņas spēles rezultāts ir:\n\nSpēle: "+
                TopBarController.instance.gameNameString +"\nSpēlētājs: " + playerName +
                "\nPunktu skaits: " + playerScore +
                "\n\nPielikumā atradīsi diplomu par dalību spēlē!\n Uz tikšanos nākamreiz!";
        }
        else
        {
            return "Hi!\nThanks for visiting the Getliņi EKO \"Environmental education center\"!\nYour Getliņi EKO landfill porcesses brain game results are:\n\nGame: " 
                +TopBarController.instance.gameNameStringEN+ "\nPlayer name: " + playerName +
                           "\nScore: " + playerScore +
                           "\n\nAttached to this email you will find a diploma!\nSee you next time!";
        }
    }
}
