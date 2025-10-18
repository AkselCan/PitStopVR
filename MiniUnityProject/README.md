# Mini VR Project (Unity 2022 LTS, XRI + OpenXR)

## Overview
Small interactive VR scene demonstrating locomotion (teleport + smooth move) and object interaction (grab + sockets).

## Setup
- Unity **2022.3 LTS**
- Packages: **XR Interaction Toolkit**, **OpenXR**, **Input System**
- Open `Scenes/Main.unity`
- Controls (XR Device Simulator): HMD selected → WASD to move, Right Mouse to look; Tab to cycle to Right Controller; Left Click to teleport/ grab.

## Build Target
- (PC Standalone / Android for Quest), OpenXR enabled)

## Video (2–3 min, narrat

https://github.com/user-attachments/assets/3a70ebae-bcb7-4889-97d5-7be5f23b6f4b

## Features
- Locomotion: Teleportation & Smooth Move
- Interactions: Grab 3 different objects; 3 sockets with precise snap poses
- Scene: 20+ meshes with PBR maps; 5 realtime lights (only one directional)

## Known Issues
- Due to the teleportation area being underneath the the plane, locomotion doesn't feel incredibly accurate.
- The socket's attachment point is a little high, making the cone seem floating on the top shelf.

---
This project was for Dr. Demirel's Virtualy Reality (CS5970) class. 
