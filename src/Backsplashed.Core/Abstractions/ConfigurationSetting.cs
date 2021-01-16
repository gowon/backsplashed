namespace Backsplashed.Core.Abstractions
{
    using System.ComponentModel.DataAnnotations;
    using Newtonsoft.Json;

    public class ConfigurationSetting
    {
        [Key]
        public string Key { get; set; }

        public string Type { get; set; }
        public string Value { get; set; }

        public static ConfigurationSetting CreateFrom(string key, object value)
        {
            return new()
            {
                Key = key,
                Type = value.GetType().FullName,
                Value = JsonConvert.SerializeObject(value)
            };
        }

        public TObject ConvertTo<TObject>()
        {
            var messageType = System.Type.GetType(Type);
            return (TObject) JsonConvert.DeserializeObject(Value, messageType!);
        }
    }
}