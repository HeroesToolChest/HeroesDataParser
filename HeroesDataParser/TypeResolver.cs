namespace HeroesDataParser;

// for spectre.console dependency injection
public sealed class TypeResolver(IServiceProvider provider) : ITypeResolver
{
    public object? Resolve(Type? type) => type is null ? null : provider.GetService(type);
}
