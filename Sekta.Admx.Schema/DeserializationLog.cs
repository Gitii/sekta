using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sekta.Admx.Schema
{
    public class DeserializationLog
    {
        public enum LogEntryLevel
        {
            Info = 0,
            Warning = 1,
            Error = 2,
        }

        public readonly struct LogEntry
        {
            public readonly LogEntryLevel Level;
            public readonly int LineIndex;
            public readonly int ColumnIndex;
            public readonly string Message;

            public LogEntry(LogEntryLevel level, int lineIndex, int columnIndex, string message)
            {
                Level = level;
                LineIndex = lineIndex;
                ColumnIndex = columnIndex;
                Message = message;
            }

            public override string ToString()
            {
                return $"{Level}: ({LineIndex}:{ColumnIndex}) {Message}";
            }
        }

        public List<LogEntry> Entries { get; } = new List<LogEntry>();

        public void AttachTo(XmlSerializer serializer)
        {
            serializer.UnknownAttribute += OnUnknownAttribute;
            serializer.UnknownNode += OnUnknownNode;
            serializer.UnknownElement += OnUnknownElement;
            serializer.UnreferencedObject += OnUnreferencedObject;
        }

        private void OnUnreferencedObject(object sender, UnreferencedObjectEventArgs args)
        {
            LogWarning(0, 0, "Unreferenced Object:" + args.UnreferencedId + "\t" + args.UnreferencedObject.ToString());
        }

        private void OnUnknownElement(object sender, XmlElementEventArgs args)
        {
            LogWarning(args.LineNumber, args.LinePosition, "Unknown Element:" + args.Element.Name + "\t" + args.Element.InnerXml);
        }

        private void OnUnknownNode(object sender, XmlNodeEventArgs args)
        {
            LogWarning(args.LineNumber, args.LinePosition, $"Unknown Node:{args.Name}\t{args.Text}");
        }

        private void OnUnknownAttribute(object sender, XmlAttributeEventArgs args)
        {
            XmlAttribute attr = args.Attr;
            LogWarning(args.LineNumber, args.LinePosition, $"Unknown attribute {attr.Name}=\'{attr.Value}\'");
        }

        private void OnValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    LogError(e.Exception?.LineNumber ?? 0, e.Exception?.LinePosition ?? 0, e.Message);
                    break;
                case XmlSeverityType.Warning:
                    LogWarning(e.Exception?.LineNumber ?? 0, e.Exception?.LinePosition ?? 0, e.Message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void LogWarning(int lineIndex, int columnIndex, string message)
        {
            Entries.Add(new LogEntry(LogEntryLevel.Warning, lineIndex, columnIndex, message));
        }

        public void LogError(int lineIndex, int columnIndex, string message)
        {
            Entries.Add(new LogEntry(LogEntryLevel.Error, lineIndex, columnIndex, message));
        }

        public void DettachFrom(XmlSerializer serializer)
        {
            serializer.UnknownAttribute -= OnUnknownAttribute;
            serializer.UnknownNode -= OnUnknownNode;
            serializer.UnknownElement -= OnUnknownElement;
            serializer.UnreferencedObject -= OnUnreferencedObject;
        }

        public void AttachTo(XmlReaderSettings readerSettings)
        {
            readerSettings.ValidationEventHandler += OnValidationEventHandler;
        }

        public void DettachFrom(XmlReaderSettings readerSettings)
        {
            readerSettings.ValidationEventHandler -= OnValidationEventHandler;
        }
    }
}
