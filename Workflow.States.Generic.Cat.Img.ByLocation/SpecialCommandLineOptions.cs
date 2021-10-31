using CommandLine;

namespace Workflow.States.Generic.Cat.Img.ByLocation
{
    /// <summary>
    /// This worklow state may need spacial parameters becasue it has excternal api's
    /// dependencies
    /// </summary>
    public class SpecialCommandLineOptions
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
