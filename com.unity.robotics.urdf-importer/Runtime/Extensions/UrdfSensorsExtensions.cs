using System;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp.Urdf
{
    public static class UrdfSensorsExtensions
    {
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
                    UrdfSensorExtension.Create(urdfSensors.transform, sensor);
                }
            }

            return urdfSensors;
        }
    }
}
