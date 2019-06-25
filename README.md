<h1>TiltHydrometerDotNet</h1>

Read your Tilt Hydrometer / iBeacon in C# .NET!

A tilt hydrometer is this little device that acts as a bluetooth low energy beacon, ie, it acts like Apple's iBeacon doodas. So you don't have to pair to the device to read it. It just spews out data every second or so, and if you are listening, you can receive the data. 

https://kvurd.com/blog/tilt-hydrometer-ibeacon-data-format/

https://tilthydrometer.com/
 

This device has a battery, an accelerometer, and a thermometer. So, based on the angle that it floats in a liquid, using the accelerometer and gravity, it can tell you the specific gravity of the liquid, and of course the temperature. Good for brewing beer! But also good for other things, such as monitoring various process fluids in a production plant. Think CNC coolant, polishing slurry, etc. 

So, how do you read these things in Windows 7? What, you have learned that the bluetooth low energy stack is only implemented in Windows 10? You like linux, but you have a bunch of Win7 machines running all of the stuff in your plant?

Fear not. Thanks to Wouter Huysentruit, who has written a BLE stack that runs fine in Windows 7. You can download Visual Studio Community Edition 2017, and start developing C# .NET apps on Windows 7 that read iBeacon devices. Great!

https://github.com/huysentruitw/win-beacon
 

I bought the Pluggable BT Radio Dongle and the thing works right out of the box. Just plugin in the dongle, switch to the WinUSB driver, get the source code from github, open the solution, and run it. Voila! Also, you can make your BLE dongle act as an iBeacon device (transmitter) as well. 

I wrote a little wrapper that makes an object which references the win-beacon package that fires off an event that you can subscribe to from a parent class or app that looks about like so:


