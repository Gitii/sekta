using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using JetBrains.Annotations;

namespace Sekta.Admx.Schema;

[XmlRoot("policyDefinitionResources")]
public partial class PolicyDefinitionResources
{
    private static XmlSerializer _serializer;
    private static XmlSchema _schema;

    [XmlIgnore]
    [NotNull]
    public string CultureEnglishName { get; set; } = string.Empty;

    [CanBeNull]
    [XmlIgnore]
    public CultureInfo Culture { get; set; }

    [XmlElement("displayName")]
    public string DisplayName { get; set; }

    [XmlElement("description")]
    public string Description { get; set; }

    /// <remarks/>
    [XmlElement("annotation")]
    public Annotation[] Annotation { get; set; }

    [XmlElement("resources")]
    public Localization Resources { get; set; }

    /// <remarks/>
    [XmlAttribute("revision", DataType = "token")]
    public string Revision { get; set; }

    /// <remarks/>
    [XmlAttribute("schemaVersion", DataType = "token")]
    public string SchemaVersion { get; set; }

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
                    typeof(PolicyDefinitionResources)
                );
            }

            return _serializer;
        }
    }

    #region Deserialize

    public static PolicyDefinitionResources Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return (
                (PolicyDefinitionResources)(Serializer.Deserialize(XmlReader.Create(stringReader)))
            );
        }
        finally
        {
            if ((stringReader != null))
            {
                stringReader.Dispose();
            }
        }
    }

    public static PolicyDefinitionResources Deserialize(Stream s)
    {
        return ((PolicyDefinitionResources)(Serializer.Deserialize(s)));
    }

    public static async Task<(PolicyDefinitionResources, DeserializationLog)> DeserializeAsync(
        Stream s
    )
    {
        PolicyDefinitionResources resources = null;
        var log = new DeserializationLog();
        var readerSettings = new XmlReaderSettings();
        try
        {
            log.AttachTo(Serializer);
            log.AttachTo(readerSettings);

            readerSettings.Schemas.Add(Schema);
            readerSettings.ValidationType = ValidationType.Schema;

            var reader = XmlReader.Create(s, readerSettings);

            await Task.Run(
                () => resources = (PolicyDefinitionResources)Serializer.Deserialize(reader)
            );
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

        return (resources, log);
    }

    #endregion
}
