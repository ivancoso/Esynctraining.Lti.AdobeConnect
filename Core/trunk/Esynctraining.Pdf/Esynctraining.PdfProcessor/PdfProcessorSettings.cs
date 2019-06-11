using System;

namespace Esynctraining.PdfProcessor
{
    public class PdfProcessorSettings
    {
        /// <summary>
        /// Setting this flag to true will force nuget gs package to search OS for installed GhostScript (through registry)
        /// </summary>
        public bool SearchForGhosts { get; set; }

        public string GhostScriptDllPath { get; set; } 

        public string GhostScriptLibPath { get; set; } 

        public Version GhostScriptVersion { get; set; }

    }

}
