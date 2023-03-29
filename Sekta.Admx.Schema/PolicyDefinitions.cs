using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

/// <summary>
/// The base type for general ADMX files with satellite resource files, etc.
/// </summary>
[XmlRoot("policyDefinitions")]
public partial class PolicyDefinitions
{
    private static XmlSerializer _serializer;
    private static XmlSchema _schema;

    [XmlElement("policyNamespaces")]
    public PolicyNamespaces PolicyNamespaces { get; set; }

    [XmlElement("supersededAdm")]
    public List<FileReference> SupersededAdm { get; set; }

    [XmlElement("annotation")]
    public List<Annotation> Annotation { get; set; }

    [XmlElement("resources")]
    public LocalizationResourceReference Resources { get; set; }

    public SupportedOnTable SupportedOn { get; set; }

    [XmlArray("categories")]
    [XmlArrayItem("category", typeof(Category))]
    public List<Category> Categories { get; set; }

    [XmlArray("policies")]
    [XmlArrayItem("policy", typeof(PolicyDefinition))]
    public List<PolicyDefinition> Policies { get; set; }

    [XmlAttribute("revision")]
    public string Revision { get; set; }

    [XmlAttribute("schemaVersion")]
    public string SchemaVersion { get; set; }

    /// <summary>
    /// PolicyDefinitions class constructor
    /// </summary>
    public PolicyDefinitions()
    {
        Policies = new List<PolicyDefinition>();
        Categories = new List<Category>();
        SupportedOn = new SupportedOnTable();
        Resources = new LocalizationResourceReference();
        Annotation = new List<Annotation>();
        SupersededAdm = new List<FileReference>();
        PolicyNamespaces = new PolicyNamespaces();
    }

    private static XmlSchema Schema
    {
        get
        {
            if (_schema == null)
            {
                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName = "Sekta.Admx.Schema.PolicyDefinitionFiles.xsd";

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    _schema = XmlSchema.Read(reader, (object sender, ValidationEventArgs e) => { });
                }
            }

            return _schema;
        }
    }

    private static XmlSerializer Serializer
    {
        get
        {
            if ((_serializer == null))
            {
                _serializer = new XmlSerializerFactory().CreateSerializer(
                    typeof(PolicyDefinitions)
                );
            }

            return _serializer;
        }
    }

    #region Deserialize

    public static PolicyDefinitions Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return ((PolicyDefinitions)(Serializer.Deserialize(XmlReader.Create(stringReader))));
        }
        finally
        {
            if ((stringReader != null))
            {
                stringReader.Dispose();
            }
        }
    }

    public static PolicyDefinitions Deserialize(Stream s)
    {
        return ((PolicyDefinitions)(Serializer.Deserialize(s)));
    }

    public static async Task<(PolicyDefinitions, DeserializationLog)> DeserializeAsync(Stream s)
    {
        PolicyDefinitions defs = null;
        var log = new DeserializationLog();
        var readerSettings = new XmlReaderSettings();
        try
        {
            log.AttachTo(Serializer);
            log.AttachTo(readerSettings);

            readerSettings.Schemas.Add(Schema);
            readerSettings.ValidationType = ValidationType.Schema;

            var reader = XmlReader.Create(s, readerSettings);

            await Task.Run(() => defs = (PolicyDefinitions)Serializer.Deserialize(reader));
        }
        catch (Exception e)
        {
            log.Entries.Add(
                new DeserializationLog.LogEntry(
                    DeserializationLog.LogEntryLevel.Error,
                    0,
                    0,
                    e.Message
                )
            );
        }
        finally
        {
            log.DettachFrom(Serializer);
            log.DettachFrom(readerSettings);
        }

        return (defs, log);
    }

    #endregion
}
