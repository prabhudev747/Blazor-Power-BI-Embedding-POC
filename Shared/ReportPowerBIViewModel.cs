namespace BlazorPowerBI.Shared
{
    public class ReportPowerBIViewModel
    {
        // Workspace Id for which Embed token needs to be generated
        public string WorkspaceId { get; set; }

        // Report Id for which Embed token needs to be generated
        public string ReportId { get; set; }

        public string username { get; set; }
        public bool userrole { get; set; }
    }
}
