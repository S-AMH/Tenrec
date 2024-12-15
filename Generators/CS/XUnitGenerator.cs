﻿using Grasshopper.Kernel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Tenrec.Components;
using Tenrec.Utils;

namespace Tenrec.Generators.CS
{
    /// <summary>
    /// Produces code files automatically, generating test code based on Grasshopper files.
    /// </summary>
    /// <remarks> 
    /// This must be called once to produce the test files automatically, or every time you add or remove a <see cref="Group_UnitTest"/> or add or remove a file from the test folder.
    /// </remarks>
    public class XUnitGenerator : IGenerator
    {
        /// <summary>
        /// Generate a code file containing all the <see cref="Group_UnitTest"/>-based tests present in Grasshopper files.
        /// </summary>
        /// <param name="ghTestFolders">The folder containing the Grasshopper files.</param>
        /// <param name="outputFolder">The folder where to save the source code file.</param>
        /// <param name="outputName">The name of the resulting code file.</param>
        /// <returns>A log of the process.</returns>
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

                sb.AppendLine("using Xunit;");
                sb.AppendLine("using Xunit.Abstractions;");
                sb.AppendLine();
                sb.AppendLine("namespace TenrecGeneratedTests");
                sb.AppendLine("{");
                foreach (var folder in ghTestFolders)
                {
                    var files = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories).Where(s => s.EndsWith(".gh") || s.EndsWith(".ghx"));
                    if (!files.Any())
                    {
                        log.Append($"{folder} does not contain any readable format (.gh|.ghx)");
                        continue;
                    }
                    foreach (var file in files)
                    {
                        if (!IOHelper.OpenDocument(file, out GH_Document doc))
                        {
                            log.Append($"Failed to open {file}.");
                            continue;
                        }
                        var groups = doc.Objects.Where(o => o.ComponentGuid == Group_UnitTest.ID).ToList();
                        if (groups.Count == 0)
                            continue;

                        #region Crearte_Fixture_Class
                        sb.AppendLine(StringHelper.IndexedString($"public class AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}_Fixture : GHFileFixture", 1));
                        sb.AppendLine(StringHelper.IndexedString("{", 1));
                        sb.AppendLine(StringHelper.IndexedString($"public AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}_Fixture()", 2));
                        sb.AppendLine(StringHelper.IndexedString($" : base(@\"{file}\")", 3));
                        sb.AppendLine(StringHelper.IndexedString("{", 2));
                        sb.AppendLine(StringHelper.IndexedString("}", 2));
                        sb.AppendLine(StringHelper.IndexedString("}", 1));
                        #endregion

                        sb.AppendLine(StringHelper.IndexedString($"public class AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}" +
                            $" : IClassFixture<AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}_Fixture>", 1));
                        sb.AppendLine(StringHelper.IndexedString("{", 1));

                        sb.AppendLine(StringHelper.IndexedString($"private readonly AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}_Fixture fixture;", 2));
                        sb.AppendLine(StringHelper.IndexedString("private readonly ITestOutputHelper context;", 2));

                        #region Test_Class_Constructor
                        sb.AppendLine(StringHelper.IndexedString($"public AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}" +
                            $" (AutoTest_{StringHelper.CodeableNickname(doc.DisplayName)}_Fixture fixture, " +
                            $"ITestOutputHelper context)", 2));
                        sb.AppendLine(StringHelper.IndexedString("{", 2));
                        sb.AppendLine(StringHelper.IndexedString("this.fixture = fixture;", 3));
                        sb.AppendLine(StringHelper.IndexedString("this.context = context;", 3));
                        sb.AppendLine(StringHelper.IndexedString("}", 2));
                        #endregion

                        foreach (var group in groups)
                        {
                            sb.AppendLine(StringHelper.IndexedString("[Fact]", 2));
                            sb.AppendLine(StringHelper.IndexedString($"public void {StringHelper.CodeableNickname(group.NickName)}()", 2));
                            sb.AppendLine(StringHelper.IndexedString("{", 2));
                            sb.AppendLine(StringHelper.IndexedString($"fixture.RunGroup(fixture.Doc, new System.Guid(\"{group.InstanceGuid}\"), context);", 3));
                            sb.AppendLine(StringHelper.IndexedString("}", 2));
                        }
                        sb.AppendLine(StringHelper.IndexedString("}", 1));
                        sb.AppendLine();
                        doc.Dispose();
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
