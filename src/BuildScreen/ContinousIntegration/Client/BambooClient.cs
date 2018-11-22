using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.ContinousIntegration.Persistance;

namespace BuildScreen.ContinousIntegration.Client
{
    public class BambooClient : BaseClient, IClient
    {
        public BambooClient(ClientConfiguration clientConfiguration)
            : base(clientConfiguration)
        {
        }

        #region Implementation of IContinousIntegrationClient

        public ReadOnlyCollection<Build> Builds()
        {
            IEnumerable<XElement> plans = GetPlans();
            IEnumerable<XElement> latestBuilds = GetLatestBuild();

            IList<Build> builds = new List<Build>();
            foreach (XElement plan in plans)
            {
                Build build = GetBuild(latestBuilds, plan);

                builds.Add(build);
            }

            return new ReadOnlyCollection<Build>(builds);
        }

        public Build BuildByUniqueIdentifier(string key)
        {
            IEnumerable<XElement> plans = GetPlans();
            IEnumerable<XElement> latestBuilds = GetLatestBuild();

            return GetBuild(latestBuilds, plans.FirstOrDefault(x => x.Attribute("key").Value.Equals(key)));
        }

        #endregion

        private Build GetBuild(IEnumerable<XElement> latestBuilds, XElement plan)
        {
            Build build = new Build();
            XElement latestBuildForPlan = latestBuilds.FirstOrDefault(x => x.Attribute("key").Value.StartsWith(plan.Attribute("key").Value));

            build.UniqueIdentifier = plan.Attribute("key").Value;
            build.Status = latestBuildForPlan.Attribute("state").Value.Equals("Successful", StringComparison.OrdinalIgnoreCase) ? Status.Success : Status.Fail;
            build.Number = latestBuildForPlan.Attribute("number").Value;
            build.ProjectName = plan.Attribute("name").Value;

            XElement details = GetDetailsForBuild(latestBuildForPlan.Attribute("key").Value);
            build.StartDate = DateTime.Parse(details.Element("buildStartedTime").Value);
            build.StatusText = details.Element("buildTestSummary").Value;
            build.FinishDate = DateTime.Parse(details.Element("buildCompletedTime").Value);

            return build;
        }

        private XElement GetDetailsForBuild(string key)
        {
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}latest/result/{1}", BaseUri(), key));
            XDocument xDocument = LoadXmlDocument(uri);

            return xDocument.Element("result");
        }

        private IEnumerable<XElement> GetPlans()
        {
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}latest/plan", BaseUri()));
            XDocument xDocument = LoadXmlDocument(uri);

            return xDocument.Element("plans").Elements("plans").Elements("plan");
        }

        private IEnumerable<XElement> GetLatestBuild()
        {
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}latest/build", BaseUri()));
            XDocument xDocument = LoadXmlDocument(uri);

            return xDocument.Element("builds").Elements("builds").Elements("build");
        }

        //http://localhost:8085/rest/api/latest/plan
        //  <plans expand="plans">
        //  <link rel="self" href="http://localhost:8085/rest/api/latest/plan/" /> 
        //  <plans expand="plan" size="2" max-result="2" start-index="0">
        //  <plan enabled="true" type="chain" name="BuildScreen.Build Project - BuildScreen plan" key="BSPK-BSP">
        //  <link rel="self" href="http://localhost:8085/rest/api/latest/plan/BSPK-BSP" /> 
        //  </plan>
        //  <plan enabled="true" type="chain" name="BuildScreen.Build Project - Test" key="BSPK-PS">
        //  <link rel="self" href="http://localhost:8085/rest/api/latest/plan/BSPK-PS" /> 
        //  </plan>
        //  </plans>
        //  </plans>

        //http://localhost:8085/rest/api/latest/build
        // <builds expand="builds">
        //  <link rel="self" href="http://localhost:8085/rest/api/latest/build" /> 
        //- <builds xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xsi:type="restBuildList" expand="build" size="2" max-result="2" start-index="0">
        //- <build xsi:type="restChainResult" id="524299" number="6" lifeCycleState="Finished" state="Successful" key="BSPK-BSP-6">
        //  <link rel="self" href="http://localhost:8085/rest/api/latest/result/BSPK-BSP-6" /> 
        //  </build>
        //- <build xsi:type="restChainResult" id="524303" number="2" lifeCycleState="Finished" state="Failed" key="BSPK-PS-2">
        //  <link rel="self" href="http://localhost:8085/rest/api/latest/result/BSPK-PS-2" /> 
        //  </build>
        //  </builds>
        //  </builds>

        //http://localhost:8085/rest/api/latest/result/BSPK-BSP-6
        //<result id="524299" number="6" lifeCycleState="Finished" state="Successful" key="BSPK-BSP-6" expand="changes,metadata,stages,labels,jiraIssues,comments">
        //<link rel="self" href="http://localhost:8085/rest/api/latest/result/BSPK-BSP-6" /> 
        //<buildStartedTime>2011-01-26T16:26:06.860+01:00</buildStartedTime> 
        //<buildCompletedTime>2011-01-26T16:26:13.583+01:00</buildCompletedTime> 
        //<buildDurationInSeconds>6</buildDurationInSeconds> 
        //<buildDuration>6723</buildDuration> 
        //<buildDurationDescription>6 seconds</buildDurationDescription> 
        //<buildRelativeTime>28 minutes ago</buildRelativeTime> 
        //<vcsRevisionKey>8606d85a429b994fd5184cec2ff793afd50bc2d2</vcsRevisionKey> 
        //<buildTestSummary>No tests found</buildTestSummary> 
        //<successfulTestCount>0</successfulTestCount> 
        //<failedTestCount>0</failedTestCount> 
        //<buildReason>Manual build</buildReason> 
        //<stages size="1" max-result="1" start-index="0" /> 
        //<labels size="0" max-result="0" start-index="0" /> 
        //<jiraIssues size="0" max-result="0" start-index="0" /> 
        //<comments size="0" max-result="0" start-index="0" /> 
        //<changes size="0" max-result="0" start-index="0" /> 
        //<metadata size="2" max-result="2" start-index="0" /> 
        //</result>
    }
}
