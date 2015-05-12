using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace atsreader
{
    public class SKUData
    {
        public string TSize;
        public int Qty;
        public SKUData(string TSize, int Qty)
        {
            this.TSize = TSize;
            this.Qty = Qty;
        }
    }
    public class ATSData
    {
        public string ANum;
        public string RCat;
        public string Plant;
        public string FName;
        public List<SKUData> Stock;
        public ATSData(string ANum, string RCat, string Plant, List<SKUData> Stock, string FName)
        {
            this.ANum = ANum;
            this.RCat = RCat;
            this.Plant = Plant;
            this.Stock = Stock;
            this.FName = FName;
        }
        public static ATSData GetATSDataFrom(string FullFileName)
        {
            XElement ATSFile;
            using (System.IO.StreamReader reader = new System.IO.StreamReader(FullFileName))
            {
                ATSFile = XElement.Load(reader);
            }
            /*
            List<ATSData> tmpData = new List<ATSData>(from root in ATSFile.Descendants("Inventory")
                                                      select new ATSData(
                                                                            root.Element("Article").Attribute("articleId").Value,
                                                                            root.Element("Article").Element("StockCategory").Value,
                                                                            root.Element("Article").Element("InvLocation").Value,
                                                                            from AllSKUdata
                                                                        )
                                                     );*/
            
            //string s;
            //XElement x;
            //x = ATSFile.Element("Article");
            //x = ATSFile.Element("ns0:Inventory");
            //x = ATSFile.Element("ns0:Inventory").Element("ns0:Article");
            //x = ATSFile.Element//.Element("Inventory");
            string ns0 = "http://www.adidas.com/integration/InventoryIDM2";
            string ns1 = "http://adidasGroup.com/schema/cdm/sharedObjects/v2";
            //List<string> l1 = new List<string>(from e in ATSFile.Elements() select e.Name.LocalName);
            //List<string> l2 = new List<string>(from e in ATSFile.Elements() select e.Name.NamespaceName);
            //x = ATSFile.Element(XName.Get("Article", ns0));
            //x = ATSFile.Element(XName.Get("Article", ns0)).Element(XName.Get("StockCategory", ns1));
            //XElement x = ATSFile.Element("ns0:Inventory").Element("ns0:Article");
            //s = ATSFile.Element("Article").Attribute("articleId").Value;
            //s = ATSFile.Element(XName.Get("Article", ns0)).Element(XName.Get("StockCategory",ns1)).Value;
            //s = ATSFile.Element(XName.Get("Article", ns0)).Element(XName.Get("InvLocation", ns1)).Value;

            ATSData tmpData = new ATSData(
                                          ATSFile.Element(XName.Get("Article", ns0)).Attribute("articleId").Value,
                                          ATSFile.Element(XName.Get("Article", ns0)).Element(XName.Get("StockCategory", ns1)).Value,
                                          ATSFile.Element(XName.Get("Article", ns0)).Element(XName.Get("InvLocation", ns1)).Attribute("locationId").Value,
                                          new List<SKUData>(
                                                                from SKUElement in ATSFile.Element(XName.Get("Article", ns0)).Element(XName.Get("InvLocation", ns1)).Element(XName.Get("InvAvDate", ns1)).Elements(XName.Get("InventorySKU", ns1))
                                                                select
                                                                    new SKUData(SKUElement.Attribute("SKUId").Value,
                                                                                Convert.ToInt32(SKUElement.Element(XName.Get("Quantity", ns1)).Value))
                                                           ),
                                          FullFileName.Substring(FullFileName.LastIndexOf("\\") != 0 ? FullFileName.LastIndexOf("\\") + 1 : 0)
                                         );
            /*
            ATSData tmpData = new ATSData(
                              ATSFile.Element("ns0:Inventory").Element("ns0:Article").Attribute("articleId").Value,
                              ATSFile.Element("ns0:Inventory").Element("ns0:Article").Element("ns1:StockCategory").Value,
                              ATSFile.Element("ns0:Inventory").Element("ns0:Article").Element("ns1:InvLocation").Value,
                              new List<SKUData>(
                                                    from SKUElement in ATSFile.Element("ns0:Inventory").Element("ns0:Article").Element("ns1:InvLocation").Element("ns1:InvAvDate").Elements()
                                                    select
                                                        new SKUData(SKUElement.Element("ns1:InventorySKU").Attribute("SKUId").Value,
                                                                    Convert.ToInt32(SKUElement.Element("ns1:InventorySKU").Element("ns1:Quantity").Value))
                                               )
                             );*/
            return tmpData;
            //ATSData tmpData = new ATSData;
            //return tmpData;
        }
        public List<string> Lines
        {
            get
            {
                return new List<string>(from SKULine in this.Stock select this.ANum + ";" + this.Plant + ";" + this.RCat + ";" + SKULine.TSize + ";" + SKULine.Qty + ";" + this.FName);
            }
            set { }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string filepath = "e:\\tasks\\20150511 RSM ATS check\\data";
            string destfile = "e:\\tasks\\20150511 RSM ATS check\\data\\out.csv";
            List<ATSData> ATS = new List<ATSData>(from file in System.IO.Directory.GetFiles(filepath) select ATSData.GetATSDataFrom(file));
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(destfile, true))
            {
                foreach (ATSData rawfile in ATS)
                {
                    foreach (string line in rawfile.Lines) writer.WriteLine(line);
                }
            }
        }
    }
}
