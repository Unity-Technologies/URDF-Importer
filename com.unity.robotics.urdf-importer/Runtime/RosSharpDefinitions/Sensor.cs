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
        public string name
        { get; set; }
        public string type
        { get; set; }
        public string topic
        { get; set; }
        private static string k_baseKey = "sensor";
        private static string k_attributeDelimit = "@";
        private static string k_elementDelimit = "/";
        public int updateRate
        { get; set; }
        public Dictionary<string,string> elements
        { get; set; }

        public Sensor(XElement node)
        {
            elements = new Dictionary<string, string>();
            foreach(XAttribute attribute in node.Attributes())
            {
                AddAttribute(attribute, k_baseKey);
            }
            foreach(XElement element in node.Elements())
            {
                AddElement(element,k_baseKey);
            }
        }

        public void AddAttribute(XAttribute attribute, string key)
        {
            string currentKey = key + k_attributeDelimit + attribute.Name;
            elements.Add(currentKey, attribute.Value);
        }

        public void AddElement(XElement element, string key)
        {
            string currentKey = key + k_elementDelimit + element.Name;
            if(element.Elements().Count() == 0 && !(element.Value == ""))
            {
                elements.Add(currentKey,element.Value);
            }
            foreach(XAttribute attribute in element.Attributes())
            {
                AddAttribute(attribute, currentKey);
            }
            foreach(XElement ele in element.Elements())
            {
                AddElement(ele,currentKey);
            }
        }
    }
}