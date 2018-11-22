using System;

namespace BuildScreen.Plugin
{
    public interface IBuildScreenBuild
    {
        string Id { get; set; }
        string Number { get; set; }
        string Status { get; set; }
        string StatusText { get; set; }
        DateTime StartDate { get; set; }
        DateTime FinishDate { get; set; }
        string TypeId { get; set; }
        string TypeName { get; set; }
        string ProjectId { get; set; }
        string ProjectName { get; set; }
    }
}
