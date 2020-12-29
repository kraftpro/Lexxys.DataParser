// Lexxys Infrastructural library.
// file: DataParser.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
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

