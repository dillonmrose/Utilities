using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Tools
{
    class Program
    {    
        
        
        static void Filter(string inputFile, string outputFile)
        {
            HashSet<string> queryIds = new HashSet<string>();

            Dictionary<string, string> inputFileMap = new Dictionary<string, string>();
            StreamReader inputReader = new StreamReader(inputFile);
            string inputLine = inputReader.ReadLine();
            Dictionary<string, int> inputColIndexMap = Utils.getColIndexMap(inputLine);
            for (inputLine = inputReader.ReadLine(); inputLine != null; inputLine = inputReader.ReadLine())
            {
                string[] inputLineSplit = inputLine.Split('\t');
                inputFileMap[inputLineSplit[inputColIndexMap["QueryID"]]] = inputLine;
                queryIds.Add(inputLineSplit[inputColIndexMap["QueryID"]]);
            }


            StreamWriter writer = new StreamWriter(outputFile);
            writer.WriteLine("QueryID\tLatitude\tLongitude\tRawQuery");
            foreach (string queryID in queryIds)
            {
                string[] inputLineSplit = inputFileMap[queryID].Split('\t');
                if (inputLineSplit[inputColIndexMap["IsLocal"]] == "1")
                {
                    writer.WriteLine(inputLineSplit[inputColIndexMap["QueryID"]] + "\t" + inputLineSplit[inputColIndexMap["Latitude"]] + "\t" + inputLineSplit[inputColIndexMap["Longitude"]] + "\t" + inputLineSplit[inputColIndexMap["RawQuery"]]);
                }
            }
            writer.Close();
        }
        
 
        public static void convertFileAsUtf8(string inputFile, string outputFile)
        {
            Encoding encoding = Encoding.Default;
            string original = string.Empty;

            using (StreamReader sr = new StreamReader(inputFile, Encoding.Default))
            {
                original = sr.ReadToEnd();
                encoding = sr.CurrentEncoding;
                sr.Close();
            }

            byte[] encBytes = encoding.GetBytes(original);
            byte[] utf8Bytes = Encoding.Convert(encoding, Encoding.UTF8, encBytes);
            string output = Encoding.UTF8.GetString(utf8Bytes);
            StreamWriter writer = new StreamWriter(outputFile);
            writer.WriteLine(output);
            writer.Close();
        }

        static void Main(string[] args)
        {
            //Task task = MDPStreams.query(args);
            ///task.Wait();
            StreamReader inputReader = new StreamReader(@"D:\Contest\4.txt");
            StreamWriter outputWriter = new StreamWriter(@"D:\Contest\4.solution.txt");

            

            outputWriter.Close();
            inputReader.Close();

            Console.WriteLine("Done");
            Console.ReadLine();
        }

        
        /*
        static string ReplaceOrdinalNumberWithCardinal(string originalDate)
        {
            Regex dayNumberRegex = new Regex(@"(\d)(st|nd|rd|th)");
            return dayNumberRegex.Replace(originalDate, "$1");
        }
        */
    }
}
