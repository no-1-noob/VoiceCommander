A BeatSaber mod that allows voice commands to trigger actions ingame.

<b>This mod does nothing on it's own so you need another mod that uses this one. See below for a list of known mods with VoiceCommander support</b>

# Players

Download the mod from the release page, put the contents in the plugin folder (including the VCR sub folder)
### Why is there a random .exe in the VCR folder?
The built in voice recognition of Unity only works on Win10 and also sometimes does not resume to work after the game has been paused using SteamVR. So this small exe starts a programm, that handles the voice recognition and the plugin running in BeatSaber can attach itself to this programm.

While it is running you will see a console window with some output, this closes when the game is closed normally. If the game was force closed you have to close the command window yourself.

### Settings
For each mod that uses the VoiceCommander mod you can override the Keyword that triggers the action and the confidence needed to trigger it.

Confidence is a value from 0 to 1 (with 1 beign the best), depending on your mic/room noise you have to adjust the limit when an action is activated.

You can test all voice commands together on the right side of the settings menu

### Known mods that support VoiceCommander

https://github.com/no-1-noob/PauseCommander 
Activate automatic pause at the next pausable space in the song


# Developers

You need to two things to get voice command support in your mod

When you finished you mod let me know so i can add you to the above list ;)

## 1) Implement IVoiceCommandHandler

You need to implement the IVoiceCommandHandler interface.
Here you add the VoiceCommands to the lsVoiceCommand array which then is used in the VoiceCommander mod.

<b>Make sure that the constructor can be created without beign injected into the needed context. For the setting i have to be able to list all available actions from the lsVoicecommand array.</b>
```c#
using VoiceCommander.Data;
using VoiceCommander.Interfaces;

namespace MyMod.Command
{
    public class MyVoiceCommandHandler : IVoiceCommandHandler
    {
        public List<VoiceCommand> lsVoicecommand { get; } = new List<VoiceCommand>();

        public MyVoiceCommandHandler()
        {
            //MyMod.MyVoiceCommand.ThisIsATest is the identifier used for users setting their own keyword and confidence value. So make it unique.
            lsVoicecommand.Add(new VoiceCommand("MyMod.MyVoiceCommand.ThisIsATest", "This is a test", 0.9f, () => Plugin.Log.Error("Yeah Testing")));
            //... Add more here
        }        
    }
}
```

## 2) Add feature to the manifest
<i>Please remove the comments if you copy this example as json doenst support comments</i>
```json
{
  "$schema": "https://raw.githubusercontent.com/bsmg/BSIPA-MetadataFileSchema/master/Schema.json",
  //...
  "dependsOn": {
    "BSIPA": "^4.2.0"
  },
  "features": {
    "VoiceCommander.VoiceCommand": {
      "Commands": [
        {
          //Name of your mod, shown in settings
          "Name": "MyMod",
          //Location of the IVoiceCommandHandler implementation (namespace)
          "CommandLoacation": "MyMod.Command.MyVoiceCommandHandler",
          //Where should the voice command be available
          //Use the Zenject.Location enum
          "ZenjectLocation": "StandardPlayer"
          //Or if you want to make it available in multiple Locations use the bitmask in hex (e.g. 0xC = StandardPlayer and CampaignPlayer)
          //"ZenjectLocation": "0xC"
        }
        //This is an array so you could add multiple IVoiceCommandHandler implementations here (different commands for different context)
      ]
    }
  }
}
```