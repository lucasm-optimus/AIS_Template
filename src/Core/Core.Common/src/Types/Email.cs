using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Optimus.Core.Common.Types
{
    public readonly struct Email
    {
        public string Value { get; }

        public Email(string value)
        {
            if (!IsValid(value))
            {
                throw new ArgumentException("Invalid email format.");
            }
            Value = value;
        }

        public static bool IsValid(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
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

    public class EmailJsonConverter : JsonConverter<Email>
    {
        public override Email Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stringValue = reader.GetString();
            return new Email(stringValue);  // Construtor verifica e lança exceção, se necessário
        }

        public override void Write(Utf8JsonWriter writer, Email email, JsonSerializerOptions options)
        {
            writer.WriteStringValue(email.ToString());
        }
    }
}
