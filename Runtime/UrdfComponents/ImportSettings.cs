using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RosSharp
{
    public class ImportSettings
    {
        public enum axisType
        {
            zAxis,
            yAxis,
        }

        public enum convexDecomposer
        {
            unity,
            vHACD,
        }

        public axisType choosenAxis = axisType.yAxis;
        public convexDecomposer convexMethod = convexDecomposer.vHACD;

        static public ImportSettings DefaultSettings()
        {
            return new ImportSettings();
        }
    }
}
