Xrns2MOD is a tool that convert [renoise song](http://en.wikipedia.org/wiki/Renoise#XRNS_file_format) format to [mod](http://en.wikipedia.org/wiki/MOD_(file_format)) or [xm](http://en.wikipedia.org/wiki/XM_(file_format)) format

# Things to know

## Startup
First off download both Xrns2XMod main application and [tool](http://www.renoise.com/tools/xrns2xmod)  
The tool is an essential UI utility either for converting the song to desired format or help the user to set correct command values 

## Adjusting volume sample on Renoise
While mod/xm do not have a volume value for sample (they have a default sample volume instead), Xrns2XMod can use 2 distinct approach to let the sample play with the same Renoise's volume.

**Sample mode (default)**  
Xrns2XMod can "readjust" the sample data according to the Renoise volume value set for each sample
These images show the result of the re-sample process based from the original sample volume

_renoise original sample, with volume set to -10 db_  
![renoise_resample_to_volume](https://github.com/01010111/xrns2xmod/blob/master/docs/images/renoise_resample_to_volume.jpg?raw=true)

_converted sample_  
![mod_resample_to_volume](https://github.com/01010111/xrns2xmod/blob/master/docs/images/mod_resample_to_volume.jpg?raw=true)

**Column mode (xm only)**

With column mode, any volume effect will be scaled according to the original volume of the playing sample. 
Notice that also default volume for sample (if is not equal to 64) is taken into consideration.
The sample data remains unchanged.

_An example of the volume column conversion with 2 samples (one on the left and one on the right):_
_volume value is 0db_  
![volume_column_original](https://github.com/01010111/xrns2xmod/blob/master/docs/images/volume_column_original.jpg?raw=true)
 
_right volume value is -12db_  
![volume_column_converted](https://github.com/01010111/xrns2xmod/blob/master/docs/images/volume_column_converted.jpg?raw=true)

## Default Sample Volume
Renoise can't handle default volume sample, by the way it is possible to set a default volume for the sample in Xrns2XMod and then simulate it by put the default volume inside the second effect column of renoise (which is always ignored by Xrns2Xmod itself)

Use the helper tool to assign the default volume and assign a key assigned to this macro feature

_the default volume macro will assign the value to the second effect column of the line selected_  
![default_volume](https://github.com/01010111/xrns2xmod/blob/master/docs/images/default_volume.jpg?raw=true)

See below this documentation to read about instruments settings dialog tool

## Global volume effects
Since Renoise 2.8, Global effect volume is set inside the mastertrack column

Xrns2XMod get the the volume effect command inside the mastertack first column and put the result on the first free column effect found starting from the left. An example below:

_Global Volume_  
![mastertrack](https://github.com/01010111/xrns2xmod/blob/master/docs/images/mastertrack.jpg?raw=true)

_The Global Volume effect, put on the free empty slot_  
![mastertrack_converted](https://github.com/01010111/xrns2xmod/blob/master/docs/images/mastertrack_converted.jpg?raw=true)

Therefore, take care to leave an empty slot on any instrument track in the line after using global volume effect

## Playback options
![playback_mode](https://github.com/01010111/xrns2xmod/blob/master/docs/images/playback_mode.jpg?raw=true)

_OS Mode_ can be set in either ways, because Xrns2XMod can convert the Renoise value to Amiga/FT2.
Btw, keep in mind that samples over 65536 bytes might have inaccurate value.

_Pitch Mode_ can be used in either Renoise or Amiga/FT2 mode. In order to use Extrafine portamento values, use Renoise mode instead of Amiga/FT2
Notice: if value is set to Renoise, use the Helper Tool window to not mess up with wrong or inaccurate values.

**Note about pitch on MOD format**  
On MOD format, portamento effects are consistent throughout the note scale (see the Amiga linear frequency table).  
Since version 4.x, Xrns2XMod scan any note / portamento though pattern to achieve the right portamento value for played frequency.

# Command Line
usage for command line is:

```
Xrns2ModCmd.exe «inputFile»

-t|type [ mod | xm ] (destination format)
-portresh (Portamento treshold value (0-4, default: 2) (affects only mod)
-ptmode (ProTracker note range limitation) (default: true)
-volscal (scale volume according to renoise sample volume) [ none | sample | volume ] (default: sample)
-tempo [32 - 512] initial tempo value (affects only xm)
-ticks [1 - 31] initial ticks / row value (affects only xm)
-out «file» (output filename)
-log file» (log filename)
-bass_email {bass email}
-bass_code {bass_code}
```

## Options
**-type**

Specifies the format destination format

**-portresh** (default: 2)

When set to 4, Portamento will be prefered to ExtraFine Portamento command at the cost of inaccurate tone. 
Set 0 to always use ExtraFine Portamento (whenever possible) even when finetune value of 1 may be loss (discouraged)

**-ptmode** _(affects only mod)_

if set to false, extended NOT-ProTracker notes are available

**-volscal=[value]** _Volume scaling, based to renoise sample volume_

possibile values are:
_none_: renoise's sample volume won't affect the resulted mod/xm's sample
_sample_: target sample data will change according to renoise volume
_volume_: renoise sample volume will affect volume column of playing channel (only for xm)

See also Sample Volume for further information:

**-tempo** _Initial tempo for xm modules [32 - 512]_

**-ticks** _Initial tempo for xm modules [1 - 31]_

**-out «filename» **

output filename. 

default value is the same name of the source with the target format extension.

**-log «filename» **

log filename. 

**-bass email «bass email»**

set bass email for registrated user

**-bass code «bass code»**

set bass code for registrated user

# Window GUI
![xrns2xmod_gui](https://github.com/01010111/xrns2xmod/blob/master/docs/images/Xrns2XMod_GUI.jpg?raw=true)

The Xrns2XMod GUI is the windows interface to access all the Xrns2XMod features

# Tool
Download the Xrns2XMod Tool [here](http://www.renoise.com/tools/xrns2xmod)

## Menu
![menu](https://github.com/01010111/xrns2xmod/blob/master/docs/images/menu.jpg?raw=true)

With menu it is possible to access to windows or templates, demo or other Xrns2XMod stuff.
Some menu may be disabled according to the context or if main application path is not set

## Control Panel
![control_panel](https://github.com/01010111/xrns2xmod/blob/master/docs/images/control_panel.jpg?raw=true)

With control panel it is possible to locate Xrns2XMod application path

## Converter
![converter](https://github.com/01010111/xrns2xmod/blob/master/docs/images/converter.jpg?raw=true)

Main window for converting and adjust specific conversion settings

## Helper
![helper](https://github.com/01010111/xrns2xmod/blob/master/docs/images/helper.jpg?raw=true)

Window that help user to set correct portamento/volume values without messing up with accuracy conversion

## Instrument Settings
![instrument_settings](https://github.com/01010111/xrns2xmod/blob/master/docs/images/instrument_settings.jpg?raw=true)

Assign sample's default volume and sample rate for each sample.

Default value for volume is 64, while the converted sample rate is the actual sample rate

# Linux/Mac
_Xrns2XMod Shell on Ubuntu platform_
![screenshot from 2014-10-28 08-14-11](https://github.com/01010111/xrns2xmod/blob/master/docs/images/Screenshot%20from%202014-10-28%2008-14-11.png?raw=true)

To run Xrns2XModShell under Linux, you must install [Mono runtime](http://www.mono-project.com/docs/advanced/runtime/). Once mono runtime is correctly installed, download Xrns2XModShell.tar.gz file, copy zipped contents wherever you want and copy the *.so files located under "libs/[x64|x86]" directory (depending to your system's architecture) into Xrns2XModCmd root folder, then run _Xrns2XModShell.exe_ in the same way of Windows binary.

Take a look to README_linux.txt for any further information

Note for Mac users:  
Xrns2XMod has not been tested on any Mac OS yet, by the way it may be possible to compile and run it.
For this purpose, try the following:

1) Download Xrns2XMod source code on this site:
2) Download and install [monodevelop](http://www.mono-project.com/docs/about-mono/supported-platforms/osx/) on your OSX system
3) Open the solution from the Xrns2XMod source , then Build / Rebuild all
4) Copy the generated content wherever you like
5) Open the [BASS site](http://www.un4seen.com/bass.html) and download bass.dylib, libbassflac.dylib, libbassmix.dylib, then copy these files into Xrns2XMod folder

_MonoDevelop_  
![screenshot from 2014-10-28 08-43-11](https://github.com/01010111/xrns2xmod/blob/master/docs/images/Screenshot%20from%202014-10-28%2008-43-11.png?raw=true)

_A screenshot on Linux platform_  
![ubuntu](https://github.com/01010111/xrns2xmod/blob/master/docs/images/ubuntu.jpg?raw=true)

# Resources

## Video
[Video tutorial](http://youtu.be/AkgFeKhTpvs)  
[Demonstration video](http://www.youtube.com/watch?v=m7SN3FjkAk4)

## Test
Look into Resources folder under executable folder

## Music
[Towards A New Decade.xrns](https://github.com/01010111/xrns2xmod/raw/master/docs/examples/Towards%20A%20New%20Decade.xrns) a whole song, pro tracker convertible  
[Lack of Time.xrns](https://github.com/01010111/xrns2xmod/raw/master/docs/examples/Lack%20of%20Time.xrns) a pro tracker convertible module (read Song Comments before convert)  
[Temple of the competitors (game version new).xrns](https://github.com/01010111/xrns2xmod/raw/master/docs/examples/Temple%20of%20the%20competitors%20(game%20version%20new).xrns) a song written for an [iPhone game](http://itunes.apple.com/us/app/twinflix-onlinetetrismino/id404432764?mt=8), xm compatible

Last edited Apr 1, 2016 at 5:50 AM by Zenon66, version 115
