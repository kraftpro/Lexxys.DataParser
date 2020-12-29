// Lexxys Infrastructural library.
// file: Engine.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Lexxys.DataParsers.Engines
{
	using Templates;
	using Tokenizer;

	public abstract class Engine
	{
		private readonly Regex _start;
		private readonly Regex _end;

		protected Engine(RecordTemplate template)
		{
			string s = SetBol(template.Bol);
			_start = Template.Regex(s, RegexOptions.Multiline);
			s = SetBol(template.Eol);
			_end = Template.Regex(s, RegexOptions.Multiline);
		}

		public abstract string ParserType { get; }

		public virtual bool StartRecord(DataRecord record, CharStream text)
		{
			if (_start == null)
				return true;

			Match m = _start.Match(text.Substring(0, 128));
			if (m.Success)
			{
				text.Forward(m.Value.Length);
				return true;
			}
			record.SetError($"Begin of record does not match the picture specified ('{record.Template.Bol}')", text.GetPosition());
			return false;
		}

		public virtual bool EndRecord(DataRecord record, CharStream text)
		{
			if (_end == null)
				return true;
			Match m = _end.Match(text.Substring(0, 128));
			if (m.Success)
			{
				text.Forward(m.Value.Length);
				return true;
			}
			record.SetError($"End of record does not match the picture specified ('{record.Template.Eol}')", text.GetPosition());
			return false;
		}

		public virtual bool ParseRecord(DataRecord record, CharStream text)
		{
			record.SetError("ParseRecord is not implemented", text.GetPosition());
			return false;
		}

		public virtual bool Parse(DataRecord record, CharStream text)
		{
			var at = text.Position;
			if (StartRecord(record, text) &&
				ParseRecord(record, text) &&
				EndRecord(record, text))
			{
				if (text.Position != at)
					return true;

				record.SetError("Parsed zero length record ()", text.GetPosition());
			}

			text.Move(at);
			return false;
		}

		public static string CleanPicure(string picture)
		{
			return SetBolEol(picture);
		}

		public static string CleanComposition(string composition)
		{
			return composition;
		}

		private static string SetBolEol(string expression)
		{
			return String.IsNullOrEmpty(expression) ? null:
				__bolRex.IsMatch(expression) ? (__eolRex.IsMatch(expression) ? expression: "(?:" + expression + ")\\z"):
				__eolRex.IsMatch(expression) ? "\\A(?:" + expression + ")": "\\A(?:" + expression + ")\\z";
		}
		private static readonly Regex __eolRex = new Regex(@"(\$|\\z)(\)|\||\z)", RegexOptions.Compiled);

		private static string SetBol(string expression)
		{
			return String.IsNullOrEmpty(expression) ? null:
				__bolRex.IsMatch(expression) ? expression: "\\A(?:" + expression + ")";
		}
		private static readonly Regex __bolRex = new Regex(@"(^|\((\?(.|<[^>]*>))?|\|)(\^|\\A)", RegexOptions.Compiled);
	}
}

