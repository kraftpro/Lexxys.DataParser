// Lexxys Infrastructural library.
// file: EnginesFactory.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
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

