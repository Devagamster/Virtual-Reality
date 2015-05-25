using Microsoft.Devices;
using Microsoft.Devices.Sensors;
using System;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace HardwareWrapper
{
    public class HardwareManager
    {
        Gyroscope gyro;
        Accelerometer accel;
        Compass comp;
        Motion motion;

        Action dataAvailableCallback;

        public float GyroX { get; set; }
        public float GyroY { get; set; }
        public float GyroZ { get; set; }
        public float AccelX { get; set; }
        public float AccelY { get; set; }
        public float AccelZ { get; set; }
        public float CompX { get; set; }
        public float CompY { get; set; }
        public float CompZ { get; set; }

        public bool FirstStop { get; set; }
        public bool SecondStop { get; set; }

        public HardwareManager(Action dataAvailable)
        {
            var sampleTimeSpan = TimeSpan.FromSeconds(1f / 60f);

            gyro = new Gyroscope {TimeBetweenUpdates = sampleTimeSpan};
            gyro.CurrentValueChanged += gyro_CurrentValueChanged;
            gyro.Start();

            accel = new Accelerometer {TimeBetweenUpdates = sampleTimeSpan};
            accel.CurrentValueChanged += accel_CurrentValueChanged;
            accel.Start();

            comp = new Compass {TimeBetweenUpdates = sampleTimeSpan};
            comp.CurrentValueChanged += comp_CurrentValueChanged;
            comp.Start();

            CameraButtons.ShutterKeyHalfPressed += CameraButtons_ShutterKeyHalfPressed;
            CameraButtons.ShutterKeyPressed += CameraButtons_ShutterKeyPressed;
            CameraButtons.ShutterKeyReleased += CameraButtons_ShutterKeyReleased;

            dataAvailableCallback = dataAvailable;
        }

        void CameraButtons_ShutterKeyHalfPressed(object sender, EventArgs e)
        {
            FirstStop = true;
        }

        void CameraButtons_ShutterKeyPressed(object sender, EventArgs e)
        {
            SecondStop = true;
        }

        void CameraButtons_ShutterKeyReleased(object sender, EventArgs e)
        {
            FirstStop = false;
            SecondStop = false;
        }

        void gyro_CurrentValueChanged(object sender, SensorReadingEventArgs<GyroscopeReading> e)
        {
            GyroX = e.SensorReading.RotationRate.X;
            GyroY = e.SensorReading.RotationRate.Y;
            GyroZ = e.SensorReading.RotationRate.Z;

            dataAvailableCallback();
        }

        void accel_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            AccelX = e.SensorReading.Acceleration.X;
            AccelY = e.SensorReading.Acceleration.Y;
            AccelZ = e.SensorReading.Acceleration.Z;
        }

        void comp_CurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> e)
        {
            CompX = e.SensorReading.MagnetometerReading.X;
            CompY = e.SensorReading.MagnetometerReading.Y;
            CompZ = e.SensorReading.MagnetometerReading.Z;
        }
    }
}
