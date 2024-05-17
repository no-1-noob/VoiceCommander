﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VoiceCommander.Interfaces;
using WebSocketSharp;

namespace VoiceCommander.Recognizer
{
    internal class ExternalWebSocketSystemSpeechRecognizer : IRecognizer
    {
        private WebSocket webSocket;
        private string url = "ws://localhost:9898/socket";
        private List<string> _lsKeywords = new List<string>();

        public event EventHandler<(string, float)> OnKeyWordRecognized;

        public void CreateAndStartRecognizer(List<string> lsKeywords)
        {
            _lsKeywords = lsKeywords;
            webSocket = new WebSocket(url);
            webSocket.OnMessage += WebSocket_OnMessage;
            webSocket.OnError += WebSocket_OnError;
            webSocket.OnClose += WebSocket_OnClose;
            webSocket.OnOpen += WebSocket_OnOpen;
            webSocket.ConnectAsync();
        }

        public void CloseAndCleanupRecognizer()
        {
            if (webSocket.IsAlive)
            {
                VoiceCommandMessage message = new VoiceCommandMessage { type = VCType.REMOVE, lsCommands = _lsKeywords };
                webSocket.Send(JsonConvert.SerializeObject(message));
            }
            webSocket.OnMessage -= WebSocket_OnMessage;
            webSocket.OnError -= WebSocket_OnError;
            webSocket.CloseAsync();
        }

        private void WebSocket_OnOpen(object sender, EventArgs e)
        {
            VoiceCommandMessage message = new VoiceCommandMessage { type = VCType.ADD, lsCommands = _lsKeywords };
            webSocket.Send(JsonConvert.SerializeObject(message));
        }

        private void WebSocket_OnClose(object sender, CloseEventArgs e)
        {
            webSocket.OnMessage -= WebSocket_OnMessage;
            webSocket.OnError -= WebSocket_OnError;
        }

        private void WebSocket_OnMessage(object sender, MessageEventArgs e)
        {
            if (e.IsText && !string.IsNullOrEmpty(e.Data))
            {
                try
                {
                    VoiceRecognitionResult vcResult = JsonConvert.DeserializeObject<VoiceRecognitionResult>(e.Data);
                    OnKeyWordRecognized?.Invoke(this, (vcResult.text, vcResult.confidence));
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error($"Error while parsing VoiceRecognitionResult {ex.Message}. Data: {e.Data}");
                }
            }
        }

        private void WebSocket_OnError(object sender, ErrorEventArgs e)
        {
            Plugin.Log.Debug($"WebSocket_OnError {e.Message}");
        }

        private class VoiceCommandMessage
        {
            public VCType type { get; set; }
            public List<string> lsCommands = new List<string>();
        }

        private enum VCType
        {
            ADD,
            REMOVE
        }

        private class VoiceRecognitionResult
        {
            public string text { get; set; }
            public float confidence { get; set; }
        }
    }
}
