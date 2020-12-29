// Lexxys Infrastructural library.
// file: DataRecord.cs
//
// Copyright (c) 2001-2014.
//
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lexxys.DataParsers
{
	using Templates;
	using Tokenizer;

	public class DataRecord
	{
		private ParseErrorInfo _error;
		private int _at;

		private readonly RecordTemplate _template;
		private readonly List<DataField> _fields;

		public DataRecord(RecordTemplate template)
		{
			_template = template;
			_fields = new List<DataField>(template.Fields.Select(o => new DataField(this, o)));
		}

		public DataRecord(DataRecord that)
		{
			_error = that._error;
			_at = that._at;
			_template = that.Template;
			_fields = new List<DataField>(that._fields.Select(o => new DataField(this, o)));
		}

		public string Name => _template.Name;

		public ParseErrorInfo Error => _error;

		public int At => _at;

		public IReadOnlyList<DataField> Fields => _fields;

		public RecordTemplate Template => _template;

		public DataField this[string fieldName]
		{
			get { int i = _template.IndexOf(fieldName); return i < 0 ? null : _fields[i]; }
		}

		public bool Parse(CharStream text)
		{
			if (text == null)
				throw EX.ArgumentNull("text");

			_at = text.Position;
			return _template.Engine.Parse(this, text);
		}

		public void SetError(string message, CharPosition position)
		{
			_error = ParseErrorInfo.Create(message, position);
		}
	}
}
