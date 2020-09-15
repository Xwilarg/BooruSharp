using System;
using System.Runtime.CompilerServices;

[assembly: CLSCompliant(true)]

// This will make members marked with "internal" access
// modifiers accessible from these assemblies/projects.
[assembly: InternalsVisibleTo("BooruSharp.Others")]
[assembly: InternalsVisibleTo("BooruSharp.UnitTests")]
