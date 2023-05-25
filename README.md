<img src="./ScriptRes/icons8-resolution-96.ico" align="left"/>

# ScriptRes
<br/>

**A program to change display settings as you run specific executable file.**
<br/><br/>

## 🙄 What's this ?
It's a small, simple WPF program to automate the creation of scripted desktop shortcuts to:
1. Change the display resolution
2. Launch the desired application
3. Restore the resolution after closing the application

**This is not a standalone application.** It cannot change the display resolution by itself. To do so it creates a `.bat` script where **QRes.exe** is called.
It's primary goal is to minimize your effort on creating such scripts by yourself and configuring a nice-looking desktop shortcut.

## ⚙️ Installation
1. Make sure you have a [.NET 7.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.5-windows-x64-installer) installed on your machine
2. Go to the [releases](https://github.com/andrii-ihnatiuk/ScriptRes/releases) page and download the latest `.rar` archive
3. Place the extracted files wherever you want and launch `ScriptRes.exe`

## 🙌🏻 Brief history
Having an ultrawide 3440x1440 monitor at home and being kind of a gamer, i often face a problem when my mid-range PC cannot run demanding games in 60 fps.
Changing the resolution to a lower one in a game often doesn't help, because some monitors or games don't allow you to leave black bars on the sides and stretch the image no matter what you do. I have faced this issue trying to play Cyberpunk 2077 with black bars by disabling scalling in Nvidia control panel - the game still stretches the image.
The only workaround for this - change the system resolution in Windows/Nvidia. Sounds not really fun, right? Imagine doing this every time you want to chill out in game.

I thought the same way, so i found out a handy command-line utility called [QRes](https://qres.sourceforge.net/). It allows you to change the display settings using cmd.
Using this utility i've created a batch script to change screen resolution on double click. But you actually need two scripts, second to revert back to original settings.

This already looks better than going to system settings on and on, but still not perfect. I tried to create a script that will change the resolution once i launch a game and restore it once the game is closed. 
And this worked! The only problem here - 0 automation. You have to adapt the script to the new game, set the file name, icon if you want it to look good.

Here the idea of this app has born. It is intended to take care of all this boiler-plate work. 
A few clicks and you have a ready-to-go desktop shortcut to launch your program in the desired resolution.

## 🚀 Available features:
- Set the initial and desired resolutions
- Change the location of your QRes.exe  (or leave the default)
- Select a custom icon for the shortcut or one of the extracted, if any
- Enter a custom name for the shortcut **(be careful not to override an existing shortcut, as there is no check for this yet)**
- Create a desktop shortcut with the entered configuration

## 👎🏻 Problems:
- The script won't work correctly if the selected program is already running.

<br/><br/>
***I'm developing this program in my spare time for personal use. But i hope it will be useful for someone else.***
