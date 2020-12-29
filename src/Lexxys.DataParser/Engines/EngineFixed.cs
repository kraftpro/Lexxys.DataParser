// Lexxys Infrastructural library.
// file: EngineFixed.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace Lexxys.DataParsers.Engines
{
	using Templates;
	using Tokenizer;

	public class EngineFixed: Engine
	{
		public EngineFixed(RecordTemplate template)
			:base(template)
		{
		}

		public override string ParserType => "fixed";

		public override bool ParseRecord(DataRecord record, CharStream text)
		{
			int position = 0;
			int at = text.Position;

			int i = text.IndexOf('\n');
			int lineSize = i < 0 ? text.Length: i;

			string buffer = text.Substring(0, lineSize);
			foreach (DataField field in record.Fields)
			{
				int offset = field.Position;
				if (offset < 0)
					offset = position;
				int length = field.Length;
				if (length < 0 || offset + length > buffer.Length)
					length = buffer.Length - offset;
				if (!field.ComposeValue(length <= 0 ? "": buffer.Substring(offset, length), at + offset, text))
					return false;
				position = offset + length;
			}
			text.Forward(lineSize);
			return true;
		}
	}
}
