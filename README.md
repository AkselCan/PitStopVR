# 🏎️ Pit Stop VR

A VR pit-stop training simulation that allows mechanics and Formula 1 enthusiasts to experience the high-pressure environment of race-day tire changes in an immersive, cost-effective, and safe virtual environment.

**Developed by:** Ryan Williams, Aksel Can Sözüdoğru, Tommaso Bergonzoni

---

## 🎯 Motivation

Pit stops are a decisive factor in motorsports, where even half-second delays can cost multiple positions in the final standings. Modern pit stops have been optimized from 25-30 seconds down to just a few seconds, making mistakes exponentially more costly.

**Key Problems We're Solving:**
- **High Equipment Costs**: Professional racing equipment is expensive, making regular training financially prohibitive
- **Risk of Damage**: Real-world training can damage costly race-day equipment
- **Limited Practice Opportunities**: Beginner mechanics need safe environments to build muscle memory and timing awareness
- **Pressure Training**: New mechanics need to experience high-pressure scenarios before stepping into real race conditions

**Our Solution:** Pit Stop VR provides an affordable, repeatable, and risk-free training environment that simulates the intensity of real pit stops without the associated costs and dangers.

---

## Walktrough GIF & Vid


https://github.com/user-attachments/assets/835ea8e4-1a33-4053-aa4c-2e8c2db15a5b


![VideoProject-ezgif com-video-to-gif-converter (1)](https://github.com/user-attachments/assets/f06eb3d0-1542-400b-9a0e-6aa93f41ef87)
(NOTE: This GIF was recorded at an obstructed area while playing the game, you can get right up to the car)

---

## ✨ Features

### 🏁 Immersive Environment
- Full pit lane environment designed to replicate real-world racing conditions
- Detailed car model with realistic positioning and scale
- Professional pit box setup with authentic spatial layout

### 🔧 Tire Change Workflow
- Complete tire removal and replacement mechanics
- Interactive tire gun with realistic grab haptics
- Proper tire positioning and placement requirements
- Sequential workflow matching real pit stop procedures

### ⏱️ Performance Tracking
- Automatic timer that starts when the car arrives and stops when it leaves
- Real-time feedback on pit stop duration
- Performance metrics to track improvement over time
- Benchmark comparison to professional pit stop times (fastest ever: 1.80 seconds)

### 🎮 VR Interactions
- 6 Degrees of Freedom (DOF) movement
- Haptic feedback on tool grab and interaction
- Audio cues for car arrival and pit box communication
- Natural hand tracking for tool manipulation

---


## 🛠️ Technology Stack

- **Game Engine**: Unity LTS 2022
- **Programming Language**: C#
- **VR Platform**: Meta Quest 3
- **Interaction Framework**: XR Interaction Toolkit
- **Assets**: Unity Asset Store (free prefabs and models)

### Technical Implementation
- **Movement & Timing**: Custom C# scripts using triggers and waypoint systems
- **Audio Management**: Dynamic audio sources controlled by event scripts
- **VR Interactions**: XR Interaction Toolkit for grab mechanics and haptics
- **Animation System**: Unity Animator for car and tire movement sequences

---

## 🚀 How to Run on Meta Quest 3

1. **Download Meta Quest Developer Hub**
   - Download and install [Meta Quest Developer Hub](https://developer.oculus.com/meta-quest-developer-hub/)

2. **Download the APK**
   - Download `PitStopVR.apk` from this repository

3. **Connect Your Headset**
   - Connect your Meta Quest 3 to your computer via USB-C cable
   - Put on your headset and allow USB debugging when prompted

4. **Add the Project**
   - Open Meta Quest Developer Hub
   - Add the APK file to the Developer Hub

5. **Launch the App**
   - The app will install automatically
   - Put on your headset and launch Pit Stop VR from your App Library

---

## 🎮 How to Play

1. **Start the Experience**
   - Put on your Meta Quest 3 headset
   - Launch Pit Stop VR from Unknown Sources
   - You'll begin in the Welcome Menu scene

2. **Begin Training**
   - Press the "Start" button to enter the pit lane

3. **Perform the Pit Stop**
   - Wait for the car to arrive at your pit box
   - The timer starts automatically when the car stops
   - Grab the tire gun using your controller grip button
   - Approach the tire and hold the trigger to remove it
   - Position and attach the new tire
   - The timer stops when the car leaves

4. **Track Your Progress**
   - Your pit stop time will be displayed
   - Try to beat your personal best!
   - Professional pit crews achieve times around 2.3-2.5 seconds
   - The world record is 1.80 seconds

---
## 🔄 Future Enhancements

Based on our initial proposal, we have identified several areas for future development:

### High Priority
- **Learning Mode**: Non-timed tutorial mode with step-by-step instructions
- **Repeatability**: Allow multiple tire changes within a single session without restarting
- **Advanced Metrics**: Track movement accuracy, tool efficiency, and positioning precision

### Medium Priority
- **Enhanced Realism**: Higher quality assets and more detailed environmental elements
- **Multiple Scenarios**: Different car models, weather conditions, and pit box configurations
- **Multiplayer Training**: Cooperative pit stop simulations with multiple crew members

### Low Priority
- **Career Mode**: Progressive difficulty with unlockable techniques
- **Leaderboards**: Global competition with other users
- **Replay System**: Review and analyze your pit stop performance

---

## 📝 Known Issues

- Tire change can only be performed once per session (requires restart for additional attempts)
- Limited to single-player experience
- Some visual assets may not meet AAA quality standards due to budget constraints
- No in-app tutorial or learning mode

---
