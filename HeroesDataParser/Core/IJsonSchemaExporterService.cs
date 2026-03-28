namespace HeroesDataParser.Core;

public interface IJsonSchemaExporterService
{
    Task ExportDataSchema();

    Task ExportGameStringSchema();
}
