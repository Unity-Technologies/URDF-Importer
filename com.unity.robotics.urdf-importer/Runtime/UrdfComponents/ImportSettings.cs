using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Robotics.UrdfImporter
{
    public class ImportSettings
    {
        public enum axisType
        {
            yAxis,
            zAxis,
        }

        public enum convexDecomposer
        {
            unity,
            vHACD,
        }

        public axisType chosenAxis = axisType.yAxis;
        public convexDecomposer convexMethod = convexDecomposer.vHACD;

        public bool OverwriteExistingPrefabs { get; set; } = false;

        public int linksLoaded = 0;
        public int totalLinks = 0;

        static public ImportSettings DefaultSettings()
        {
            return new ImportSettings();
        }
    }
}
