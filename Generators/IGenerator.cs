namespace Tenrec.Generators
{
    /// <summary>
    /// Produces code files automatically, generating test code based on Grasshopper files.
    /// </summary>
    /// <remarks> 
    /// This must be called once to produce the test files automatically, or every time you add or remove a <see cref="Group_UnitTest"/> or add or remove a file from the test folder.
    /// </remarks>
    public interface IGenerator
    {
        /// <summary>
        /// Generate a code file containing all the <see cref="Group_UnitTest"/>-based tests present in Grasshopper files.
        /// </summary>
        /// <param name="ghTestFolders">The folder containing the Grasshopper files.</param>
        /// <param name="outputFolder">The folder where to save the source code file.</param>
        /// <param name="outputName">The name of the resulting code file.</param>
        /// <returns>A log of the process.</returns>
        string CreateAutoTestSourceFile
            (string[] ghTestFolders, string outputFolder, string outputName);
    }
}
