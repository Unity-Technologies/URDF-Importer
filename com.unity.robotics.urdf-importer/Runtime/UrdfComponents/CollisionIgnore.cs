using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter
{
    [System.Serializable]
    public class CollisionIgnore {
        public Transform Link1;
        public Transform Link2;

        public CollisionIgnore(Transform l1, Transform l2)
        {
            Link1 = l1;
            Link2 = l2;
        }
    }
}
