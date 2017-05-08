using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public class WufooManager : MonoBehaviour {

    public string wufooAPIKey;
    public string subdomain;

    public class WuFooUser
    {
        public string User { get; set; }
        public string Email { get; set; }
        public string TimeZone { get; set; }
        public string Company { get; set; }
        public string IsAccountOwner { get; set; }
        public string CreateForms { get; set; }
        public string CreateReports { get; set; }
        public string CreateThemes { get; set; }
        public string AdminAccess { get; set; }
        public string Image { get; set; }
        public string ApiKey { get; set; }
        public string LinkForms { get; set; }
        public string LinkReports { get; set; }
        public string Hash { get; set; }
        public string ImageUrlBig { get; set; }
        public string ImageUrlSmall { get; set; }
        public string HttpsEnabled { get; set; }
    }

    public class WufooUserJSON
    {
        public List<WuFooUser> Users { get; set; }
    }

    // Use this for initialization
    void Start()
    {
        Authenticate();
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    public void Authenticate()
    {
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

        try
        {
            string url = ("https://" + subdomain + ".wufoo.com/api/v3/forms.json");

            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

            string authInfo = wufooAPIKey + ":footastic";
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;

            Debug.Log("Getting response..." + authInfo);
            request.GetResponse();

            Debug.Log("...but we never get this far without the exception error :(" + authInfo);

            using (var webResponse = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(webResponse.GetResponseStream()))
                {
                    var objText = reader.ReadToEnd();

                    // haven't even got to test this far :(
                    WufooUserJSON deserializedProduct = JsonUtility.FromJson<WufooUserJSON>(objText);
                    Debug.Log(deserializedProduct.Users[0]);
                }
            }
        }
        catch (WebException we)
        {
            var ex = we as Exception;

            // well we always gets here, this catches the exception :(
            while (ex != null)
            {
                Debug.Log(ex.ToString());
                ex = ex.InnerException;
            }
        }

    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        Debug.Log("This never executes, which may be an indicator it's the server denying us.");

        bool isOk = true;

        // If there are errors in the certificate chain, look at each error to determine the cause.
        if (sslPolicyErrors != SslPolicyErrors.None)
        {
            for (int i = 0; i < chain.ChainStatus.Length; i++)
            {
                if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
                {
                    chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                    chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                    bool chainIsValid = chain.Build((X509Certificate2)certificate);
                    if (!chainIsValid)
                    {
                        isOk = false;
                    }
                }
            }
        }

        return isOk;
    }

}
