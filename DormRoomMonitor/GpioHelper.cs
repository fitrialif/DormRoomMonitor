﻿using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace DormRoomMonitor
{
    /// <summary>
    /// Interface with the device's GPIO controller in order to monitor for motion and control the LED.
    /// </summary>
    public class GpioHelper
    {
        private GpioController gpioController;
        private GpioPin pirSensor;
        private GpioPin doorLockPin;

        /// <summary>
        /// Initialize the GPIO pins. Configure the PIR motion sensor and the LED.
        /// Returns true if initialization is successful and Gpio can be utilized. Returns false otherwise.
        /// </summary>
        public bool Initialize()
        {
            // Get the GpioController
            gpioController = GpioController.GetDefault();
            if (gpioController == null)
            {
                // There is no Gpio Controller on this device so return false.
                return false;
            }

            // Open the GPIO pin that interacts with the PIR sensor
            pirSensor = gpioController.OpenPin(GpioConstants.PirPin);

            if (pirSensor == null)
            {
                // Pin wasn't opened properly so return false
                return false;
            }

            // Set the direction of the PIR sensor as input
            pirSensor.SetDriveMode(GpioPinDriveMode.Input);

            // Opens the GPIO pin that interacts with the door lock system
            doorLockPin = gpioController.OpenPin(GpioConstants.DoorLockPinID);
            if (doorLockPin == null)
            {
                // Pin wasn't opened properly, return false
                return false;
            }
            // Sets doorlock pin drive mode to output as pin will be used to output information to lock
            doorLockPin.SetDriveMode(GpioPinDriveMode.Output);
            // Initializes pin to high voltage. This locks the door. 
            doorLockPin.Write(GpioPinValue.High);

            //Initialization was successfull, return true
            return true;
        }

        /// <summary>
        /// Returns the GpioPin for the PIR sensor. Will be used in to setup event handler when motion is detected.
        /// </summary>
        public GpioPin GetPirSensor()
        {
            return pirSensor;
        }

        /// <summary>
        /// Unlocks door for time specified in GpioConstants class
        /// </summary>
        public async void UnlockDoor()
        {
            // Unlock door
            doorLockPin.Write(GpioPinValue.Low);
            // Wait for specified length
            await Task.Delay(TimeSpan.FromSeconds(GpioConstants.DoorLockOpenDurationSeconds));
            // Lock door
            doorLockPin.Write(GpioPinValue.High);
        }

    }
}

