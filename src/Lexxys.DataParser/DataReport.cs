// Lexxys Infrastructural library.
// file: DataReport.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexxys.DataParsers
{
	using Templates;

	public class DataReport
	{
		public DataReport(ReportTemplate template)
		{
			if (template == null)
				throw EX.ArgumentNull("template");

			ReportTemplate = template;
			var records = new Dictionary<string, DataRecord>(StringComparer.OrdinalIgnoreCase);
			foreach (var record in template.Records.Select(o => new DataRecord(o)))
			{
				records.Add(record.Name, record);
			}
			Records = ReadOnly.Wrap(records);
		}

		public ReportTemplate ReportTemplate { get; }

		public IReadOnlyDictionary<string, DataRecord> Records { get; }

		public DataRecord Record(string name)
		{
			RecordTemplate template = ReportTemplate.Record(name);
			return template == null ? null: new DataRecord(template);
		}
	}
}
