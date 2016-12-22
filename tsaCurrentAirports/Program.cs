using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace tsaCurrentAirports
{
    /*
    public class Airport
    {
        string name;
        string shortcode;
        string city;
        string state;
        string latitude;
        string longitude;
        string utc;
        string dst;
        string precheck;

        Airport(string iname, string ishortcode, string icity, string istate,string ilatitude, string ilongitude, string iutc, string idst, string iprecheck)
        {
            name = iname;
            shortcode = ishortcode;
            city = icity;
            state = istate;
        }

    }
    */
    class Program
    {
        static void Main(string[] args)
        {
            var theUri = "https://www.tsa.gov/data/apcp.xml";
            XmlDocument xmlResult = new XmlDocument();
            //List<Airport> airportList = new List<Airport>();
            
            try
            {
                HttpWebRequest webReq = WebRequest.CreateHttp(theUri);
                webReq.CookieContainer = new CookieContainer();
                webReq.Method = "GET";
                webReq.UserAgent = "Something";
                webReq.Referer = "https://www.tsa.gov";

                WebResponse thisthis = webReq.GetResponse();
                using (var streamReader = new StreamReader(thisthis.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    xmlResult.LoadXml(result);
                    //xmlResult.Save("c:\\users\\jmwin7\\Desktop\\tsaprojectresult.txt"); //working!
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("catch1 triggered: {0}", e.Message);
            }
            

            //At this point I have XML!

            //loop over that and 
            XmlNodeList xmlNodes = xmlResult.SelectNodes("/airports/airport");
            foreach (XmlNode node in xmlNodes)
            {
                string name = node["name"].InnerText;
                string shortcode = node["shortcode"].InnerText;
                string city = node["city"].InnerText;
                string state = node["state"].InnerText;
                string latitude = node["latitude"].InnerText;
                string longitude = node["longitude"].InnerText;
                string utc = node["utc"].InnerText;
                string dst = node["dst"].InnerText;
                string precheck = node["precheck"].InnerText;

                //Console.WriteLine("Name: {0} , short {1} , city {2}", name, shortcode, city);
                
                //sql insert command will go here;


            }

            Console.ReadLine();



        }
    }
}
