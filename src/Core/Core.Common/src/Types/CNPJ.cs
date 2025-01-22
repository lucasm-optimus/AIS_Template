using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Optimus.Core.Common.Types
{
    [JsonConverter(typeof(CNPJJsonConverter))]
    public readonly struct CNPJ
    {
        private static readonly Regex CNPJRegex = new(@"^\d{2}\.\d{3}\.\d{3}/\d{4}-\d{2}$");
        private string Value { get; }

        public CNPJ(string value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException("Invalid CNPJ format.");
            }
            Value = value;
        }

        public static bool IsValid(string cnpj)
        {
            // Validação de formato
            cnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            if (cnpj.Length != 14 || new string(cnpj[0], cnpj.Length) == cnpj)
                return false;

            // Implementar lógica de validação de CNPJ
            // ...

            return true;
        }

        public override string ToString() => Value;

        public string Unformatted => Value.Replace(".", "").Replace("/", "").Replace("-", "");

        public bool Equals(string other)
        {
            if (other == null) 
                return false;

            return string.Equals(Unformatted, other.Replace(".", "").Replace("/", "").Replace("-", ""), StringComparison.Ordinal);
        }
    }

    public class CNPJJsonConverter : JsonConverter<CNPJ>
    {
        public override CNPJ Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return new CNPJ(stringValue);  // Construtor verifica e lança exceção, se necessário
        }

        public override void Write(Utf8JsonWriter writer, CNPJ cnpj, JsonSerializerOptions options)
        {
            writer.WriteStringValue(cnpj.ToString());
        }
    }
}
