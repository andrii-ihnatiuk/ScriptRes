# ScriptRes
Program to change display settings as you run specific executable file


## What's this?
It's a small, simple WPF program to automate the creation of scripted desktop shortcuts to:
1. Change the display resolution
2. Launch the desired application
3. Restore the resolution after closing the application


## Brief history
Having an ultrawide 3440x1440 monitor at home and being kind of a gamer, i often face a problem when my mid-range PC cannot run demanding games in 60 fps.
Changing the resolution to a lower one in a game often doesn't help, because some monitors or games don't allow you to leave black bars on the sides and stretch the image no matter what you do. I have faced this issue trying to play Cyberpunk 2077 with black bars by disabling scalling in Nvidia control panel - the game still stretches the image.
The only workaround for this - change the system resolution in Windows/Nvidia. Sounds not really fun, right? Imagine doing this every time you want to chill out in game.

I thought the same way, so i found out a handy command-line utility called [QRes](https://qres.sourceforge.net/). It allows you to change the display settings using cmd.
Using this utility i've created a batch script to change screen resolution on double click. But you actually need two scripts, second to revert back to original settings.

This already looks better than going to system settings on and on, but still not perfect. I tried to create a script that will change the resolution once i launch a game and restore it once the game is closed. 
And this worked! The only problem here - 0 automation. You have to adapt the script to the new game, set the file name, icon if you want it to look good.

Here the idea of this app has born. It is intended to take care of all this boiler-plate work. 
A few clicks and you have a ready-to-go desktop shortcut to launch your program in the desired resolution.

## Current features:
Not much
- You can set the initial and desired resolutions (lol)
- Change the location of your QRes.exe  (or leave the default)
- Create a desktop shortcut with configured name, icon and path to the generated batch file.


## Problems:
Much...

## TBD:
- Instead of entering X and Y, display all the resolutions available to the video adapter
- Ability to set the location of batch files
- Try to fix the poor color depth of saved extracted icons
- Improve app design, layout
- Set custom icons to shortcuts
- Use quality icons from .exe, .dll files
- idk.

*I'm developing this program in my spare time for personal use. But i hope it will be useful for someone else.*
