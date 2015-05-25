using System;
using System.Net;

namespace HardwareWrapper
{
    public class HardwareManager
    {
        public float GyroX { get { return 0; } }
        public float GyroY { get { return 0; } }
        public float GyroZ { get { return 0; } }
        public float AccelX { get { return 0; } }
        public float AccelY { get { return -1; } }
        public float AccelZ { get { return 0; } }
        public float CompX { get { return 1; } }
        public float CompY { get { return 0; } }
        public float CompZ { get { return 0; } }

        public float MotionX { get; set; }
        public float MotionY { get; set; }
        public float MotionZ { get; set; }
        public float MotionW { get; set; }

        public bool CompassEnabled { get; set; }

        public double Accuracy { get; set; }

        public bool FirstStop { get; set; }
        public bool SecondStop { get; set; }

        public HardwareManager()
        {
        }

        public void Update()
        {
        }
    }
}
