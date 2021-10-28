using CommandLine;

namespace Workflow.States.Generic.Cat.Img.ByAuthor
{
    /// <summary>
    /// This class is one the pieces from the CommandLineParser nuget package 
    /// which takes commands line arguments in a console app / service and perses them
    /// to put them into this class properties. 
    /// The commands line arguments can be mandatory (see "value" attribute) or optional (see "option" attribute).
    /// </summary>
    public class CommandLineOptions
    {
        [Value(index: 0, Required = true, HelpText = "Path to watch.")]
        public string Path { get; set; }

        [Value(index: 1, MetaName ="OutputPath", Required = true, HelpText = "Output directory.")]
        public string OutputPath { get; set; }

        [Value(index: 2, MetaName = "ExecOrder", Required = true, HelpText = "Execution Order into the workflow nodes chain.")]
        public int ExecOrder { get; set; }

        [Option(shortName: 'e', longName: "extensions", Required = false, HelpText = "Valid image extensions.", Default = new[] { "png", "jpg", "jpeg" })]
        public string[] Extensions { get; set; }

        public float Confidence { get; set; }
    }
}
