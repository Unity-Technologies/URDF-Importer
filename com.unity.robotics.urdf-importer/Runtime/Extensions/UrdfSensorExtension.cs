using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Robotics.Sensors;

namespace RosSharp.Urdf
{
    public static class UrdfSensorExtension
    {
        const string k_PoseKey = "sensor/pose";
        const string k_NameKey = "sensor@name";
        const string k_TypeKey = "sensor@type";
        const string k_pluginkey = "sensor/plugin@name";
        public static async Task<UrdfSensor> Create(Transform parent, Sensor sensor)
        {
            GameObject sensorObject = new GameObject("unnamed");
            sensorObject.transform.SetParentAndAlign(parent);
            UrdfSensor urdfSensor = sensorObject.AddComponent<UrdfSensor>();
            urdfSensor.sensorType = sensor.elements[k_TypeKey];
            await ImportSensorData(sensorObject.transform, sensor);
            return urdfSensor;
        }

        static async Task ImportSensorData(Transform sensorObject, Sensor sensor)
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
            
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                if (sensor.elements.ContainsKey(k_pluginkey) && String.IsNullOrEmpty(sensor.elements[k_pluginkey]))
                {
                    sensorGameObject = await SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements[k_pluginkey],sensor.elements);
                }
                else
                {
                    sensorGameObject = await SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements);
                }
            }
            else
            {
                Task<GameObject> loadOperation;
                if (sensor.elements.ContainsKey(k_pluginkey) && String.IsNullOrEmpty(sensor.elements[k_pluginkey]))
                {
                    loadOperation = SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements[k_pluginkey],sensor.elements);
                }
                else
                {
                    loadOperation = SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements);
                }
                loadOperation.Wait();
                sensorGameObject = loadOperation.Result;
            }
#else
            if (sensor.elements.ContainsKey(k_pluginkey) && String.IsNullOrEmpty(sensor.elements[k_pluginkey]))
            {
                sensorGameObject = await SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements[k_pluginkey],sensor.elements);
            }
            else
            {
                sensorGameObject = await SensorFactory.InstantiateSensor(sensor.elements[k_TypeKey], sensor.elements);
            }
#endif
            sensorGameObject.transform.SetParentAndAlign(sensorObject);
        }
    }
}
