using ConsolAppForPOCs.EWS;
using ConsolAppForPOCs.VisionAPI.ocr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsolAppForPOCs
{
    class Program
    {
        static void Main(string[] args)
        {
            RunEwsOAuthLogic();
        }

        private static void RunEwsOAuthLogic()
        {
            var ewsQAuth = new EwsOAuth();
            ewsQAuth.AuthenticateClientAppPermission().Wait();
            //ewsQAuth.AuthenticateClientDelegatePermission().Wait();
        }

        private static void RunExtractTestLogic()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            string imageFilePath = projectDirectory + @"\Files\Image1.jpg";

            var extractText = new ExtractPrintedText();

            extractText.StartExtraction(imageFilePath).Wait();
        }
    }
}
