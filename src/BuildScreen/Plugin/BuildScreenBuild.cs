using System;

namespace BuildScreen.Plugin
{
    [Serializable]
    public class BuildScreenBuild : IBuildScreenBuild
    {
        public string Id { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string StatusText { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime FinishDate { get; set; }
        public string TypeId { get; set; }
        public string TypeName { get; set; }
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
