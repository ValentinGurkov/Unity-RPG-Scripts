using System;

public static class SerializeReferenceInspectorUtility {
    public static Type GetRealTypeFromTypename(string stringType) {
        var names = GetSplitNamesFromTypename(stringType);
        var realType = Type.GetType($"{names.ClassName}, {names.AssemblyName}");
        return realType;
    }
    public static(string AssemblyName, string ClassName) GetSplitNamesFromTypename(string typename) {
        if (string.IsNullOrEmpty(typename))
            return ("", "");

        var typeSplitString = typename.Split(char.Parse(" "));
        var typeClassName = typeSplitString[1];
        var typeAssemblyName = typeSplitString[0];
        return (typeAssemblyName, typeClassName);
    }
}
