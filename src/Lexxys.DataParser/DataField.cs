// Lexxys Infrastructural library.
// file: DataField.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Lexxys.DataParsers
{
	using Templates;
	using Tokenizer;

	public class DataField
	{
		private ParseErrorInfo _error;
		private object _value;
		private readonly object _default;

		public DataField(DataRecord record, DataField that)
		{
			Record = record;
			Template = that.Template;
			_error = that._error;
			At = that.At;
			_default = that._default;
			_value = that._value;
		}

		public DataField(DataRecord record, FieldTemplate template)
		{
			Record = record ?? throw EX.ArgumentNull(nameof(record));
			Template = template ?? throw EX.ArgumentNull(nameof(template));
			_default = Factory.DefaultValue(Template.ValueType);
			TryParse(template.DefaultValue, Template.ValueType, ref _default);
			_value = _default;
		}

		public DataRecord Record { get; }
		public int At { get; set; }
		public FieldTemplate Template { get; }
		public ParseErrorInfo Error => _error;
		public object Value => _value;
		public int Position => Template.Position;
		public int Length => Template.Length;
		public string Name => Template.Name;
		public Type ValueType => Template.ValueType;
		public string StringValue => _value == null ? null: Convert.ToString(_value).TrimToNull();
		public int? IntValue => _value == null ? (int?)null: Convert.ToInt32(_value);
		public decimal? DecimalValue => _value == null ? (decimal?)null: Convert.ToDecimal(_value);
		public double? DoubleValue => _value == null ? (double?)null: Convert.ToDouble(_value);
		public DateTime? DateTimeValue => _value == null ? (DateTime?)null: Convert.ToDateTime(_value);

		public void SetError(string message, CharPosition at)
		{
			_error = ParseErrorInfo.Create(message, at);
		}

		public bool PutValue(string value)
		{
			_value = _default;
			if (TryParse(value, ValueType, ref _value))
				return true;
			_value = _default;
			return false;
		}

		private static bool TryParse(string value, Type valueType, ref object result)
		{
			if (string.IsNullOrWhiteSpace(value))
				return true;
			if (valueType == typeof(DateTime?))
			{
				string t = value.Replace("-", "").Trim();
				if (string.IsNullOrEmpty(t) || t == "000000" || t =="00000000")
					return true;
				var m = __dateTimeRex.Match(value);
				if (m.Success)
				{
					value = (m.Groups["y"].Length == 2 ? "20" + m.Groups["y"].Value : m.Groups["y"].Value) + "-" + 
						m.Groups["m"].Value.PadLeft(2, '0') + "-" + 
						m.Groups["d"].Value.PadLeft(2, '0') +
						m.Groups["r"].Value;
				}
			}
			return Xml.XmlTools.TryGetValue(value, valueType, out result);
		}
		private static readonly Regex __dateTimeRex = new Regex(@"\A\s*(?<y>(?:\d\d)?\d\d)-?(?<m>\d\d?)-?(?<d>\d\d?)(?<r>.*)\z", RegexOptions.Compiled);

		public bool ComposeValue(string text, int position, CharStream stream)
		{
			At = position;
			if (Template.Picture == null)
				return PutValue(text);

			string value;
			if (Template.Composition != null)
			{
				const string Pad = "\uFFFF";
				value = Template.Picture.Replace(text, Pad + Template.Composition);
				if (!value.StartsWith(Pad, StringComparison.Ordinal))
				{
					PutValue(null);
					SetError($"Unrecognized format value of field {Template.Name}. ('{text}', pic:{Template.PictureText})", stream.GetPosition());
					return false;
				}
				value = value.Substring(1);
			}
			else if (!Template.Picture.IsMatch(text))
			{
				PutValue(null);
				SetError($"Unrecognized format value of field {Template.Name}. ('{text}', pic:{Template.PictureText})", stream.GetPosition());
				return false;
			}
			else
			{
				value = text;
			}

			if (PutValue(value))
				return true;

			PutValue(null);
			SetError($"Unrecognized format value of field {Template.Name}. ('{value}')", stream.GetPosition());
			return false;
		}
	}
}

