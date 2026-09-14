namespace HeroesDataParser.Core;

public interface IPortraitExtractService
{
    Task Extract();

    void DisplayAvailablePortraits();
}
