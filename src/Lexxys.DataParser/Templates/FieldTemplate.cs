// Lexxys Infrastructural library.
// file: FieldTemplate.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using System;
using System.Collections.Generic;
using Lexxys.Xml;
using System.Text.RegularExpressions;

namespace Lexxys.DataParsers.Templates
{

	public class FieldTemplate
	{
		public string Name { get; }
		public string Description { get; }
		public int Position { get; }
		public int Length { get; }
		public int Order { get; }
		public string PictureText { get; }
		public string Composition { get; }
		public string DefaultValue { get; }
		public Type ValueType { get; }
		public Regex Picture { get; }
		public IReadOnlyDictionary<string, Alias> Aliases { get; }

		static FieldTemplate()
		{
			Name2Type = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
			{
				{ "bool", typeof(int?) },		
				{ "ternary", typeof(Ternary) },	
				{ "byte", typeof(byte?) },		
				{ "sbyte", typeof(sbyte?) },	
				{ "short", typeof(short?) },	
				{ "ushort", typeof(ushort?) },	
				{ "int", typeof(int?) },		
				{ "integer", typeof(int?) },		
				{ "uint", typeof(uint?) },		
				{ "long", typeof(long?) },		
				{ "ulong", typeof(ulong?) },	
				{ "decimal", typeof(decimal?) },	
				{ "currency", typeof(decimal?) },
				{ "money", typeof(decimal?) },
				{ "fixed", typeof(decimal?) },	
				{ "float", typeof(double?) },	
				{ "double", typeof(double?) },	
				{ "single", typeof(float?) },	
				{ "string", typeof(string) },	
				{ "datetime", typeof(DateTime?) },	
				{ "date", typeof(DateTime?) },	
				{ "timespan", typeof(TimeSpan?) },	
				{ "time", typeof(TimeSpan?) },	
				{ "guid", typeof(Guid?) },		
				{ "type", typeof(Type) },		
				{ "object", typeof(object) },	
				{ "variant", typeof(object) },	
				{ "void", typeof(void) },		
			};
		}

		public FieldTemplate(RecordTemplate record, XmlLiteNode node)
		{
			if (record == null)
				throw EX.ArgumentNull("record");
			if (node == null)
				throw EX.ArgumentNull("node");

			Aliases = record.Aliases;
			Name = node["name"];
			Description = node["description"];
			Position = node["position"].AsInt32(0) - 1;
			Length = node["length"].AsInt32(0);
			Order = node["order"].AsInt32(0);
			DefaultValue = node["default"].TrimToNull();
			ValueType = Name2Type[node["type"].TrimToNull() ?? "object"];

			var text = node["picture"];
			var composition = node["composition"];
			record.Report.Template.SubstituteAliases(ref text, ref composition);
			PictureText = Engines.Engine.CleanPicure(text);
			Composition = Engines.Engine.CleanComposition(composition);
			Picture = Template.Regex(PictureText);
		}

		private static readonly Dictionary<string, Type> Name2Type;
	}
}

