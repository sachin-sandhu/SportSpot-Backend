using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SportSpot_Test
{
    internal static class ResourceHelper
    {
        internal static Stream GetEmbeddedFileAsStream(string name)
        {
            Assembly asm = Assembly.GetCallingAssembly();
            return asm.GetManifestResourceStream(name) ?? throw new FileNotFoundException($"Resource File {name} not found!");
        }
    }
}
