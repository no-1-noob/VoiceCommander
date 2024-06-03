using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Speech.Recognition;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace VoiceCommandRecognizer
{
    internal class VCWebSocketServer : IDisposable
    {
        private string _serverUrl = "ws://localhost:";
        private WebSocketServer _server;
        private List<string> _keywords = new List<string>();
        private SpeechRecognitionEngine listener;
        private VoiceCommandMessage currentCommand = null;
        internal List<VoiceCommandMessage> lsMessageBuffer = new List<VoiceCommandMessage>();

        public VCWebSocketServer()
        {
            string port = ConfigurationManager.AppSettings["port"];
            _serverUrl += port;
            StartSocket();
        }

        public void StartSocket()
        {
            _server = new WebSocketServer($"{_serverUrl}");
            _server.AddWebSocketService<VCRSocketBehaviour>("/socket");
            _server.Start();
            Program.Log($"VoiceCommander recognizer started at {_serverUrl}", Program.MsgType.SUCCESS);

            Task.Run(async () =>
            {
                while (true)
                {
                    if (currentCommand == null)
                        HandleBuffer();
                    await Task.Delay(50);
                }
            });
        }

        public void CloseSocket()
        {
            _server?.Stop();
            Program.Log($"WebSocket closed", Program.MsgType.SUCCESS);
        }

        private void ReStartRecognition()
        {
            try
            {
                Program.Log($"Re/Start voice recognition after changing keywords", Program.MsgType.INFO);
                if (listener == null)
                {
                    listener = new SpeechRecognitionEngine();
                    listener.SetInputToDefaultAudioDevice();
                    listener.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(listener_SpeechRecognized);
                }
                listener.RecognizeAsyncStop();
                listener.RecognizeAsyncCancel();
                listener.UnloadAllGrammars();
                StartRecognizer();
            }
            catch (Exception ex)
            {
                Program.Log($"Error while ReStartRecognition {ex.Message}", Program.MsgType.ERROR);
            }
        }

        private void StartRecognizer()
        {
            Program.Log($"StartRecognizer", Program.MsgType.DEBUG);
            foreach (var keyword in _keywords.GroupBy(x => x))
            {
                Program.Log($"Add to SpeechRecognitionEngine: {keyword.Key} : Count : {keyword.Count<string>()}", Program.MsgType.DEBUG);
                GrammarBuilder builder = new GrammarBuilder(keyword.Key);
                Grammar g = new Grammar(builder);
                listener.LoadGrammar(g);
            }
            if (listener.Grammars.Count > 0)
            {
                listener.RecognizeAsync(RecognizeMode.Multiple);
            }
            currentCommand = null;
        }

        private void listener_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Program.Log($"listener_SpeechRecognized {_server}: {e.Result.Confidence} {e.Result.Text}", Program.MsgType.DEBUG);
            if (_server != null)
            {
                VoiceRecognitionResult vcResult = new VoiceRecognitionResult() { confidence= e.Result.Confidence, text = e.Result.Text };
                _server.WebSocketServices["/socket"].Sessions.Broadcast(JsonConvert.SerializeObject(vcResult));
            }
        }

        private void HandleBuffer()
        {
            if (lsMessageBuffer.Count > 0)
            {
                currentCommand = lsMessageBuffer.First();
                lsMessageBuffer.RemoveAt(0);
                HandleMessage(currentCommand);
            }
        }

        private void HandleMessage(VoiceCommandMessage vcMessage)
        {
            switch (vcMessage.type)
            {
                case VCType.ADD:
                    AddKeywords(vcMessage.lsCommands);
                    break;
                case VCType.REMOVE:
                    RemoveKeywords(vcMessage.lsCommands);
                    break;
                default:
                    break;
            }
        }

        private void AddKeywords(List<string> keywords)
        {
            foreach (var item in keywords)
            {
                _keywords.Add(item);
                Program.Log($"Added keyword: {item}", Program.MsgType.DEBUG);
            }
            ReStartRecognition();
        }

        private void RemoveKeywords(List<string> keywords)
        {
            foreach (var item in keywords)
            {
                _keywords.Remove(item);
                Program.Log($"Removed keyword: {item}", Program.MsgType.DEBUG);
            }
            ReStartRecognition();
        }

        public void Dispose()
        {
            CloseSocket();
        }
    }

    internal class VCRSocketBehaviour : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Program.Log($"VCRSocketBehaviour received message: {e.Data}", Program.MsgType.DEBUG);
            if (e.IsText)
            {
                try
                {
                    VoiceCommandMessage socketData = JsonConvert.DeserializeObject<VoiceCommandMessage>(e.Data);
                    Program.VCWebSocketServer.lsMessageBuffer.Add(socketData);
                }
                catch (Exception ex)
                {
                    Program.Log($"VCRSocketBehaviour error while parsing: {e.Data}. Error {ex.Message}", Program.MsgType.ERROR);
                }
            }
        }
    }

    internal class VoiceCommandMessage
    {
        public VCType type { get; set; }
        public List<string> lsCommands = new List<string>();
    }

    internal enum VCType
    {
        ADD,
        REMOVE
    }

    internal class VoiceRecognitionResult
    {
        public string text { get; set; }
        public float confidence { get; set; }
    }
}
