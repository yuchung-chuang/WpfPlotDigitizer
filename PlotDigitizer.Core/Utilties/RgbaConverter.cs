using Emgu.CV.Structure;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlotDigitizer.Core
{
	public class RgbaConverter : JsonConverter<Rgba>
	{
		public override Rgba Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{

			var color = new Rgba();
			object obj = color; // since rgba is a struct, it's necessary to box/unbox it to a class in order to take advantage of GetProperty().SetValue()
			string property = null;
			var isDone = false;
			while (!isDone) {
				reader.Read();
				switch (reader.TokenType) {
					case JsonTokenType.PropertyName:
						property = reader.GetString();
						break;
					case JsonTokenType.Number:
						var value = reader.GetDouble();
						typeof(Rgba).GetProperty(property).SetValue(obj, value);
						break;
					case JsonTokenType.EndObject:
						isDone = true;
						break;
					default:
						break;
				}
			}
			color = (Rgba)obj;
			return color;
		}

		public override void Write(Utf8JsonWriter writer, Rgba value, JsonSerializerOptions options)
		{
			writer.WriteStartObject();
			writer.WriteNumber(nameof(value.Red), value.Red);
			writer.WriteNumber(nameof(value.Green), value.Green);
			writer.WriteNumber(nameof(value.Blue), value.Blue);
			writer.WriteNumber(nameof(value.Alpha), value.Alpha);
			writer.WriteEndObject();
		}
	}
}
