// Lexxys Infrastructural library.
// file: RecordTemplate.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lexxys.DataParsers.Templates
{
	using Xml;
	using Engines;

	public class RecordTemplate
	{
		public RecordTemplate(ReportTemplate report, XmlLiteNode node)
		{
			if (report == null)
				throw EX.ArgumentNull(nameof(report));
			if (node == null)
				throw EX.ArgumentNull(nameof(node));

			Report = report;
			Name = node["name"];
			Description = node["description"];
			ParserType = node["parserType"] ?? report.ParserType;
			Bol = report.Template.SubstituteAliases(node["BOL"] ?? report.Bol);
			Eol = report.Template.SubstituteAliases(node["EOL"] ?? report.Eol);
			Delimiter = report.Template.SubstituteAliases(node["delimiter"] ?? report.Delimiter);
			Strict = node["strict"].AsBoolean(false);
			Fields = new List<FieldTemplate>(node.Where("field").Select(o => new FieldTemplate(this, o)));
			Engine = EnginesFactory.Create(this);
			Map = new Dictionary<string, int>(Fields.Count, StringComparer.OrdinalIgnoreCase);
			for (int i = 0; i < Fields.Count; ++i)
			{
				Map[Fields[i].Name] = i;
			}
		}

		public string Name { get; }
		public string Description { get; }
		public string ParserType { get; }
		public string Bol { get; }
		public string Eol { get; }
		public string Delimiter { get; }
		public Engine Engine { get; }
		public ReportTemplate Report { get; }
		public bool Strict { get; private set; }
		public List<FieldTemplate> Fields { get; }
		public Dictionary<string, int> Map { get; }

		public IReadOnlyDictionary<string, Alias> Aliases => Report.Aliases;

		public int IndexOf(string name)
		{
			int result;
			return Map.TryGetValue(name, out result) ? result : -1;
		}
	}
}
