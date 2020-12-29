// Lexxys Infrastructural library.
// file: DataParser.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Lexxys.DataParsers
{
	using Templates;
	using Xml;

	public class DataParser
	{
		public DataParser(XmlLiteNode templateRef)
		{
			Template = new Template(templateRef);
		}

		public Template Template { get; }

		public DataReport Report(string reportType)
		{
			ReportTemplate template = Template.Report(reportType);
			return template == null ? null: new DataReport(template);
		}
	}
}
