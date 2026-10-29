using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laba_1
{
    // 1. Полиморфный конвертер (требование лабы)
    public class EnemyTemplateConverter : JsonConverter<CEnemyTemplate>
    {
        public override CEnemyTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var jsonDoc = JsonDocument.ParseValue(ref reader);

            string type = "CNormalEnemyTemplate";
            if (jsonDoc.RootElement.TryGetProperty("$type", out JsonElement typeElement))
            {
                type = typeElement.GetString();
            }

            // Создаем опции без нашего конвертера, чтобы избежать StackOverflow
            var safeOptions = new JsonSerializerOptions(options);
            for (int i = safeOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (safeOptions.Converters[i] is EnemyTemplateConverter)
                    safeOptions.Converters.RemoveAt(i);
            }

            return type switch
            {
                "CArmoredEnemyTemplate" => JsonSerializer.Deserialize<CArmoredEnemyTemplate>(jsonDoc.RootElement.GetRawText(), safeOptions),
                "CHealingEnemyTemplate" => JsonSerializer.Deserialize<CHealingEnemyTemplate>(jsonDoc.RootElement.GetRawText(), safeOptions),
                "CNormalEnemyTemplate" => JsonSerializer.Deserialize<CNormalEnemyTemplate>(jsonDoc.RootElement.GetRawText(), safeOptions),
                _ => JsonSerializer.Deserialize<CNormalEnemyTemplate>(jsonDoc.RootElement.GetRawText(), safeOptions)
            };
        }

        public override void Write(Utf8JsonWriter writer, CEnemyTemplate value, JsonSerializerOptions options)
        {
            string type = value.GetType().Name;

            var safeOptions = new JsonSerializerOptions(options);
            for (int i = safeOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (safeOptions.Converters[i] is EnemyTemplateConverter)
                    safeOptions.Converters.RemoveAt(i);
            }

            string json = JsonSerializer.Serialize(value, value.GetType(), safeOptions);
            using var jsonDoc = JsonDocument.Parse(json);

            writer.WriteStartObject();
            writer.WriteString("$type", type);
            foreach (var property in jsonDoc.RootElement.EnumerateObject())
            {
                property.WriteTo(writer);
            }
            writer.WriteEndObject();
        }
    }

    // 2. Класс сохранения, реализующий интерфейс
    public class JsonEnemySaver : ISaveList<List<CEnemyTemplate>>
    {
        private readonly JsonSerializerOptions _options;

        public JsonEnemySaver()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new EnemyTemplateConverter() }
            };
        }

        public List<CEnemyTemplate> Load(string path)
        {
            if (!File.Exists(path)) return new List<CEnemyTemplate>();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<CEnemyTemplate>>(json, _options) ?? new List<CEnemyTemplate>();
        }

        public void Save(List<CEnemyTemplate> data, string path)
        {
            string json = JsonSerializer.Serialize(data, _options);
            File.WriteAllText(path, json);
        }
    }
}