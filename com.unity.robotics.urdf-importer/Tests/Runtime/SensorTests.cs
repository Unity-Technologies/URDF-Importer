using System.Collections;
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
    [Test]
    public void TotalDataTest()
    {
        int totalData = 13;
        XDocument xdoc = XDocument.Load("Assets/test.txt");
        Sensor sensor = new Sensor(xdoc.Element("sensor"));
        Assert.AreEqual(totalData,sensor.elements.Count);
    }

    [Test]
    public void NumberofAttributeTest()
    {
        int totalAttributes = 5;
        int numberofAttributes = 0;
        XDocument xdoc = XDocument.Load("Assets/test.txt");
        Sensor sensor = new Sensor(xdoc.Element("sensor"));
        foreach(var key in sensor.elements.Keys)
            if(key.Contains("@"))
                numberofAttributes++;
        Assert.AreEqual(totalAttributes , numberofAttributes);
    }

    [Test]
    public void NumberofElementsTest()
    {
        int totalElements = 8;
        int numberofElements = 0;
        XDocument xdoc = XDocument.Load("Assets/test.txt");
        Sensor sensor = new Sensor(xdoc.Element("sensor"));
        foreach(var key in sensor.elements.Keys)
            if(!(key.Contains("@")))
                numberofElements++;
        Assert.AreEqual(totalElements , numberofElements);
    }

    [Test]
    public void NoEmptyValue()
    {
        int emptyValue = 0;
        XDocument xdoc = XDocument.Load("Assets/test.txt");
        Sensor sensor = new Sensor(xdoc.Element("sensor"));
        foreach(var value in sensor.elements.Values)
            if(value == "" || value == null)
                emptyValue++;
        Assert.AreEqual(emptyValue , 0);       
    }
}
