using Grasshopper.Kernel;
using System;

namespace Tenrec.Utils
{
    /// <summary>
    /// Contains helper classes for input/output operations used in Tenrec project.
    /// </summary>
    public static class IOHelper
    {
        public static bool OpenDocument(string filePath, out GH_Document doc)
        {
            var io = new GH_DocumentIO();
            if (!io.Open(filePath))
            {
                doc = null;
                throw new Exception($"Failed to open file: {filePath}");
            }
            doc = io.Document;
            doc.Enabled = true;
            return true;
        }
    }
}
