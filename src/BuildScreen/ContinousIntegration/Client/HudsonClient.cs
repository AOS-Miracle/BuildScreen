using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using BuildScreen.ContinousIntegration.Entities;
using BuildScreen.ContinousIntegration.Persistance;
using BuildScreen.Core.Extensions;

namespace BuildScreen.ContinousIntegration.Client
{
    public class HudsonClient : BaseClient, IClient
    {
        public HudsonClient(ClientConfiguration clientConfiguration)
            : base(clientConfiguration)
        {
        }

        #region Implementation of IContinousIntegrationClient

        public ReadOnlyCollection<Build> Builds()
        {
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/api/xml?tree=jobs[name,lastBuild[result]]", BaseUri()));
            XDocument xDocument = LoadXmlDocument(uri);

            IEnumerable<XElement> xmlBuilds = xDocument.Element("hudson").Elements("job");

            IList<Build> builds = xmlBuilds.Select(xmlBuild => new Build
            {
                UniqueIdentifier = xmlBuild.Element("name").Value,
                TypeName = xmlBuild.Element("name").Value,
                Status =
                    xmlBuild.Element("lastBuild").Value.Equals(
                        "success",
                        StringComparison.OrdinalIgnoreCase)
                        ? Status.Success
                        : Status.Fail,
            }).ToList();

            return new ReadOnlyCollection<Build>(builds);
        }

        public Build BuildByUniqueIdentifier(string key)
        {
            string buildNumber = GetBuildNumber(key);

            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/job/{1}/{2}/api/xml?", BaseUri(), key, buildNumber));
            XDocument xDocument = LoadXmlDocument(uri);

            string root = xDocument.Element("freeStyleBuild") != null ? "freeStyleBuild" : "mavenModuleSetBuild";

            DateTime startTime = Double.Parse(xDocument.Element(root).Element("timestamp").Value).ToDateTimeFromUnixTimestamp();

            return new Build
            {
                Number = buildNumber,
                Status = xDocument.Element(root).Element("result").Value.Equals("success", StringComparison.OrdinalIgnoreCase) ? Status.Success : Status.Fail,
                UniqueIdentifier = key,
                TypeName = key,
                StartDate = startTime,
                FinishDate = startTime.AddMilliseconds(Double.Parse(xDocument.Element(root).Element("duration").Value))
            };
        }

        #endregion

        internal string GetBuildNumber(string typeId)
        {
            Uri uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/job/{1}/api/xml?", BaseUri(), typeId));
            XDocument xDocument = LoadXmlDocument(uri);

            string root = xDocument.Element("freeStyleProject") != null ? "freeStyleProject" : "mavenModuleSet";
            IEnumerable<XElement> xmlBuilds = xDocument.Element(root).Elements("build");

            return xmlBuilds.First().Element("number").Value;
        }
    }
}
