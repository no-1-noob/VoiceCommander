﻿using System;
using System.Collections.Generic;
using System.IO;
using IPA.Loader;
using IPA.Loader.Features;
using Newtonsoft.Json.Linq;
using VoiceCommander.Interfaces;

namespace VoiceCommander
{
    public class VoiceCommandFeature : Feature
    {
        private Dictionary<PluginMetadata, List<VoiceCommandSettings>> voiceCommandsToLoad = new Dictionary<PluginMetadata, List<VoiceCommandSettings>>();

        protected override bool Initialize(PluginMetadata meta, JObject featureData)
        {
            VoiceCommandSettingsList lsVoiceCommands;
            try
            {
                lsVoiceCommands = featureData.ToObject<VoiceCommandSettingsList>();
            }
            catch (Exception e)
            {
                InvalidMessage = $"Invalid data: {e}";
                return false;
            }

            voiceCommandsToLoad.Add(meta, lsVoiceCommands.Commands);
            return true;
        }

        public override void AfterInit(PluginMetadata meta)
        {
            if (voiceCommandsToLoad.TryGetValue(meta, out List<VoiceCommandSettings> lsVoiceCommand))
            {
                foreach (var voiceCommand in lsVoiceCommand)
                {
                    if (TryLoadType(ref voiceCommand.CommandType, meta, voiceCommand.CommandLoacation))
                    {
                        try
                        {
                            IVoiceCommandHandler voiceCommandHandler = (IVoiceCommandHandler) Activator.CreateInstance(voiceCommand.CommandType);
                            Plugin.Log.Error($"voiceCommandHandler lsCommands {voiceCommandHandler.lsVoicecommand.Count}");
                            if(voiceCommandHandler.lsVoicecommand.Count == 0)
                            {
                                Plugin.Log.Error($"No voice commands found for {voiceCommand.Name}. Make sure the List of commands is filled when calling the standard constructor");
                                continue;
                            }
                            //TODO:
                            //Check if Identifier is unique
                            //Check if Keywords are unique
                        }
                        catch (Exception ex)
                        {
                            Plugin.Log.Error($"Failed to create instance for {voiceCommand.Name} using the default constructor. Please do not use any properties that could be null in the constructor etc. Exception: {ex.Message}");
                            continue;
                        }

                        if (Enum.TryParse(voiceCommand.ZenjectLocation, out voiceCommand.ZenjectLocationEnum))
                        {
                            Plugin.AvailableVoiceCommandSettings.Add(voiceCommand);
                            Plugin.Log.Notice($"Loaded VoiceCommands for Plugin {voiceCommand.Name}");
                        }
                        else
                        {
                            Plugin.Log.Error($"Failed to parse ZenjectLocation for {voiceCommand.Name}.");
                        }
                    }
                    else
                    {
                        Plugin.Log.Error($"Failed to load a Type from the provided CommandLoacation for {voiceCommand.Name}.");
                    }
                }
            }
            else
            {
                Plugin.Log.Critical($"VoiceCommand not loaded {meta.Name}");
            }
        }

        private bool TryLoadType(ref Type typeToLoad, PluginMetadata meta, string location)
        {
            // totally didn't yoink this from Counters+ wich yoinked it from BSIPA's ConfigProviderFeature
            try
            {
                typeToLoad = meta.Assembly.GetType(location);
            }
            catch (ArgumentException)
            {
                InvalidMessage = $"Invalid type name {location}";
                return false;
            }
            catch (Exception e) when (e is FileNotFoundException || e is FileLoadException || e is BadImageFormatException)
            {
                string filename;

                switch (e)
                {
                    case FileNotFoundException fn:
                        filename = fn.FileName;
                        goto hasFilename;
                    case FileLoadException fl:
                        filename = fl.FileName;
                        goto hasFilename;
                    case BadImageFormatException bi:
                        filename = bi.FileName;
                    hasFilename:
                        InvalidMessage = $"Could not find {filename} while loading type";
                        break;
                    default:
                        InvalidMessage = $"Error while loading type: {e}";
                        break;
                }

                return false;
            }
            catch (Exception e) // Is this unnecessary? Maybe.
            {
                InvalidMessage = $"An unknown error occured: {e}";
                return false;
            }

            return true;
        }
    }
}
