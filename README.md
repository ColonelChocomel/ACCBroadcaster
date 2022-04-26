# ACC Broadcaster

## Introduction
ACC Broadcaster is a broadcasting client for Assetto Corsa Competizione, built using C# and WinUI 3. 
The goal is to provide a free, open source client that is superior to the default test client provided by the developers of ACC. 

![](https://imgur.com/8Eo4Q8e.png)

## System requirements
* Windows 10 version 1809 or later, or Windows 11.
* WindowsAppRuntime 1.0.3. Included in installer, or download from [here](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads#windows-app-sdk-10).

## How to use
1. Download and install the latest release from the [releases page](https://github.com/ColonelChocomel/ACCBroadcaster/releases).
2. Open Documents\Assetto Corsa Competizione\Config\broadcasting.json to edit the login credentials for broadcasting.
2. Launch Assetto Corsa Competizione and enter a single or multi player session.
2. Launch ACC Broadcaster. Under IP, enter 127.0.0.1 if ACC is running on your local PC. If ACC is running on a remote PC, enter that PC's IP address.
2. Enter the login credentials you entered earlier. If you have not set something, you can leave it empty.
2. Click the Connect button. If everything is correct, you should now be in the main broadcasting view and able to control the sim.

## Advantages over default client
ACC Broadcaster has the following advantages over the default ACC broadcasting client provided by Kunos:
* Cleaner interface with single click interactions rather than double click.
* Time deltas are displayed with all three decimals rather than one.
* Display of lap intervals during non-race sessions like Practice and Qualifying.
* Track position display lets you see exactly where cars are on track.
* Right click a car to change the focused car and camera angle at the same time, or start an instant replay for the selected car.
* Disconnected cars are moved to position 99 rather than sticking around in their pre-disconnect position.
* Invalid current laps are highlighted in red.

## Missing features 
The following features are currently missing compared to the default client and may or may not be added in the future.
* Auto director.
* List of highlights.
* Display of team names in car list.

ACC Broadcaster is currently also not optimised for multi-class sessions. It will work, however there are no specific tweaks for it like display of position in class.

## How to develop
If you would like to contribute or make your own modifications to ACC Broadcaster, please see the following:
* To develop WinUI 3 apps, check Microsoft's documentation [here](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/set-up-your-development-environment?tabs=vs-2022-17-1-a%2Cvs-2022-17-1-b)
to make sure you have installed the right tools. Also make sure you have installed WindowsAppRuntime 1.0.3 from [here](https://docs.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads#windows-app-sdk-10) or your build may not launch.
* In the Visual Studio solution you will find the ksBroadcastingNetwork project which is not included in the repository. This is a library provided by Kunos Simulazioni that handles communication between the 
broadcasting client and Assetto Corsa Competizione. To acquire this project, download "Assetto Corsa Competizione Dedicated Server" from the tools section on Steam. 
Then copy the `sdk\broadcasting\Sources\ksBroadcastingNetwork` folder to the root folder of the solution and reload the project.

## License
ACC Broadcaster is distributed under the GNU General Public License v3. A copy of the license is included in the COPYING file.