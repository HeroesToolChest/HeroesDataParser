namespace HeroesDataParser.Core;

public interface IJsonSerializerOptionService
{
    JsonSerializerOptions GeneralJsonSerializerOptions { get; }

    JsonSerializerOptions JsonSerializerDataOptions { get; }

    JsonSerializerOptions JsonSerializerGameStringOptions { get; }
}
