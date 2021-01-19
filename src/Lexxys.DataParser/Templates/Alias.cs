// Lexxys Infrastructural library.
// file: Alias.cs
//
// Copyright (c) 2001-2014, Kraft Pro Utilities.
// You may use this code under the terms of the MIT license
//
namespace Lexxys.DataParsers.Templates
{
	using Xml;

	public class Alias
	{
		public string Name { get; }
		public string Picture { get; }
		public string Compositon { get; }
		public string Description { get; }

		public Alias(string name, string picture, string composition, string description)
		{
			Name = name;
			Picture = picture;
			Compositon = composition;
			Description = description;
		}

		public Alias(XmlLiteNode node)
		{
			if (node == null)
				throw EX.ArgumentNull(nameof(node));

			Name = node["name"];
			Picture = node["picture"];
			Compositon = node["composition"];
			Description = node["description"];
		}
	}
}

