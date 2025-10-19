namespace HeroesDataParser.Core;

public interface IJsonSerializerOptionService
{
    JsonSerializerOptions JsonSerializerDataOptions { get; }

    JsonSerializerOptions JsonSerializerGameStringOptions { get; }
}
