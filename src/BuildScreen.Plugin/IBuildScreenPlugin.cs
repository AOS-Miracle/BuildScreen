namespace BuildScreen.Plugin
{
    public interface IBuildScreenPlugin
    {
        bool Start(IBuildScreenBuild build);
        bool Kill(IBuildScreenBuild build);
        void CleanUp();
        string Name { get; set; }
    }
}
