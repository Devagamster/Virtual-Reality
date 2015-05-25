# Virtual-Reality
A VR program for the Windows Phone based on [google cardboard](https://www.google.com/get/cardboard/) and Oculus Rift. I wrote this when I saw the Cardboard project by google and decided I wanted that on my phone. Unfortunately I had a windows phone, so I wrote it myself.

## Interesting Pieces

### Motion Algorithm

I developed a simple 3d renderer and integration system with the motion code so that the camera would always face approximately in the direction of the phone's orientation. This required transforming the output of the [madgwick motion algorithm](http://www.x-io.co.uk/open-source-imu-and-ahrs-algorithms/) into matrices pointing in the right direction.

### Renderer and Setup

I built cardboard holder for my phone and two lenses which were held at the proper distance from the screen. I then adapted a [simple pixel shader](https://github.com/Devagamster/Virtual-Reality/blob/master/VirtualReality/Content/OculusRift.fx) to warp the end result so that things would display nicely through the lenses without giving me a headache.
