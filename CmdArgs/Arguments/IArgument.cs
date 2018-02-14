namespace CmdArgs
{
    /// <summary>
    /// For Argument and ArgumentAttribute
    /// </summary>
    public interface IArgument
    {
        char? ShortName { get; }
        string LongName { get; }


        bool Mandatory { get; }
        bool AllowMultiple { get; }


        string Description { get; }
        string FullDescription { get; }
        string Example { get; }
    }
}