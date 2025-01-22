using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Optimus.Core.Common.Types
{
    public readonly struct CPF
    {
        private static readonly Regex CPFRegex = new(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$");
        private string Value { get; }

        public CPF(string value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException("Invalid CPF format.");
            }
            Value = value;
        }

        public static bool IsValid(string cpf)
        {
            if (!CPFRegex.IsMatch(cpf))
                return false;

            // Implementar lógica para validar o CPF (cálculo dos dígitos verificadores, etc.)
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

    public class CPFJsonConverter : JsonConverter<CPF>
    {
        public override CPF Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return new CPF(stringValue);  // Construtor verifica e lança exceção, se necessário
        }

        public override void Write(Utf8JsonWriter writer, CPF cpf, JsonSerializerOptions options)
        {
            writer.WriteStringValue(cpf.ToString());
        }
    }
}
