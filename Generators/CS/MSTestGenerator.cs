﻿using Grasshopper.Kernel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Tenrec.Components;

namespace Tenrec.Generators.CS
{
    public class MSTestGenerator : IGenerator
    {
        public string CreateAutoTestSourceFile
            (string[] ghTestFolders, string outputFolder, string outputName)
        {
            var log = new StringBuilder();
            var sb = new StringBuilder();
            var exits = false;
            var fileName = string.Empty;
            try
            {
                if (ghTestFolders == null || ghTestFolders.Length == 0)
                    throw new ArgumentNullException(nameof(outputFolder));
                if (string.IsNullOrEmpty(outputFolder))
                    throw new ArgumentNullException(nameof(outputFolder));

                sb.AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;");
                sb.AppendLine();
                sb.AppendLine($"namespace TenrecGeneratedTests");
                sb.AppendLine("{");
                foreach (var folder in ghTestFolders)
                {
                    var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".gh") || s.EndsWith(".ghx"));
                    if (files != null && files.Any())
                    {
                        foreach (var file in files)
                        {
                            if (Utils.IOHelper.OpenDocument(file, out GH_Document doc))
                            {
                                var groups = new List<IGH_DocumentObject>();
                                foreach (var obj in doc.Objects)
                                {
                                    if (obj.ComponentGuid == Group_UnitTest.ID)
                                    {
                                        groups.Add(obj);
                                    }
                                }
                                if (groups.Any())
                                {
                                    sb.AppendLine("    [TestClass]");
                                    sb.AppendLine($"    public class AutoTest_{Utils.StringHelper.CodeableNickname(doc.DisplayName)}");
                                    sb.AppendLine("    {");
                                    sb.AppendLine($"        public string FilePath => @\"{doc.FilePath}\";");
                                    sb.AppendLine("        private TestContext testContextInstance;");
                                    sb.AppendLine("        public TestContext TestContext { get => testContextInstance; set => testContextInstance = value; }");
                                    foreach (var group in groups)
                                    {
                                        sb.AppendLine("        [TestMethod]");
                                        sb.AppendLine($"        public void {Utils.StringHelper.CodeableNickname(group.NickName)}()");
                                        sb.AppendLine("        {");
                                        sb.AppendLine($"            Tenrec.Runner.Initialize(TestContext);");
                                        sb.AppendLine($"            Tenrec.Runner.RunTenrecGroup(FilePath, new System.Guid(\"{group.InstanceGuid}\"), TestContext);");
                                        sb.AppendLine("        }");
                                    }
                                    sb.AppendLine("    }");
                                    sb.AppendLine();
                                }
                            }
                            else
                            {
                                log.AppendLine($"File {file} failed to open.");
                            }
                        }
                    }
                }
                sb.AppendLine("}");

                fileName = Path.Combine(outputFolder, outputName + ".cs");
                exits = File.Exists(fileName);
                File.WriteAllText(fileName, sb.ToString());
            }
            catch (Exception e)
            {
                log.AppendLine($"EXCEPTION: {e}.");
            }

            if (exits)
                log.AppendLine($"File successfully overwritten.");
            else
                log.AppendLine($"File successfully created.");

            return log.ToString();
        }
    }
}