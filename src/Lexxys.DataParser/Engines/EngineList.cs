// Lexxys Infrastructural library.
// file: EngineList.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexxys.DataParsers.Engines
{
	using Templates;
	using Tokenizer;


	public class EngineList: Engine
	{
		private readonly Regex _delimiter;

		public EngineList(RecordTemplate template)
			:base(template)
		{
			string d = string.IsNullOrEmpty(template.Delimiter) ? Template.DefaultDelimiter: template.Delimiter;
			string e = string.IsNullOrEmpty(template.Eol) ? Template.DefaultEol: template.Eol;
			string expression = @"(( *?""(?<v1>([^""]|"""")*)"") *?|(?<v2>[^""].*?))" + "(" + d + "|" +
					(e == null ? "": e + "|") + "$)";
			_delimiter =  new Regex(expression, RegexOptions.ExplicitCapture | RegexOptions.Multiline | RegexOptions.Compiled);
		}

		public override string ParserType => "list";

		public override bool ParseRecord(DataRecord record, CharStream text)
		{
			var start = text.Position;
			var fields = new List<DataField>(record.Fields);
			int n = fields.Count;
			for (int i = 1; i <= n; ++i)
			{
				Match m = text.Match(_delimiter);
				if (!m.Success)
					break;
				string value = m.Groups["v1"].Success ? m.Groups["v1"].Value.Replace("\"\"", "\""): m.Groups["v2"].Value;

				int k = fields.FindIndex(o => o != null && o.ComposeValue(value, text.Position, text));
				if (k < 0)
					break;

				text.Forward(m.Value.Length);
				fields[k] = null;
			}

			if (start != text.Position)
				return true;
			
			record.SetError("No fields found", text.GetPosition());
			return false;
		}
	}
}

