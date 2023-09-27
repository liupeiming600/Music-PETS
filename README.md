# Music-PETS
### Dependency
- **Lab Windows/CVI 2017 Full Development System**: version 17.0.0<br>
- **Ableton Live 10 Suite**: version 10.1.1<br>
- **Unity**: version 2017.4 (LTS release)<br>
# 0. Introduce
This is a rehabilitation system designed for finger exercises. It utilizes Unity to connect our self-made devices (desktop and handheld versions). The desktop version is compatible with Hololens for AR training. The handheld version can be connected to an iPad via Bluetooth for training, and it provides a specialized UI on the iPad to assist with the exercises.
This is a demo held in Nara's hospital.

https://www.youtube.com/watch?v=Pf5aNwb4C14
# 1. Setup
## 1.1 Lab Windows
### 1.1.1 Connect PETS to PC
Connect the cables of load cells to the DAQ device (**National Instruments USB-6211**) like the bottom image. If you use right hand, connect the thumb's cable to **AI1** and **AI9**.<br>
<img src=/images/lab_hardware.jpg width=250> <img src=/images/lab_connection.png width=400><br>
### 1.1.2 Start DAQ project
Start the project, double cllicking "**PETS.exe**" or Open "**PETS.prj**" in Lab Windows and click the run button.<br>
<img src=/images/lab_openproject.png width=400> <img src=/images/lab_runbutton.png width=400><br>
### 1.1.3 Check the correspondences of fingers and monitor
Confirm that it matches the fingers on the monitor by pressing the sensors in order.<br>
The thumb part in the window should react when pressed with the thumb.<br>
**If they don't react**, check the cable connection (section 1.1.1). **+5V** or **GND** cables often miss the connection.<br>
**If they don't match**, Change the cabele connections or the setting of finger correspondences (the part enclosed in red in the bottom right  image, line 62-66) in "**main.c**".<br>
<img src=/images/lab_checkfinger.png width=400> <img src=/images/lab_fingersetting.png width=400><br>
## 1.2 Ableton Live
### 1.2.1 Set Max Effects into the Ableton Live folder
Set the Max effect, "**UDPtoUnity.amxd**" under "**Ableton/User Library/Presets/Audio Effects/Max Audio Effect/~**" and set "**playviaudp.amxd**" under "**Ableton/User Library/Presets/MIDI Effects/Max MIDI Effect/~**" (In my case, I found the Ableton folder in "Users/(user name)/Music/~". I think the place of the folder changes according to the setting when you install Ableton Live). <br>
### 1.2.2 Open Ableton Live project
Open the music project (**.als file**) in Music-PETS project floder.<br>
And check if Ableton Live recognizes Max effects correctly. If it recognizes them, you can see them like the bottom images. If you see the piano roll in the bottom of the Abelton Live, press "**Shift+Tab**". you can see switching the display there.<br>
If Ableton Live is missing the Max effects, Set them into the missed effects.<br> 
<img src=/images/live_maxaudioeffect.png width=400> <img src=/images/live_maxmidieffect.png width=400><br>
<br>
___
**Postscript**: The function, "**OSC-route**", used in the effect, "**UDPtoUnity.amxd**", is the plugin function. So you hove to download it so that it works well. Go to http://cnmat.berkeley.edu/downloads and download "CNMAT Max/MSP Externals for Mac and Windows". After unzip this, move it into the folder (**Max8/Packages/**). Reboot Ableton Live. Then "OSC-route" works.
___
## 1.3 Unity
### 1.3.1 Open Unity project
Open Music-PETS folder by Unity2017. (I recommend you use Unity Hub. It can manage various versions of Unity.) This folder is the Unity project file as it is. Or import "Music-PETS.unitypackage" after make a new project.<br>
### 1.3.2 Deploy HoloLens app
**If you play Music-PETS without HoloLens (play with desktop monitor), you can skip this section.**<br>
> #### 1.3.2.1 Set Mixed Reality setting
> Click the tab, "**Mixed Reality Toolkit**" -> "**Configure**" -> "**Apply UWP Capability Settings**". And Check the check boxes other than "Internet Client".<br>
> <img src=/images/uni_uwpsetting.png width=500> <img src=/images/uni_uwpsetting2.png width=200><br>
> #### 1.3.2.2 Build HoloLens app
> Click the tab, "**File**" -> "**Build Settings...**". And Set like the bottom image.<br>
> After setting, Click **Build button**. And select build folder. If you don't have build folder or you want to remain previous builded app, make new folder.<br>
> <img src=/images/uni_holobuild.png width=500><br>
> #### 1.3.2.3 Deploy HoloLens app
> Connect HoloLens to the PC via USB cable.<br>
> Open "**HololensPETS-Unity.sln**" in the build folder. And set the deploy setting like the bottom image.<br>
After setting, click the run button. If you are asked the PIN code of HoloLens, input it (you can check PIN code in HoloLens,  "**Setting**" -> "**Update&Security**" -> "**For developer**" -> "**Pair**").<br>
> <img src=/images/uni_holodep.png width=700><br>
> If the app starts well, stop the run. the app is already listed in HoloLens applications. When you want to start HoloLens app, click "**All apps**" and the app you want to start.<br>
# 2. Usage
## 2.1 Start Lab Windows app
Start the project, double cllicking "**PETS.exe**" or Open "**PETS.prj**" in Lab Windows and click the run button.<br>
And press **start button** on the window.
<br>
**If it work correctly**, The waveform reacts as shown in the image below. Confirm that it matches the fingers on the monitor by pressing the sensors in order.<br>
<img src=/images/lab_checkfinger.png width=400><br>
## 2.2 Start Unity app
Open "**PCControlPanelStartScene**" in Unity. (It's in "**Asset/PETSControlPanel/Scenes**".) Click the run button, then Unity app starts.<br>
## 2.3 Calibrate finger force data
After Unity app starts, calibrate base forces and max forces of fingers.<br>
First, click PETS calibration button. **When it calibrate, don't press sensors mounted on PETS**.<br>
Second, click each finger calibration button. **When it calibrate, press the sensor as hard as possible**. If you'll use only right hand or left hand, you don't need to calibrate both hands.<br>
<br>
Once you calibrate, you can reuse this data by saving and loading it. If you want to save the data, enter the name and click the save button.<br>
<img src=/images/uni_forceCalibration.png height=300><br>
## 2.4 Start HoloLens app
**If you play Music-PETS without HoloLens (play with desktop monitor), you can skip this section.**<br>
> #### 2.4.1 Check IP address
> Check the IP address of HoloLens and enter it in the textbox in the upper right of Unity window. You can check the IP address in "**Settings**" -> "**Network & Internet**" -> (connected Wi-Fi) -> "**Advanced options**" -> (scroll down) -> "**IPv4 address**".<br>
> #### 2.4.2 Start HoloLnes app
> Start the HoloLens app by click "**All apps**" and "**HololensPETS-Unity**".<br>
> <img src=/images/holo_startapp.png height=250><br>
> #### 2.4.3 Calibrate the sensor position
> When you start the app, you can see the text "**State: Runnning**" in the upper right. After long tap, you can see it change to "**State: Calibtating**". Then you calibrate the five sensor potision using euphoria image. <br>
> <img src=/images/holo_running.png height=50> <img src=/images/holo_calibration.png height=50><br>
> The xyz axis can be seen by placing the Euphoria image on the sensor like the image in below. And when you tap, the potision of the sensor is registered. And you can see a green cube in registered position. you repeat this five times from thumb to pinky. After finish five calibrations, you can see five green cube on the sensors like the image in below. Then tap and Check the state is "**Running**".<br>
> <img src=/images/holo_calibrating.jpg height=200>
> <img src=/images/holo_finishcalibration.jpg height=200><br>
## 2.5 Open Ableton Live project
Open the project of the music you want to play. Then enter the IP address of Unity. Click the edit button enclosed in red in the bottom image and open the max effect, "**UDPtoUnity**". Doublr click the state enclosed in red in the bottom image and enter the IP address. (Second argument is the port. You don't need to change it.)<br>
<img src=/images/live_editmaxeffect.png height=200><br>
<img src=/images/live_ipaddress.png height=300><br>
After editting, save it by pressing "**Ctrl+s**" key. And **Finish Max software**. When Max software work, the communication of Unity and Ableton Live don't work well.<br>
## 2.6 Set Music-PETS setting
Click "**Music Play**" toggle box in Unity app. Then you can see like the image in below.<br>
The settings menu is open from the beginning. You can choose the music and set the difficulty in this window. You can open and close this menu by pressing "**s**" key.<br>
<img src=/images/uni_musicPlayWindow.png height=300><br>
First, **enter the IP address of Ableton Live**. If Unity and Ableton Live work on the same PC, enter "127.0.0.1".<br>
Second, **choose the music and track**. You have to choose the music you opened in Abelton Live in previous chapter. Basically, choose the melody track because some track have few note for playing.<br> When the connection between Unity and Ableton Live works well, you can see the message from Unity like the image in below. When you select the track, you can see the light of the selected track turn off. The  track turned off are muted so that the player play.<br>
<img src=/images/live_messagefromUnity.png height=300><br>
Third, set the difficulty. **Frequency** means the minimum interval of finger pressing. The longer this interval is, the easier the difficulty is. Set the number (**1, 2, 4, 8, 16, 32**) in it. 1/4 bar means the interval is quarter note.<br>
**Finger Select** enables select of fingers for playing. If you want to play without thumb, remove the check on thumb toggle button.<br>
After setting all, click "**OK**" button.

**If you use HoloLens,**<br>
> You can see the five music lanes on the sensors like the image in below. And when you press the sensors, you can see the circle under the lanes.<br>
> <img src=/images/holo_musiclanes.jpg height=200> <img src=/images/holo_pressing.jpg height=200><br>
## 2.7 Before playing
**Check the following**:
- **You can listen the sound when you press the sensors.**<br>
If this is wrong, you might miss setting IP address. Set it correctly.
- **The music you selected in Unity and the music opened in Ableton Live are same.**<br>
If this is wrong, set it in the setting menu after open it by press "**s**" key.
- **The track you selected in Unity and the track muted in Ableton Live are same.**<br>
If this is wrong, set it in the setting menu after open it by press "**s**" key.
- **The start position in Ableton Live is set the begining of the music (the number to left of play button is "1.1.1")**.<br>
If this is wrong, click the stop button in Ableton Live **twice**.<br>
 
**If you use HoloLens,**<br>
> - **You can see the music lanes in the correct position.**<br>
> If this is wrong, HoloLens miss tracking. click the toggle button of the other game in Unity and click "**Music Play**" again. Then you might see them correctly.<br>
## 2.8 Start playing
press "**space**" key. Then the music starts. Press the sensors at the timing indicated by the score.<br>
<img src=/images/uni_playing.png height=250> <img src=/images/holo_playing.jpg height=250><br>
The music stops by pressing "**space**" key.<br>
The setting menu opens by pressign "**s**" key.<br>
# 3. Add Music
I'll write this section in future. In this section, I write how to add new musics in this system.<br>
