// Lexxys Infrastructural library.
// file: EngineDelimited.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexxys.DataParsers.Engines
{
	using Templates;
	using Tokenizer;

	public class EngineDelimited: Engine
	{
		private readonly Regex _delimiter;
		private readonly string _endOfField;
		private readonly string _endOfLine;

		public EngineDelimited(RecordTemplate template)
			:base(template)
		{
			var d = string.IsNullOrEmpty(template.Delimiter) ? Template.DefaultDelimiter: template.Delimiter;
			var e = string.IsNullOrEmpty(template.Eol) ? Template.DefaultEol: template.Eol;
			string expression = @"(| *?""(?<v1>([^""]|"""")*)"" *?|(?<v2>[^""].*?))" + "((?<d>" + d + ")|" +
				(e == null ? "": "(?=" + e + ")|") + @"\z)";
			_endOfField = Clean(d);
			_endOfLine = Clean(e);
			if (_endOfLine == null)
				_endOfField = null;
			_delimiter = Template.Regex(expression, RegexOptions.ExplicitCapture | RegexOptions.Multiline);
		}

		private static string Clean(string value)
		{
			var v = value.Replace("\\n", "").Replace("\\t", "").Replace("\n", "").Replace("\t", "");
			return v == Regex.Escape(v) ? value.Replace("\\n", "\n").Replace("\\t", "\t"): null;
		}


		public override string ParserType => "delimited";

		public override bool ParseRecord(DataRecord record, CharStream text)
		{
			bool delimiter = true;
			foreach (DataField field in record.Fields)
			{
				if (!delimiter && record.Template.Strict)
				{
					record.SetError("Unexpected end of record found.", text.GetPosition());
					return false;
				}
				var fi = GetField(text);
				if (fi.Value == null)
				{
					field.At = text.Position;
					field.SetError($"Cannot extract field {field.Name}", text.GetPosition());
					return false;
				}
				if (!field.ComposeValue(fi.Value, text.Position, text))
					return false;

				delimiter = fi.Delimiter;
				text.Forward(fi.Length);
			}
			return true;
		}

		private FieldInfo GetField(CharStream text)
		{
			if (_endOfField != null)
			{
				bool delim = true;
				int i = text.IndexOf(_endOfField);
				int length;
				string s;
				if (i >= 0)
				{
					s = text.Substring(0, i);
					int j = s.IndexOf(_endOfLine, StringComparison.Ordinal);
					if (j < 0)
					{
						length = i + _endOfField.Length;
					}
					else
					{
						delim = false;
						length = j;
						s = s.Substring(0, j);
					}
				}
				else
				{
					delim = false;
					i = text.IndexOf(_endOfLine);
					length = i < 0 ? text.Length: i;
					s = text.Substring(0, length);
				}
				int k = s.IndexOf('"');
				if (k < 0 || !string.IsNullOrWhiteSpace(s.Substring(0, k)))
				{
					return new FieldInfo
					{
						Value = s,
						Length = length,
						Delimiter = delim
					};
				}
			}
			Match m = text.Match(_delimiter);
			if (m == null)
				return new FieldInfo();
			return new FieldInfo
			{
				Value = m.Groups["v1"].Success ? m.Groups["v1"].Value.Replace("\"\"", "\""): m.Groups["v2"].Value,
				Length = m.Length,
				Delimiter = m.Groups["d"].Success
			};
		}

		struct FieldInfo
		{
			public string Value;
			public int Length;
			public bool Delimiter;
		}
	}
}
