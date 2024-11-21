using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using U8Xml;

namespace HeroesDataParser.Benchmarks;

[MemoryDiagnoser]
public class Benchmarks
{
    [Benchmark]
    public bool WithStringCompare()
    {
        string test = "starcraFT";

        if (test.Equals("diablo", StringComparison.OrdinalIgnoreCase))
            return true;
        if (test.Equals("starcraft", StringComparison.OrdinalIgnoreCase))
            return true;
        else
            return false;
    }

    [Benchmark]
    public bool WithAlloc()
    {
        string test = "starcraFT";

        Span<char> buffer = stackalloc char[test.Length];

        test.AsSpan().ToUpperInvariant(buffer);

        if (buffer.SequenceEqual("diablo"))
            return true;
        if (buffer.SequenceEqual("starcraft"))
            return true;
        else
            return false;
    }
    //[Benchmark]
    //public string WithStackAlloc()
    //{
    //    using XmlObject xmlDataTag = XmlParser.Parse("<d const=\"$YrelSacredGroundArmorBonus\" precision=\"2\"/>");
    //    XmlNode xmlRoot = xmlDataTag.Root;
    //    XmlAttributeList xmlAttributes = xmlRoot.Attributes;


    //    if (xmlAttributes.TryFind("const", out XmlAttribute constAttribute) || xmlAttributes.TryFind("CONST", out constAttribute))
    //    {
    //        Span<char> buffer = stackalloc char[constAttribute.Value.GetCharCount()];

    //        bool asdf = System.Text.UTF8Encoding.UTF8.TryGetChars(constAttribute.Value.AsSpan(), buffer, out int charsWritten);

    //        if (Test(buffer, out string output))
    //        {
    //            return output;
    //        }
    //    }

    //    throw new Exception("oops");
    //}

    //[Benchmark]
    //public string WithString()
    //{
    //    using XmlObject xmlDataTag = XmlParser.Parse("<d const=\"$YrelSacredGroundArmorBonus\" precision=\"2\"/>");
    //    XmlNode xmlRoot = xmlDataTag.Root;
    //    XmlAttributeList xmlAttributes = xmlRoot.Attributes;

    //    if (xmlAttributes.TryFind("const", out XmlAttribute constAttribute) || xmlAttributes.TryFind("CONST", out constAttribute))
    //    {
    //        if (Test(constAttribute.Value.ToString(), out string output))
    //        {
    //            return output;
    //        }
    //    }

    //    throw new Exception("oops");
    //}

    //private bool Test(ReadOnlySpan<char> buffer, out string output)
    //{
    //    output = buffer.ToString();

    //    return true;
    //}

    //private bool Test(string buffer, out string output)
    //{
    //    output = buffer;

    //    return true;
    //}
}
