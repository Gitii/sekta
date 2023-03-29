using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema;

public partial class PolicyNamespaceAssociation
{
    private static XmlSerializer serializer;

    [XmlAttribute("prefix")]
    public string Prefix { get; set; }

    [XmlAttribute("namespace")]
    public string Namespace { get; set; }

    private static XmlSerializer Serializer
    {
        get
        {
            if ((serializer == null))
            {
                serializer = new XmlSerializerFactory().CreateSerializer(
                    typeof(PolicyNamespaceAssociation)
                );
            }

            return serializer;
        }
    }

    #region Serialize/Deserialize

    /// <summary>
    /// Serializes current PolicyNamespaceAssociation object into an XML string
    /// </summary>
    /// <returns>string XML value</returns>
    public virtual string Serialize()
    {
        StreamReader streamReader = null;
        MemoryStream memoryStream = null;
        try
        {
            memoryStream = new MemoryStream();
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings);
            Serializer.Serialize(xmlWriter, this);
            memoryStream.Seek(0, SeekOrigin.Begin);
            streamReader = new StreamReader(memoryStream);
            return streamReader.ReadToEnd();
        }
        finally
        {
            if ((streamReader != null))
            {
                streamReader.Dispose();
            }

            if ((memoryStream != null))
            {
                memoryStream.Dispose();
            }
        }
    }

    /// <summary>
    /// Deserializes workflow markup into an PolicyNamespaceAssociation object
    /// </summary>
    /// <param name="input">string workflow markup to deserialize</param>
    /// <param name="obj">Output PolicyNamespaceAssociation object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool Deserialize(
        string input,
        out PolicyNamespaceAssociation obj,
        out Exception exception
    )
    {
        exception = null;
        obj = default(PolicyNamespaceAssociation);
        try
        {
            obj = Deserialize(input);
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }

    public static bool Deserialize(string input, out PolicyNamespaceAssociation obj)
    {
        Exception exception = null;
        return Deserialize(input, out obj, out exception);
    }

    public static PolicyNamespaceAssociation Deserialize(string input)
    {
        StringReader stringReader = null;
        try
        {
            stringReader = new StringReader(input);
            return (
                (PolicyNamespaceAssociation)(Serializer.Deserialize(XmlReader.Create(stringReader)))
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

    public static PolicyNamespaceAssociation Deserialize(Stream s)
    {
        return ((PolicyNamespaceAssociation)(Serializer.Deserialize(s)));
    }

    #endregion

    /// <summary>
    /// Serializes current PolicyNamespaceAssociation object into file
    /// </summary>
    /// <param name="fileName">full path of outupt xml file</param>
    /// <param name="exception">output Exception value if failed</param>
    /// <returns>true if can serialize and save into file; otherwise, false</returns>
    public virtual bool SaveToFile(string fileName, out Exception exception)
    {
        exception = null;
        try
        {
            SaveToFile(fileName);
            return true;
        }
        catch (Exception e)
        {
            exception = e;
            return false;
        }
    }

    public virtual void SaveToFile(string fileName)
    {
        StreamWriter streamWriter = null;
        try
        {
            string xmlString = Serialize();
            FileInfo xmlFile = new FileInfo(fileName);
            streamWriter = xmlFile.CreateText();
            streamWriter.WriteLine(xmlString);
            streamWriter.Close();
        }
        finally
        {
            if ((streamWriter != null))
            {
                streamWriter.Dispose();
            }
        }
    }

    /// <summary>
    /// Deserializes xml markup from file into an PolicyNamespaceAssociation object
    /// </summary>
    /// <param name="fileName">string xml file to load and deserialize</param>
    /// <param name="obj">Output PolicyNamespaceAssociation object</param>
    /// <param name="exception">output Exception value if deserialize failed</param>
    /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
    public static bool LoadFromFile(
        string fileName,
        out PolicyNamespaceAssociation obj,
        out Exception exception
    )
    {
        exception = null;
        obj = default(PolicyNamespaceAssociation);
        try
        {
            obj = LoadFromFile(fileName);
            return true;
        }
        catch (Exception ex)
        {
            exception = ex;
            return false;
        }
    }

    public static bool LoadFromFile(string fileName, out PolicyNamespaceAssociation obj)
    {
        Exception exception = null;
        return LoadFromFile(fileName, out obj, out exception);
    }

    public static PolicyNamespaceAssociation LoadFromFile(string fileName)
    {
        FileStream file = null;
        StreamReader sr = null;
        try
        {
            file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            sr = new StreamReader(file);
            string xmlString = sr.ReadToEnd();
            sr.Close();
            file.Close();
            return Deserialize(xmlString);
        }
        finally
        {
            if ((file != null))
            {
                file.Dispose();
            }

            if ((sr != null))
            {
                sr.Dispose();
            }
        }
    }
}
