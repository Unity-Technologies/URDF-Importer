using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
#if UNITY_EDITOR
                    if (Application.isPlaying)
                    {
                        UrdfSensorExtension.Create(urdfSensors.transform, sensor);
                    }
                    else
                    {
                        Task<UrdfSensor> sensorLoadOperation = UrdfSensorExtension.Create(urdfSensors.transform, sensor);
                        sensorLoadOperation.Wait();
                    }
#else
                    UrdfSensorExtension.Create(urdfSensors.transform, sensor);
#endif
                }
            }

            return urdfSensors;
        }
    }
}
