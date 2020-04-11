using System;
using System.IO;

namespace BlockSites
{
    class Program
    {

#if DEBUG
        const string hostsFilePath = "hosts";
#else
        const string hostsFilePath = "drivers\\etc\\hosts";
#endif        
        
        const string startToken = "# start blocked sites";
        const string endToken = "# end blocked sites";
        const string sites = "sites.txt";

        const string blockArg = "-block";
        const string unblockArg = "-unblock";
        

        static void Main(string[] args)
        {
            
            if(args.Length == 0)
            {
                Console.WriteLine("Arguments is empty...");
                return;
            }

            if(args.Length > 1)
            {
                Console.WriteLine("There are too much arguments...");
                return;
            }

            var arg = args[0];

#if DEBUG
            var hostsFile = hostsFilePath;
#else
            var hostsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System),hostsFilePath);
#endif

            Console.WriteLine(hostsFile);     
            var content = string.Empty;

            using(var file = File.OpenText(hostsFile))
            {
                content =  file.ReadToEnd();
            }

            if(string.IsNullOrEmpty(content))
            {
                Console.WriteLine("The Content is empty");
                return;
            }

            switch(arg)
            {
                case blockArg:
                    ApplyBlokedSites(hostsFile, content);
                    break;
                case unblockArg:
                    UnblockSites(hostsFile, content);
                    break;
            }

            Console.WriteLine("Done...");
        }

        private static void ApplyBlokedSites(string filename, string content)
        {
            if(content.IndexOf(startToken) > -1)
            {
                Console.WriteLine("Sites already have been blocked...");
                return;
            }

           var blokedSitesContent = File.ReadAllText(sites);
            if(!string.IsNullOrEmpty(blokedSitesContent))
            {
                content += (content[content.Length - 1] == '\n' ? Environment.NewLine : "") + startToken + Environment.NewLine + blokedSitesContent + Environment.NewLine + endToken;
                File.WriteAllText(filename,content);
            } 
        }

        private static void UnblockSites(string filename, string content)
        {
            var startIndex = -1;
            if((startIndex = content.IndexOf(startToken)) == -1)
            {
                Console.WriteLine("Sites already have been unblocked...");
                return;
            }
            var endIndex = content.IndexOf(endToken) + endToken.Length;
            var newContent = content.Substring(0, startIndex) + content.Substring(endIndex,content.Length - endIndex);
            WriteContent(filename, newContent);
        }

        private static void WriteContent(string filename, string content)
        {
            File.WriteAllText(filename, content);
        }

    }
}
