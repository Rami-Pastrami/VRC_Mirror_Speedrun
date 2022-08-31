# VRC Mirror SpeedRun
**Give Mirror Dwellers a token of shame!**
If a user darts to a mirror upon world load, give them an achievement to see privately or have it synced over the network so that everyone can see!
[video]

## Installation
Just open the Unity Package.

## Setup
(A Demo Scene is Included. This can be safely deleted if you don't want it)
Drag the prefab into the scene, and position the MirrorSpeedRun GameObject (Do not move the AchivementBanner child) such that it is at the lower right corner of the mirror like so:
[image]
In the Inspector, you should first select your mirror from the Hierarchy and drag it here.
[inspector]
Adjust the sync settings to your personal liking, some users may find that a sound playing every time a user darts to the mirror be too annoying. Set the max number of seconds the script should wait for a user for them to set this off.

The system will attempt to resize everything based on your mirror transform properties. However, you can tweak the Mirror Adjustment vector if needed. You can see the real player trigger (Dark Green box) and the Particle Emitter (cyan box) if you enable the Gizmos view in Unity
