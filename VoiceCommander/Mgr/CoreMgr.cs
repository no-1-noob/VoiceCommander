using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR;
using VoiceCommander.Data;
using VoiceCommander.Interfaces;
using Zenject;

namespace VoiceCommander.Mgr
{
    internal class CoreMgr : IInitializable, IDisposable
    {
        [Inject] protected List<IVoiceCommandHandler> lsVoiceCommand = new List<IVoiceCommandHandler>();

        private KeywordRecognizer m_Recognizer;
        private List<VoiceCommand> lsKeyWordActions;

        public void Initialize()
        {
            Plugin.Log.Error("CoreMgr Initialize");
            foreach (var device in Microphone.devices)
            {
                Plugin.Log.Error("Microphone.devices: " + device);
            }
            List<VoiceCommand> lsAllVoiceCommands = new List<VoiceCommand>();
            foreach (IVoiceCommandHandler voiceCommandPlugin in lsVoiceCommand)
            {
                lsAllVoiceCommands.AddRange(voiceCommandPlugin.lsVoicecommand);
            }
            CreateRecognizer(lsAllVoiceCommands);
            Application.focusChanged += Application_focusChanged;
        }

        private void Application_focusChanged(bool obj)
        {
            Plugin.Log.Error($"Application_focusChanged {obj} Is running? {m_Recognizer?.IsRunning}");
        }

        public void Dispose()
        {
            Plugin.Log.Error($"Dispose coreMgr");
            m_Recognizer.Stop();
            m_Recognizer.Dispose();
            m_Recognizer = null;
            Application.focusChanged -= Application_focusChanged;
        }

        public void CreateRecognizer(List<VoiceCommand> lsKeyWordActions)
        {
            this.lsKeyWordActions = lsKeyWordActions;
            if (m_Recognizer != null)
            {
                m_Recognizer.Stop();
                m_Recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            }
            m_Recognizer = new KeywordRecognizer(lsKeyWordActions.Select(x => x.DefaultKeyword).ToArray());
            m_Recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            m_Recognizer.Start();

            if (!m_Recognizer.IsRunning)
            {
                Plugin.Log.Error("KeywordRecognizer is not running for some reason");
            }
        }

        private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            Plugin.Log?.Error($"{args.confidence} - {args.text}");
            VoiceCommand kewwordAction = ParseAction(args);
            if (kewwordAction.Activate)
            {
                try
                {
                    kewwordAction.Action.Invoke();
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error($"Error while activating Action for Keyword '{kewwordAction.Identifier}': {ex.Message}");
                }
            }
        }

        private VoiceCommand ParseAction(PhraseRecognizedEventArgs args)
        {
            VoiceCommand keywordAction = new VoiceCommand();
            VoiceCommand keywordActionSettings = lsKeyWordActions.Find(x => x.DefaultKeyword == args.text);
            if (keywordActionSettings != null)
            {
                keywordAction.Identifier = keywordActionSettings.Identifier;
                keywordAction.DefaultKeyword = keywordActionSettings.DefaultKeyword;
                keywordAction.ConfidenceLevel = args.confidence;
                keywordAction.Activate = keywordAction.ConfidenceLevel >= args.confidence;
                keywordAction.Action = keywordActionSettings.Action; ;
            }
            return keywordAction;
        }
    }
}
