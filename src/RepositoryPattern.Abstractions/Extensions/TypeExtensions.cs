namespace RepositoryPattern.Abstractions.Extensions;

internal static class TypeExtensions
{
	public static bool IsImplementing(this Type type, Type interfaceType)
		=> type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
}