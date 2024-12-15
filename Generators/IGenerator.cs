namespace Tenrec.Generators
{
    public interface IGenerator
    {
        string CreateAutoTestSourceFile
            (string[] ghTestFolders, string outputFolder, string outputName);
    }
}
