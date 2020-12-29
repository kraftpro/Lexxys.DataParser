// Lexxys Infrastructural library.
// file: Template.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lexxys.DataParsers.Templates
{
	using Xml;

	public class Template
	{
		public const string DefaultParserType = "fixed";
		public const string DefaultBol = @"";
		public const string DefaultEol = @"\n";
		public const string DefaultDelimiter = @",";

		private readonly string _name;
		private readonly string _description;
		private readonly IReadOnlyDictionary<string, Alias> _aliases;
		private readonly Dictionary<string, ReportTemplate> _reports;
		private readonly XmlLiteNode _configuration;

		//private static ConcurrentDictionary<Tuple<string, RegexOptions>, Regex> RegexMap;
		//static Template()
		//{
		//	RegexMap = new ConcurrentDictionary<Tuple<string, RegexOptions>, Regex>();
		//	RegexMap.TryAdd(Tuple.Create((string)null, RegexOptions.Compiled | RegexOptions.None), null);
		//	RegexMap.TryAdd(Tuple.Create((string)null, RegexOptions.Compiled | RegexOptions.Multiline), null);
		//	RegexMap.TryAdd(Tuple.Create((string)null, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture), null);
		//	RegexMap.TryAdd(Tuple.Create("", RegexOptions.Compiled | RegexOptions.None), null);
		//	RegexMap.TryAdd(Tuple.Create("", RegexOptions.Compiled | RegexOptions.Multiline), null);
		//	RegexMap.TryAdd(Tuple.Create("", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.ExplicitCapture), null);
		//}

		public Template(XmlLiteNode configuration)
		{
			if (configuration == null)
				throw EX.ArgumentNull(nameof(configuration));

			_name = configuration["name"];
			_description = configuration["description"];
			_configuration = configuration;
			var aliases = new Dictionary<string, Alias>(StringComparer.OrdinalIgnoreCase);
			foreach (var alias in configuration.Where("alias").Select(o => new Alias(o)))
			{
				aliases.Add(alias.Name, alias);
			}
			_aliases = ReadOnly.Wrap(aliases);
			_reports = new Dictionary<string, ReportTemplate>(StringComparer.OrdinalIgnoreCase);
		}

		public string Name => _name;

		public string Description => _description;

		public IReadOnlyDictionary<string, Alias> Aliases => _aliases;

		public ReportTemplate Report(string name)
		{
			lock (this)
			{
				if (_reports.TryGetValue(name, out ReportTemplate x))
					return x;
				XmlLiteNode node = _configuration.Where("report").FirstOrDefault(o => String.Equals(o["name"], name, StringComparison.OrdinalIgnoreCase));
				if (node == null)
					return null;
				var report = new ReportTemplate(this, node);
				_reports.Add(report.Name, report);
				return report;
			}
		}

		public Alias Alias(string name)
		{
			_aliases.TryGetValue(name, out Alias x);
			return x;
		}

		internal static Regex Regex(string picture, RegexOptions options = RegexOptions.None)
		{
			return String.IsNullOrEmpty(picture) ? null : new Regex(picture, options | RegexOptions.Compiled);
		}

		internal string SubstituteAliases(string text)
		{
			string composition = "";
			SubstituteAliases(ref text, ref composition);
			return text;
		}

		internal void SubstituteAliases(ref string text, ref string composition)
		{
			if (text == null)
				return;

			string comp = composition;
			text = System.Text.RegularExpressions.Regex.Replace(text, @"\$\(([^\)]*)\)", m =>
			{
				string name = m.Groups[1].Value;
				if (!_aliases.TryGetValue(name, out Alias a))
					return m.Value;
				if (comp == null)
					comp = a.Compositon;
				return a.Picture;
			});
			composition = comp;
		}
	}
}

