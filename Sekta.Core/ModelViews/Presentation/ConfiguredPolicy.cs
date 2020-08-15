using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using ReactiveUI;
using Sekta.Admx.Schema;

namespace Sekta.Core.ModelView.Presentation
{
    public class ConfiguredPolicy: ReactiveObject
    {
        public ConfiguredPolicy(string policyDefinitionName, PolicyClass policyClass)
        {
            PolicyDefinitionName = policyDefinitionName ?? throw new ArgumentNullException(nameof(policyDefinitionName));
            PolicyClass = policyClass;
            Values = new ObservableCollection<ConfiguredPolicyOption>();
            IsEnabled = null;
        }

        public const int VERSION = 1;
        public const int MINIMUM_SUPPORTED_VERSION = VERSION;

        public string PolicyDefinitionName { get; }

        public PolicyClass PolicyClass { get; }

        public ObservableCollection<ConfiguredPolicyOption> Values { get; }

        bool? _isEnabled;
        public bool? IsEnabled
        {
            get { return _isEnabled; }
            set { this.RaiseAndSetIfChanged(ref _isEnabled, value); }
        }

        public static string Serialize(IEnumerable<ConfiguredPolicy> policies)
        {
            return JsonConvert.SerializeObject(policies.ToList(), new ConfiguredPolicyConverter());
        }

        public static List<ConfiguredPolicy> Deserialize(string strPolicyList)
        {
            return JsonConvert.DeserializeObject<List<ConfiguredPolicy>>(strPolicyList, new ConfiguredPolicyConverter());
        }

        private class ConfiguredPolicyConverter : JsonConverter<ConfiguredPolicy>
        {
            public override void WriteJson(JsonWriter writer, ConfiguredPolicy value, JsonSerializer serializer)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(nameof(VERSION));
                writer.WriteValue(VERSION);
                writer.WritePropertyName(nameof(IsEnabled));
                writer.WriteValue(value.IsEnabled);
                writer.WritePropertyName(nameof(PolicyDefinitionName));
                writer.WriteValue(value.PolicyDefinitionName);
                writer.WritePropertyName(nameof(PolicyClass));
                writer.WriteValue((int) value.PolicyClass);
                writer.WritePropertyName("Values");
                writer.WriteStartArray();
                foreach (ConfiguredPolicyOption policyOption in value.Values)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName(nameof(ConfiguredPolicyOption.ElementId));
                    writer.WriteValue(policyOption.ElementId);
                    writer.WritePropertyName(nameof(ConfiguredPolicyOption.ElementValue));
                    writer.WriteValue(policyOption.ElementValue.Serialize());
                    writer.WriteEndObject();
                }
                writer.WriteEndObject();
            }

            public override ConfiguredPolicy ReadJson(JsonReader reader, Type objectType, ConfiguredPolicy existingValue, bool hasExistingValue,
                JsonSerializer serializer)
            {
                reader.Read(); // start object
                var version = ReadIntegerProperty(nameof(VERSION));
                if (!version.HasValue || version < MINIMUM_SUPPORTED_VERSION)
                {
                    throw new ArgumentException($"Failed to deserialize the policy: The minimum supported version is {MINIMUM_SUPPORTED_VERSION} but got {version}!");
                }

                var isEnabled = ReadBooleanProperty(nameof(IsEnabled));
                var name = ReadStringProperty(nameof(PolicyDefinitionName));
                var policyClass = ReadIntegerProperty(nameof(PolicyClass)).Value;
                var policy = existingValue ?? new ConfiguredPolicy(name, (Admx.Schema.PolicyClass) policyClass);

                reader.Read(); // start array
                while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                {
                    reader.Read(); // start object
                    var id = ReadStringProperty(nameof(ConfiguredPolicyOption.ElementId));
                    var rawValue = ReadStringProperty(nameof(ConfiguredPolicyOption.ElementValue));

                    PolicyOptionValue optionValue = PolicyOptionValue.Deserialize(rawValue);
                    policy.Values.Add(new ConfiguredPolicyOption(id, optionValue));
                }

                reader.Read(); // end array

                policy.IsEnabled = isEnabled;

                return policy;

                void AssertCurrent(JsonToken token)
                {
                    Debug.Assert(reader.TokenType == token);
                    reader.Read();
                }

                T ReadProperty<T>(string propertyName, Func<T> readValue)
                {
                    Debug.Assert(reader.TokenType == JsonToken.PropertyName);
                    string actualPropertyName = (string) reader.Value;
                    Debug.Assert(propertyName == actualPropertyName);
                    
                    T value = readValue();

                    reader.Read();

                    return value;
                }
                
                string ReadStringProperty(string propertyName)
                {
                    return ReadProperty<string>(propertyName, () => reader.ReadAsString());
                }

                bool? ReadBooleanProperty(string propertyName)
                {
                    return ReadProperty<bool?>(propertyName, () => reader.ReadAsBoolean());
                }

                int? ReadIntegerProperty(string propertyName)
                {
                    return ReadProperty<int?>(propertyName, () => reader.ReadAsInt32());
                }
            }
        }
    }
}