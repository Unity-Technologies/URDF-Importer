using System;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Robotics.Sensors;

namespace RosSharp.Urdf
{
    public class UrdfSensorExtension
    {
        const string poseKey = "sensor/pose";
        const string nameKey = "sensor@name";
        const string typeKey = "sensor@type";
        public static UrdfSensor Create(Transform parent, Sensor sensor)
        {
            GameObject sensorObject = new GameObject("unnamed");
            sensorObject.transform.SetParentAndAlign(parent);
            UrdfSensor urdfSensor = sensorObject.AddComponent<UrdfSensor>();
            urdfSensor.sensorType = sensor.elements[typeKey];
            ImportSensorData(sensorObject.transform, sensor);
            return urdfSensor;
        }

        static async Task ImportSensorData(Transform sensorObject, Sensor sensor)
        {
            if (sensor.elements.ContainsKey(poseKey))
            {
                string originString = sensor.elements[poseKey];
                string[] originPose = originString.Split(' ');
                double[] xyz = new[] { Convert.ToDouble(originPose[0]), Convert.ToDouble(originPose[1]), Convert.ToDouble(originPose[2]) };
                double[] rpy = new[] { Convert.ToDouble(originPose[3]), Convert.ToDouble(originPose[4]), Convert.ToDouble(originPose[5]) };
                Origin origin = new Origin(xyz, rpy);
                UrdfOrigin.ImportOriginData(sensorObject.transform, origin);
            }

            sensorObject.name = sensor.elements[nameKey];
            GameObject sensorGameObject;
            if (Application.isPlaying)
            {
                sensorGameObject = await SensorFactory.InstantiateSensor(sensor.elements[typeKey], sensor.elements);
            }
            else
            {
                 Task<GameObject> loadOperation = SensorFactory.InstantiateSensor(sensor.elements[typeKey], sensor.elements);
                 sensorGameObject = loadOperation.Result;
            }

            sensorGameObject.transform.SetParentAndAlign(sensorObject);
        }
    }
}
