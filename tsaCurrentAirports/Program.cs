using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace tsaCurrentAirports
{

    public class Airport
    {
        public string name;
        public string shortcode;
        public string city;
        public string state;
        public string latitude;
        public string longitude;
        public int utc;
        public int dst;
        public int precheck;

        public Airport(string name, string shortcode, string city, string state, string latitude, string longitude, int utc, int dst, int precheck)
        {
            this.name = name;
            this.shortcode = shortcode;
            this.city = city;
            this.state = state;
            this.latitude = latitude;
            this.longitude = longitude;
            this.utc = utc;
            this.dst = dst;
            this.precheck = precheck;
        }
    }
    
    class databaseOps
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["production"].ConnectionString;

        internal static bool dataAirportExists(string ishortcode)
        {
            SqlConnection sqlConnection1 = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = string.Format("SELECT shortcode FROM airports WHERE shortcode = '{0}'", ishortcode);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sqlConnection1;

            Console.WriteLine(cmd.CommandText);

            sqlConnection1.Open();
            cmd.ExecuteNonQuery();
            bool foundAirport = cmd.ExecuteReader().HasRows;
            sqlConnection1.Close();
            return foundAirport;
        }

        internal static void insertAirport(Airport iAirport)
        {
            SqlConnection sqlConnection2 = new SqlConnection(connectionString);
            SqlCommand cmd2 = new SqlCommand();
            cmd2.CommandText = String.Format("INSERT INTO airports (name, shortcode, city, state, latitude, longitude, utc, dst, precheck) values ('{0}','{1}','{2}','{3}',{4},{5},{6},{7},{8})", iAirport.name, iAirport.shortcode, iAirport.city, iAirport.state, iAirport.latitude, iAirport.longitude, iAirport.utc, iAirport.dst, iAirport.precheck);
            cmd2.CommandType = CommandType.Text;
            cmd2.Connection = sqlConnection2;

            Console.WriteLine(cmd2.CommandText);

            sqlConnection2.Open();
            cmd2.ExecuteNonQuery();
            sqlConnection2.Close();
        }

        internal static void updateAirport(Airport iAirport)
        {
            SqlConnection sqlConnection3 = new SqlConnection(connectionString);
            SqlCommand cmd3 = new SqlCommand();
            cmd3.CommandText = String.Format("UPDATE airports SET name = '{0}', shortcode = '{1}', city = '{2}', state = '{3}', latitude = {4}, longitude = {5}, utc = {6}, dst = {7}, precheck = {8} where shortcode = '{1}'", iAirport.name, iAirport.shortcode, iAirport.city, iAirport.state, iAirport.latitude, iAirport.longitude, iAirport.utc, iAirport.dst, iAirport.precheck);
            cmd3.CommandType = CommandType.Text;
            cmd3.Connection = sqlConnection3;

            Console.WriteLine(cmd3.CommandText);

            sqlConnection3.Open();
            cmd3.ExecuteNonQuery();
            sqlConnection3.Close();
        } 
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            var theUri = "https://www.tsa.gov/data/apcp.xml";
            XmlDocument xmlResult = new XmlDocument();
            var listAirports = new List<Airport>();

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

            //loop over that XML object
            XmlNodeList xmlNodes = xmlResult.SelectNodes("/airports/airport");
            foreach (XmlNode node in xmlNodes)
            {
                Airport thisAirport = new Airport(
                    name: node["name"].InnerText.Replace("'", "''"),
                    shortcode: node["shortcode"].InnerText,
                    city: node["city"].InnerText,
                    state: node["state"].InnerText,
                    latitude: node["latitude"].InnerText,
                    longitude: node["longitude"].InnerText,
                    utc: (node["utc"].InnerText.ToLower() == "true") ? 1 : 0,
                    dst: (node["dst"].InnerText.ToLower() == "true") ? 1 : 0,
                    precheck: (node["precheck"].InnerText.ToLower() == "true") ? 1 : 0
                );
                listAirports.Add(thisAirport);
            }

            foreach (var ap in listAirports)
            {
                Console.WriteLine("AP value: {0}", ap.shortcode);
                Console.WriteLine(databaseOps.dataAirportExists(ap.shortcode));

                
                if (databaseOps.dataAirportExists(ap.shortcode) == false)
                {
                    databaseOps.insertAirport(ap);
                }
                else
                {
                    databaseOps.updateAirport(ap);
                }
                Console.WriteLine("");
                Console.WriteLine("");

                
            }



        }
    }
}
