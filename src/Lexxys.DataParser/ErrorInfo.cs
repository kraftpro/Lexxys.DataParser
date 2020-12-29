// Lexxys Infrastructural library.
// file: ErrorInfo.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using Lexxys.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexxys.DataParsers
{
	public class ParseErrorInfo
	{
		public CharPosition Position { get; }
		public string Message { get; }

		private ParseErrorInfo(string message, CharPosition position)
		{
			Position = position;
			Message = message;
		}

		public override string ToString()
		{
			return Message == null ? "":
				Position.Line == 0 && Position.Column == 0 ? Message:
				Position.Column == 0 ? $"{Message}, line {Position.Line + 1}.": $"{Message}, line {Position.Line + 1}, column {Position.Column + 1}";
		}

		public static ParseErrorInfo Create(string message, CharPosition position)
		{
			message = message.TrimToNull();
			return message == null ? null: new ParseErrorInfo(message, position);
		}

	}
}

