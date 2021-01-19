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
				throw EX.ArgumentNull(nameof(template));

			return template.ParserType?.ToUpperInvariant() switch
			{
				"FIXED" => new Engines.EngineFixed(template),
				"DELIMITED" => new Engines.EngineDelimited(template),
				"LIST" => new Engines.EngineList(template),
				_ => throw EX.ArgumentOutOfRange("template.ParserType", template.ParserType),
			};
		}
	}
}

