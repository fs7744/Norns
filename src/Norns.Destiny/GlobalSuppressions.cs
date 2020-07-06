// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Major Code Smell", "S3925:\"ISerializable\" should be implemented correctly", Justification = "<挂起>", Scope = "type", Target = "~T:Norns.Destiny.AOP.Notations.ProxyGeneratorContext")]
[assembly: SuppressMessage("Major Code Smell", "S3881:\"IDisposable\" should be implemented correctly", Justification = "<挂起>", Scope = "type", Target = "~T:Norns.Destiny.Immutable.LazyImmutableArrayEnumerator`1")]
[assembly: SuppressMessage("Major Code Smell", "S1118:Utility classes should not have public constructors", Justification = "<挂起>", Scope = "type", Target = "~T:Norns.Destiny.RuntimeSymbol.SymbolCache`2")]