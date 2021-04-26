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
        public string name;
        public string type;
        public string topic;
        private static string baseKey = "sensor";
        private static string attributeDelimit = "@";
        private static string elementDelimit = "/";
        public int updateRate;
        public Dictionary<string,string> elements;

        public Sensor(XElement node)
        {
            elements = new Dictionary<string, string>();
            foreach(XAttribute attribute in node.Attributes())
                AddAttribute(attribute, baseKey);
            foreach(XElement element in node.Elements())
                AddElement(element,baseKey);
        }

        public void AddAttribute(XAttribute attribute, string key)
        {
            string currentKey = key + attributeDelimit + attribute.Name;
            elements.Add(currentKey, attribute.Value);
        }

        public void AddElement(XElement element, string key)
        {
            string currentKey = key + elementDelimit + element.Name;
            if(element.Elements().Count() == 0 && !(element.Value == ""))
                elements.Add(currentKey,element.Value);
            foreach(XAttribute attribute in element.Attributes())
                AddAttribute(attribute, currentKey);
            foreach(XElement ele in element.Elements())
                AddElement(ele,currentKey);
        }


    }

    
}