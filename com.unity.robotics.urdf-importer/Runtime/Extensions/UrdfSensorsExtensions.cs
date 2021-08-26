using System;
using System.Collections.Generic;
using Unity.Robotics.Sensors;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter
{
    public static class UrdfSensorsExtensions
    {
        const string k_SensorTopic = "sensor/topic";
        public static UrdfSensors Create(Transform parent, List<Sensor> sensors = null)
        {
            GameObject sensorsObject = new GameObject("Sensors");
            sensorsObject.transform.SetParentAndAlign(parent);
            UrdfSensors urdfSensors = sensorsObject.AddComponent<UrdfSensors>();

            sensorsObject.hideFlags = HideFlags.NotEditable;
            urdfSensors.hideFlags = HideFlags.None;
            
            if (sensors != null)
            {
                foreach (Sensor sensor in sensors)
                {
                    try
                    {
                        UrdfSensorExtension.Create(urdfSensors.transform, sensor);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Failed loading `{sensor.name}`");
                    }
                }
            }


            if (parent.GetComponent<ArticulationBody>() != null)
            {
                GameObject transformSensor = AddTransformSensor(parent);
                transformSensor.transform.SetParentAndAlign(sensorsObject.transform);
            }

            return urdfSensors;
        }

        static GameObject AddTransformSensor(Transform parent)
        {
            string topicName = "/"+parent.root.name + "/" + parent.name + "/TransformStamped";
            Dictionary<string, string> settings = new Dictionary<string, string> ();//{ { k_SensorTopic, topicName } };
            return SensorFactory.InstantiateSensor("transform",settings);
        }
    }
}
