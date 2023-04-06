using System;

/// This utility exists, because serialize reference managed reference typename returns combined string
/// and not data class that contains separate strings for assembly name and for class name (and possibly namespace name)
public static class SerializeReferenceTypeNameUtility
{
    public static Type GetRealTypeFromTypename(string stringType)
    {
        (string assemblyName, string className) = GetSplitNamesFromTypename(stringType);
        var realType = Type.GetType($"{className}, {assemblyName}");
        return realType;
    }

    public static (string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename)
    {
        if (string.IsNullOrEmpty(typename))
            return ("", "");

        string[] typeSplitString = typename.Split(char.Parse(" "));
        string typeClassName = typeSplitString[1];
        string typeAssemblyName = typeSplitString[0];
        return (typeAssemblyName, typeClassName);
    }
}