using System;
using System.Collections.Generic;

namespace VoiceCommander.Interfaces
{
    /// <summary>
    /// Interface for VoiceRecognizer
    /// </summary>
    internal interface IRecognizer
    {
        /// <summary>
        /// Start voice recognition with the given keywords
        /// </summary>
        /// <param name="lsKeywords"></param>
        void CreateAndStartRecognizer(List<string> lsKeywords);
        /// <summary>
        /// Remove all old Keywords and add the new ones
        /// </summary>
        /// <param name="lsKeywords"></param>
        void ResetRecognizer(List<string> lsKeywords);
        /// <summary>
        /// Restart the recognition with the previously given keywords
        /// </summary>
        void Start();
        /// <summary>
        /// Pausing the recognition but keep all other infos
        /// </summary>
        void Pause();
        /// <summary>
        /// Stop voice recognition and clean up everything relating to it
        /// </summary>
        void CloseAndCleanupRecognizer();
        /// <summary>
        /// Notify when a keyword has been recognized (Keyword, confidence [0->1])
        /// </summary>
        event EventHandler<(string keyword, float confidence)> OnKeyWordRecognized;
    }
}
