using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System;
using System.Numerics;


namespace RosSharp.Urdf
{
    public class Sensor
    {
        public string name { get; set; }
        public string type { get; set; }
        public string topic { get; set; }
        private static string k_BaseKey = "sensor";
        private static string k_AttributeDelimit = "@";
        private static string k_ElementDelimit = "/";
        public int updateRate { get; set; }
        public Dictionary<string,string> elements { get; set; }

        public Sensor(XElement node)
        {
            elements = new Dictionary<string, string>();
            foreach (XAttribute attribute in node.Attributes())
            {
                AddAttribute(attribute, k_BaseKey);
            }
            foreach (XElement element in node.Elements())
            {
                AddElement(element,k_BaseKey);
            }
        }
 
        public void AddAttribute(XAttribute attribute, string key)
        {
            string currentKey = key + k_AttributeDelimit + attribute.Name;
            elements.Add(currentKey, attribute.Value);
        }

        public void AddElement(XElement element, string key)
        {
            string currentKey = key + k_ElementDelimit + element.Name;
            if (element.Elements().Count() == 0 && !(element.Value == ""))
            {
                elements.Add(currentKey,element.Value);
            }
            foreach (XAttribute attribute in element.Attributes())
            {
                AddAttribute(attribute, currentKey);
            }
            foreach (XElement ele in element.Elements())
            {
                AddElement(ele,currentKey);
            }
        }
    }
}
