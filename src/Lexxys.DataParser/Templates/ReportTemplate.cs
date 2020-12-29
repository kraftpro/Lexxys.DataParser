// Lexxys Infrastructural library.
// file: ReportTemplate.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexxys.DataParsers.Templates
{
	using Xml;

	public class ReportTemplate
	{
		public ReportTemplate(Template template, XmlLiteNode node)
		{
			if (template == null)
				throw EX.ArgumentNull(nameof(template));
			if (node == null)
				throw EX.ArgumentNull(nameof(node));

			Template = template;
			Name = node["name"];
			Description = node["description"];
			ParserType = node["parserType"] ?? Template.DefaultParserType;
			Delimiter = node["delimiter"] ?? Template.DefaultDelimiter;
			Bol = node["BOL"] ?? Template.DefaultBol;
			Eol = node["EOL"] ?? Template.DefaultEol;
			var records = new List<RecordTemplate>(node.Where("record").Select(o => new RecordTemplate(this, o)));
			Records = ReadOnly.Wrap(records);
			var recordsMap = new Dictionary<string, RecordTemplate>(StringComparer.OrdinalIgnoreCase);
			foreach (var record in records)
			{
				recordsMap.Add(record.Name, record);
			}
			RecordsMap = ReadOnly.Wrap(recordsMap);
		}

		public string Name { get; }

		public string Description { get; }

		public string ParserType { get; }

		public string Delimiter { get; }

		public string Bol { get; }

		public string Eol { get; }

		public Template Template { get; }

		public IReadOnlyList<RecordTemplate> Records { get; }

		public IReadOnlyDictionary<string, Alias> Aliases => Template.Aliases;
		public IReadOnlyDictionary<string, RecordTemplate> RecordsMap { get; }

		public RecordTemplate Record(string name)
		{
			RecordsMap.TryGetValue(name, out RecordTemplate x);
			return x;
		}

		public Alias Alias(string name)
		{
			return Template.Alias(name);
		}
	}
}

