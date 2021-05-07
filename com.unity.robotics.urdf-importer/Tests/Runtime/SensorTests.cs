using System.Collections;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RosSharp.Urdf;
using System.Xml;
using System.Xml.Linq;


namespace  RosSharp.Urdf.Sensors.Test
{
    public class SensorTests
    {
        // A Test behaves as an ordinary method
        private TextReader GetSample()
        {
            string sampleDoc = "<sensor name='my_camera' type='camera'>\n" +
                                "  <camera>\n" +
                                "    <save enabled=\"true\">\n" +
                                "      <path>/tmp/camera_save_tutorial</path>\n" +
                                "    </save>\n" +
                                "    <horizontal_fov>1.047</horizontal_fov>\n" +
                                "    <image>\n" +
                                "      <width>1920</width>\n" +
                                "      <height>1080</height>\n" +
                                "    </image>\n" +
                                "    <clip>\n" +
                                "      <near>0.1</near>\n" +
                                "      <far>100</far>\n" +
                                "    </clip>\n" +
                                "  </camera>\n" +
                                "  <always_on>1</always_on>\n" +
                                "  <update_rate>30</update_rate>\n" +
                                "  <plugin name='test_plugin' filename='test_filename'>\n" +
                                "  </plugin>\n" +
                                "</sensor>\n";
            return new StringReader(sampleDoc);
        }
        [Test]
        public void TotalDataTest()
        {
            
            int totalData = 13;
            XDocument xdoc = XDocument.Load(GetSample());
            Sensor sensor = new Sensor(xdoc.Element("sensor"));
            Assert.AreEqual(totalData,sensor.elements.Count);
        }

        [Test]
        public void NumberofAttributeTest()
        {
            int totalAttributes = 5;
            int numberofAttributes = 0;
            XDocument xdoc = XDocument.Load(GetSample());
            Sensor sensor = new Sensor(xdoc.Element("sensor"));
            foreach (var key in sensor.elements.Keys)
            {
                if (key.Contains("@"))
                {
                    numberofAttributes++;
                }
            }
            Assert.AreEqual(totalAttributes , numberofAttributes);
        }

        [Test]
        public void NumberofElementsTest()
        {
            int totalElements = 8;
            int numberofElements = 0;
            XDocument xdoc = XDocument.Load(GetSample());
            Sensor sensor = new Sensor(xdoc.Element("sensor"));
            foreach (var key in sensor.elements.Keys)
            {
                if (!(key.Contains("@")))
                {
                    numberofElements++;
                }
            }
            Assert.AreEqual(totalElements , numberofElements);
        }

        [Test]
        public void NoEmptyValue()
        {
            int emptyValue = 0;
            XDocument xdoc = XDocument.Load(GetSample());
            Sensor sensor = new Sensor(xdoc.Element("sensor"));
            foreach (var value in sensor.elements.Values)
            {
                if (value == "" || value == null)
                {
                    emptyValue++;
                }
            }
            Assert.AreEqual(emptyValue , 0);       
        }

        [Test]
        public void CorrectAttributeValues()
        {
            XDocument xdoc = XDocument.Load(GetSample());
            XElement sensor = xdoc.Element("sensor");
            Sensor testSensor = new Sensor(sensor);
            Assert.AreEqual(testSensor.elements["sensor@name"],"my_camera");
            Assert.AreEqual(testSensor.elements["sensor@type"],"camera");
            Assert.AreEqual(testSensor.elements["sensor/camera/save@enabled"],"true");
            Assert.AreEqual(testSensor.elements["sensor/camera/save/path"],"/tmp/camera_save_tutorial");
            Assert.AreEqual(testSensor.elements["sensor/camera/horizontal_fov"],"1.047");
            Assert.AreEqual(testSensor.elements["sensor/camera/image/width"],"1920");
            Assert.AreEqual(testSensor.elements["sensor/camera/image/height"],"1080");
            Assert.AreEqual(testSensor.elements["sensor/camera/clip/near"],"0.1");
            Assert.AreEqual(testSensor.elements["sensor/camera/clip/far"],"100");
            Assert.AreEqual(testSensor.elements["sensor/always_on"],"1");
            Assert.AreEqual(testSensor.elements["sensor/update_rate"],"30");
            Assert.AreEqual(testSensor.elements["sensor/plugin@name"],"test_plugin");
            Assert.AreEqual(testSensor.elements["sensor/plugin@filename"],"test_filename");

        }
    }
}
