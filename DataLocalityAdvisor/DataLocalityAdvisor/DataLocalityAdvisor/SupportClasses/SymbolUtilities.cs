using Microsoft.CodeAnalysis;

namespace DataLocalityAnalyzer.SupportClasses
{
    internal static class SymbolUtilities
    {
        public static bool CanBeStruct(INamedTypeSymbol symbol)
        {
            if (!IsNamedSymbolValidForConversion(symbol))
                return false;
            
            foreach (var member in symbol.GetMembers())
            {
                if (!IsValidMember(member))
                    return false;
            }

            return true;
        }

        private static  bool IsValidMember(ISymbol member)
        {
            switch (member.Kind)
            {
                case SymbolKind.Field:
                    if (!IsValidSpecialType(((IFieldSymbol) member).Type.SpecialType))
                        return false;
                    break;
                case SymbolKind.Property:
                    if (!IsValidSpecialType(((IPropertySymbol) member).Type.SpecialType))
                        return false;
                    break;
                case SymbolKind.Method:
                    if (!IsMethodSymbolValidForConversion(member as IMethodSymbol))
                        return false;
                    break;
                default:
                    return false;
            }

            return true;
        }

        private static bool IsValidSpecialType(SpecialType memberType)
        {
            switch (memberType)
            {
                case SpecialType.System_Enum:
                case SpecialType.System_Void:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_Array:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsNamedSymbolValidForConversion(INamedTypeSymbol namedSymbol)
        {
            if (namedSymbol.IsNamespace && namedSymbol.TypeKind != TypeKind.Class)
                return false;
            if (namedSymbol.IsStatic || namedSymbol.IsGenericType || namedSymbol.IsAbstract || namedSymbol.IsOverride || namedSymbol.IsVirtual)
                return false;
            if (namedSymbol.BaseType.Name != "Object")
                return false;
            
            return true;
        }

        private static bool IsMethodSymbolValidForConversion(IMethodSymbol member)
        {
            switch (member.MethodKind)
            {
                case MethodKind.Constructor:
                    return true;
            }
            return false;

        }
    }
}
