namespace RepositoryPattern.Abstractions.Extensions;

internal static class TypeExtensions
{
	public static bool IsImplementingRepository(this Type type, Type interfaceType)
		=> type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
	
	public static bool IsImplementingUnitOfWork(this Type type, Type interfaceType)
		=> type.GetInterfaces().Any(i => i == interfaceType);
}