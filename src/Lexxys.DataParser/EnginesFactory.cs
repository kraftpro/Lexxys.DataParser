// Lexxys Infrastructural library.
// file: EnginesFactory.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexxys.DataParsers
{
	static class EnginesFactory
	{
		public static Engines.Engine Create(Templates.RecordTemplate template)
		{
			if (template == null)
				throw EX.ArgumentNull("template");

			switch ((template.ParserType ?? "").ToUpperInvariant())
			{
				case "FIXED":
					return new Engines.EngineFixed(template);
				case "DELIMITED":
					return new Engines.EngineDelimited(template);
				case "LIST":
					return new Engines.EngineList(template);
				default:
					throw EX.ArgumentOutOfRange("template.ParserType", template.ParserType);
			}
		}
	}
}
