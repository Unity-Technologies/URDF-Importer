#if ROBOTICS_SENSORS
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.Robotics.Sensors;

namespace Unity.Robotics.UrdfImporter
{
    public static class UrdfSensorExtension
    {
        const string k_PoseKey = "sensor/pose";
        const string k_NameKey = "sensor@name";
        const string k_TypeKey = "sensor@type";
        const string k_PluginKey = "sensor/plugin@name";
        public static UrdfSensor Create(Transform parent, Sensor sensor)
        {
            GameObject sensorObject = new GameObject("unnamed");
            sensorObject.transform.SetParentAndAlign(parent);
            UrdfSensor urdfSensor = sensorObject.AddComponent<UrdfSensor>();
            urdfSensor.sensorType = sensor.elements[k_TypeKey];
            ImportSensorData(sensorObject.transform, sensor);
            return urdfSensor;
        }

        static void ImportSensorData(Transform sensorObject, Sensor sensor)
        {
            if (sensor.elements.ContainsKey(k_PoseKey))
            {
                string originString = sensor.elements[k_PoseKey];
                string[] originPose = originString.Split(' ');
                double[] xyz = new[] { Convert.ToDouble(originPose[0]), Convert.ToDouble(originPose[1]), Convert.ToDouble(originPose[2]) };
                double[] rpy = new[] { Convert.ToDouble(originPose[3]), Convert.ToDouble(originPose[4]), Convert.ToDouble(originPose[5]) };
                Origin origin = new Origin(xyz, rpy);
                UrdfOrigin.ImportOriginData(sensorObject.transform, origin);
            }

            sensorObject.name = sensor.elements[k_NameKey];
            GameObject sensorGameObject;
            Dictionary<string, string> unusedSettings;
            if (sensor.elements.ContainsKey(k_PluginKey) && String.IsNullOrEmpty(sensor.elements[k_PluginKey]))
            {
                sensorGameObject = SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements[k_PluginKey],sensor.elements, out unusedSettings);
            }
            else
            {
                sensorGameObject = SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements, out unusedSettings);
            }

            sensorObject.GetComponent<UrdfSensor>().unusedSettings = unusedSettings;
            sensorGameObject.transform.SetParentAndAlign(sensorObject);
        }

        public static Sensor ExportSensorData(this UrdfSensor urdfSensor)
        {
            var sensorProperties = SensorFactory.GetSensorSettingsAsDictionary(urdfSensor.gameObject);
            foreach (var unusedSettings in urdfSensor.unusedSettings.Where(unusedSettings => unusedSettings.Key != k_NameKey && unusedSettings.Key != k_PoseKey && unusedSettings.Key != k_TypeKey))
                sensorProperties.Add(unusedSettings.Key,unusedSettings.Value);
            sensorProperties.Add(k_NameKey,urdfSensor.name);
            sensorProperties.Add(k_TypeKey,urdfSensor.sensorType);
            var sensorPose = UrdfOrigin.ExportOriginData(urdfSensor.transform);
            if (sensorPose != null)
            {
                string poseString = string.Join(" ", string.Join(" ", sensorPose.Xyz ?? new double[] { 0, 0, 0 }), string.Join(" ", sensorPose.Rpy ?? new double[] { 0, 0, 0 }));
                sensorProperties.Add(k_PoseKey,poseString);
            }
            return new Sensor()
            {
                elements = sensorProperties
            };
        }
    }
}
#endif
