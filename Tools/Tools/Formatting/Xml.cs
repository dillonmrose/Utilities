using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;

namespace Tools
{
    class XML
    {
        static void ExtractXml()
        {
            string inputFile = @"D:\Data\Events\Events_RDS_Ranked_Satori.xml";
            StreamReader reader = new StreamReader(inputFile);
            StreamWriter writer = new StreamWriter(inputFile + ".extracted.txt");
            string line = "";
            for (line = reader.ReadLine(); line != null; line = reader.ReadLine())
            {
                var xdoc = XDocument.Parse(line, LoadOptions.None);
                string id = xdoc.Descendants("Event").Descendants("Id").Select(x => x.Value).FirstOrDefault();
                string name = xdoc.Descendants("Event").Descendants("Name").Select(x => x.Value).FirstOrDefault();
                string startTime = xdoc.Descendants("Event").Descendants("StartTm").Select(x => x.Value).FirstOrDefault();
                List<string> EventCategories = new List<string>();
                foreach (var EventCategory in xdoc.Descendants("Event").Descendants("EventCategories").Descendants("Category"))
                {
                    EventCategories.Add(EventCategory.Descendants("Name").Select(x => x.Value).FirstOrDefault());
                }
                string venueName = xdoc.Descendants("Event").Descendants("Venue").Descendants("Name").Select(x => x.Value).FirstOrDefault();
                string venueSatoriId = xdoc.Descendants("Event").Descendants("Venue").Descendants("SatoriId").Select(x => x.Value).FirstOrDefault();
                Console.WriteLine("id: " + id);
                Console.WriteLine("name: " + name);
                Console.WriteLine("startTime: " + startTime);
                foreach (var cat in EventCategories)
                {
                    Console.WriteLine("Category: " + cat);
                }
                Console.WriteLine("venueName: " + venueName);
                Console.WriteLine("venueSatoriId: " + venueSatoriId);
                Console.ReadLine();
            }


        }
    }
}
