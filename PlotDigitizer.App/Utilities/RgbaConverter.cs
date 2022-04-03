using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PlotDigitizer.App
{
	public class RgbaConverter : JsonConverter<Rgba>
	{
		public override Rgba Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var s = reader.GetString();
			var s2 = s.Substring(s.IndexOf('(') + 1, s.IndexOf(')') - s.IndexOf('(') - 1);
			var values = s2.Split(',').Select(s => double.Parse(s)).ToArray();
			return new Rgba(values[0], values[1], values[2], values[3]);
		}

		public override void Write(Utf8JsonWriter writer, Rgba value, JsonSerializerOptions options)
		{
			writer.WriteStringValue($"{nameof(Rgba)}:({value.Red},{value.Green},{value.Blue},{value.Alpha})");
		}
	}
}
