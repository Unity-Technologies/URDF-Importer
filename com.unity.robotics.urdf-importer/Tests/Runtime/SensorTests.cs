using System.Collections;
using System.IO;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using RosSharp.Urdf;
using System.Xml;
using System.Xml.Linq;

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
        foreach(var key in sensor.elements.Keys)
        {
            if(key.Contains("@"))
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
        foreach(var key in sensor.elements.Keys)
        {
            if(!(key.Contains("@")))
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
        foreach(var value in sensor.elements.Values)
        {
            if(value == "" || value == null)
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
        Assert.IsNotNull(sensor);
        XAttribute sensorName = sensor.Attribute("name");
        Assert.IsNotNull(sensorName);
        Assert.AreEqual(sensorName.Value,"my_camera");
        XAttribute sensorType = sensor.Attribute("type");
        Assert.IsNotNull(sensorType);
        Assert.AreEqual(sensorType.Value,"camera");

        XElement camera = sensor.Element("camera");
        Assert.IsNotNull(camera);

        XElement save = camera.Element("save");
        Assert.IsNotNull(save);
        XAttribute enabled = save.Attribute("enabled");
        Assert.AreEqual(enabled.Value,"true");
        XElement path = save.Element("path");
        Assert.IsNotNull(path);
        Assert.AreEqual(path.Value,"/tmp/camera_save_tutorial");

        XElement horizontal_fov = camera.Element("horizontal_fov");
        Assert.IsNotNull(horizontal_fov);
        Assert.AreEqual(horizontal_fov.Value,"1.047");

        XElement image = camera.Element("image");
        Assert.IsNotNull(image);

        XElement width = image.Element("width");
        Assert.IsNotNull(width);
        Assert.AreEqual(width.Value,"1920");

        XElement height = image.Element("height");
        Assert.IsNotNull(height);
        Assert.AreEqual(height.Value,"1080"); 

        XElement clip = camera.Element("clip");
        Assert.IsNotNull(clip);

        XElement far = clip.Element("far");
        Assert.IsNotNull(far);
        Assert.AreEqual(far.Value,"100");

        XElement near = clip.Element("near");
        Assert.IsNotNull(near);
        Assert.AreEqual(near.Value, "0.1");

        XElement always_on = sensor.Element("always_on");
        Assert.IsNotNull(always_on);
        Assert.AreEqual(always_on.Value,"1");

        XElement updateRate = sensor.Element("update_rate");
        Assert.IsNotNull(updateRate);
        Assert.AreEqual(updateRate.Value,"30");

        XElement plugin = sensor.Element("plugin");
        Assert.IsNotNull(plugin);

        XAttribute pluginName = plugin.Attribute("name");
        Assert.IsNotNull(pluginName);
        Assert.AreEqual(pluginName.Value,"test_plugin");

        XAttribute pluginFileName = plugin.Attribute("filename");
        Assert.IsNotNull(pluginFileName);
        Assert.AreEqual(pluginFileName.Value,"test_filename");

    }
}
