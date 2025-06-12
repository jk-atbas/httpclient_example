// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.SpacingRules",
		"SA1000:Keywords should be spaced correctly",
		Justification = "Not compatible with new new() declarations")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.SpacingRules",
		"SA1008:OpeningParenthesisMustBeSpacedCorrectly",
		Justification = "Not compatible with pattern matching syntax in a value tuple")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.SpacingRules",
		"SA1009:Closing parenthesis should be spaced correctly",
		Justification = "Not compatible with primary constructors")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.SpacingRules",
		"SA1010:Opening square brackets should be spaced correctly",
		Justification = "Not compatible with modern [] collection initialization")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.OrderingRules",
		"SA1200:Using directives should be placed correctly",
		Justification = "Don't need to appear within namespaces")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.OrderingRules",
		"SA1208:System using directives should be placed before other using directives",
		Justification = "Not necessary")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.NamingRules",
		"SA1313:Parameter names should begin with lower-case letter",
		Justification = "Not compatible with record struct auto properties in a primary constructor")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.DocumentationRules",
		"SA1623:Property summary documentation should match accessors",
		Justification = "Comments shouldn't need that, due to ide features covering this already")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.DocumentationRules",
		"SA1629:Documentation text should end with a period",
		Justification = "Don't see advantage of this")]

[assembly:
	SuppressMessage(
		"StyleCop.CSharp.DocumentationRules",
		"SA1633:File should have header",
		Justification = "Not intended as public library")]
