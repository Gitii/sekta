using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public partial class PolicyNamespaces
    {
        private static XmlSerializer serializer;

        [XmlElement("target")] public PolicyNamespaceAssociation Target { get; set; }

        [XmlElement("using")] public List<PolicyNamespaceAssociation> Using { get; set; }

        /// <summary>
        /// PolicyNamespaces class constructor
        /// </summary>
        public PolicyNamespaces()
        {
            Using = new List<PolicyNamespaceAssociation>();
            Target = new PolicyNamespaceAssociation();
        }

        private static XmlSerializer Serializer
        {
            get
            {
                if ((serializer == null))
                {
                    serializer = new XmlSerializerFactory().CreateSerializer(typeof(PolicyNamespaces));
                }

                return serializer;
            }
        }

        #region Serialize/Deserialize

        /// <summary>
        /// Serializes current PolicyNamespaces object into an XML string
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
        /// Deserializes workflow markup into an PolicyNamespaces object
        /// </summary>
        /// <param name="input">string workflow markup to deserialize</param>
        /// <param name="obj">Output PolicyNamespaces object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
        public static bool Deserialize(string input, out PolicyNamespaces obj, out Exception exception)
        {
            exception = null;
            obj = default(PolicyNamespaces);
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

        public static bool Deserialize(string input, out PolicyNamespaces obj)
        {
            Exception exception = null;
            return Deserialize(input, out obj, out exception);
        }

        public static PolicyNamespaces Deserialize(string input)
        {
            StringReader stringReader = null;
            try
            {
                stringReader = new StringReader(input);
                return ((PolicyNamespaces) (Serializer.Deserialize(XmlReader.Create(stringReader))));
            }
            finally
            {
                if ((stringReader != null))
                {
                    stringReader.Dispose();
                }
            }
        }

        public static PolicyNamespaces Deserialize(Stream s)
        {
            return ((PolicyNamespaces) (Serializer.Deserialize(s)));
        }

        #endregion

        /// <summary>
        /// Serializes current PolicyNamespaces object into file
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
        /// Deserializes xml markup from file into an PolicyNamespaces object
        /// </summary>
        /// <param name="fileName">string xml file to load and deserialize</param>
        /// <param name="obj">Output PolicyNamespaces object</param>
        /// <param name="exception">output Exception value if deserialize failed</param>
        /// <returns>true if this Serializer can deserialize the object; otherwise, false</returns>
        public static bool LoadFromFile(string fileName, out PolicyNamespaces obj, out Exception exception)
        {
            exception = null;
            obj = default(PolicyNamespaces);
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

        public static bool LoadFromFile(string fileName, out PolicyNamespaces obj)
        {
            Exception exception = null;
            return LoadFromFile(fileName, out obj, out exception);
        }

        public static PolicyNamespaces LoadFromFile(string fileName)
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
}