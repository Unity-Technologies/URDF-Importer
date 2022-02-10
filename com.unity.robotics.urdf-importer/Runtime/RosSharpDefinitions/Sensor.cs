using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System;
using System.Xml;

namespace Unity.Robotics.UrdfImporter
{
    public class Sensor
    {
        const string k_BaseKey = "sensor";
        const char k_AttributeDelimit = '@';
        const char k_ElementDelimit = '/';
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

        public Sensor()
        {
        }
 
        public void AddAttribute(XAttribute attribute, string key)
        {
            string currentKey = key + k_AttributeDelimit + attribute.Name;
            elements.Add(currentKey, attribute.Value);
        }

        public void AddElement(XElement element, string key)
        {
            string currentKey = key + k_ElementDelimit + element.Name;
            if (!element.Elements().Any() && element.Value != "")
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

        public void WriteToUrdf(XmlWriter writer)
        {
            ExportElement("sensor",writer);
        }

        void ExportElement(string elementKey, XmlWriter writer)
        {
            char[] delimiterChars = { k_AttributeDelimit, k_ElementDelimit };
            string elementName = elementKey.Split(delimiterChars).Last();
            writer.WriteStartElement(elementName);
            var attributeList = elements.Select(x => x).Where(x => x.Key.StartsWith(elementKey + k_AttributeDelimit)).ToList();
            foreach(var attribute in attributeList)
                writer.WriteAttributeString(attribute.Key.Substring((elementKey + k_AttributeDelimit).Length),attribute.Value);
            if(elements.Keys.Contains(elementKey))
                writer.WriteString(elements[elementKey]);
            var elementList = elements.Select(x => x).Where(x => x.Key.StartsWith(elementKey + k_ElementDelimit)).ToList();
            foreach(var element in elementList)
                ExportElement(element.Key,writer);
            writer.WriteEndElement();
        }
        
    }
}
