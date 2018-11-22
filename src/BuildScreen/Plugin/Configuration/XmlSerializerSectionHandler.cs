using System;
using System.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

namespace BuildScreen.Plugin.Configuration
{
    /// <summary>
    /// A Generic configuration section handler that leverages the power of xml serialization. 
    /// See http://alt.pluralsight.com/wiki/default.aspx/Craig.XmlSerializerSectionHandler for
    /// more information.
    /// </summary>
    public class XmlSerializerSectionHandler : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates the object representing the configuration section. The section will
        /// be serialized into the type specified by the type attribute in the xml.
        /// 
        /// This method is called by the framework, for example when ConfiguratinManager.GetSection() 
        /// is called. It should not be called directly by client code. 
        /// </summary>
        /// <param name="parent">Parent object</param>
        /// <param name="configContext">Configuration context object</param>
        /// <param name="section">Section XML node</param>
        /// <returns></returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            XPathNavigator nav = section.CreateNavigator();
            string typename = (string)nav.Evaluate("string(@type)");
            Type type = Type.GetType(typename);

            if (type == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Type '{0}' specified with the XmlSerializerSectionHandler could not be resolved.", typename)
                    );
            }

            try
            {
                using (XmlNodeReader xmlNodeReader = new XmlNodeReader(section))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(type);
                    return xmlSerializer.Deserialize(xmlNodeReader);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Unwrap any exception
                if (ex.InnerException != null)
                    throw ex.InnerException;
                throw;
            }
        }
    }
}
