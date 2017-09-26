using System.Xml;

namespace CloudFlareImUnderAttackMode
{
    class XmlDocFactory
    {
        /// <remarks>
        /// Xml Resolver is set to null in order to prevent XXE attacks:
        /// https://stackoverflow.com/questions/14230988/how-to-prevent-xxe-attack-xmldocument-in-net
        /// </remarks>
        public static XmlDocument Create(string xml)
        {
            XmlDocument xmlDoc = new XmlDocument { XmlResolver = null };
            xmlDoc.LoadXml(xml);
            return xmlDoc;
        }
    }
}
