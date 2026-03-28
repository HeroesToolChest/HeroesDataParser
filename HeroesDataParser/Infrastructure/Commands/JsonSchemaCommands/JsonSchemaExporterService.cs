using System.Text.Json.Schema;

namespace HeroesDataParser.Infrastructure.Commands.JsonSchemaCommands;

public class JsonSchemaExporterService : IJsonSchemaExporterService
{
    private readonly ILogger<JsonSchemaExporterService> _logger;
    private readonly JsonSchemaExportOptions _options;
    private readonly IAnsiConsole _console;
    private readonly IJsonSerializerOptionService _jsonSerializerOptionService;

    private readonly JsonSerializerOptions _jsonSerializerOptions;
    private readonly JsonSchemaExporterOptions _jsonSchemaExporterOptions;

    public JsonSchemaExporterService(
        ILogger<JsonSchemaExporterService> logger,
        IOptions<JsonSchemaExportOptions> options,
        IAnsiConsole console,
        IJsonSerializerOptionService jsonSerializerOptionService)
    {
        _logger = logger;
        _options = options.Value;
        _console = console;
        _jsonSerializerOptionService = jsonSerializerOptionService;
        _jsonSerializerOptions = new(_jsonSerializerOptionService.GeneralJsonSerializerOptions)
        {
            TypeInfoResolver = new HeroesElementResolver()
            {
                Modifiers =
                {
                    typeInfo => JsonTypeInfoModifiers.SerializationModifiers(typeInfo, LocalizedTextOption.None, []),
                },
            },
            NumberHandling = JsonNumberHandling.Strict,
        };
        _jsonSerializerOptions.Converters.Add(new GameStringTextConverter(new GameStringTextConverterOptions() { GameStringTextType = GameStringTextType.RawText }));

        _jsonSchemaExporterOptions = GetJsonSchemaExporterOptions();
    }

    public async Task ExportDataSchema()
    {
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Hero))
            CreateDataSchemaFile<Hero>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Unit))
            CreateDataSchemaFile<Unit>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.MatchAward))
            CreateDataSchemaFile<MatchAward>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Skin))
            CreateDataSchemaFile<Skin>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Mount))
            CreateDataSchemaFile<Mount>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Banner))
            CreateDataSchemaFile<Banner>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Spray))
            CreateDataSchemaFile<Spray>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Announcer))
            CreateDataSchemaFile<Announcer>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.VoiceLine))
            CreateDataSchemaFile<VoiceLine>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.PortraitPack))
            CreateDataSchemaFile<PortraitPack>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.RewardPortrait))
            CreateDataSchemaFile<RewardPortrait>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Emoticon))
            CreateDataSchemaFile<Emoticon>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.EmoticonPack))
            CreateDataSchemaFile<EmoticonPack>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Veterancy))
            CreateDataSchemaFile<Veterancy>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Bundle))
            CreateDataSchemaFile<Bundle>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Boost))
            CreateDataSchemaFile<Boost>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.LootChest))
            CreateDataSchemaFile<LootChest>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.TypeDescription))
            CreateDataSchemaFile<TypeDescription>();
        if (_options.ExtractDataOptions.HasFlag(ExtractDataOptions.Map))
            CreateDataSchemaFile<Map>();
    }

    public async Task ExportGameStringSchema()
    {
        CreateGameStringSchemaFile();
    }

    private static (Type KeyType, Type ValueType)? GetDictionaryTypes(Type type)
    {
        foreach (Type candidate in type.GetInterfaces().Prepend(type))
        {
            if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                Type[] args = candidate.GetGenericArguments();
                return (args[0], args[1]);
            }
        }

        return null;
    }

    private static JsonObject? GetPrimitiveSchemaType(Type type) => type switch
    {
        _ when type == typeof(double) || type == typeof(float) || type == typeof(decimal) => new JsonObject { ["type"] = "number" },
        _ when type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte) => new JsonObject { ["type"] = "integer" },
        _ when type == typeof(string) => new JsonObject { ["type"] = "string" },
        _ when type == typeof(bool) => new JsonObject { ["type"] = "boolean" },
        _ => null,
    };

    private static JsonSchemaExporterOptions GetJsonSchemaExporterOptions()
    {
        return new JsonSchemaExporterOptions()
        {
            TransformSchemaNode = (context, schema) =>
            {
                Type resolvedType = Nullable.GetUnderlyingType(context.TypeInfo.Type) ?? context.TypeInfo.Type;

                if (resolvedType == typeof(GameStringText) || resolvedType.IsAssignableTo(typeof(LinkId)))
                {
                    bool isNullable = context.PropertyInfo?.IsGetNullable ?? false;

                    if (isNullable)
                    {
                        return new JsonObject
                        {
                            ["type"] = new JsonArray(
                                JsonValue.Create("string"),
                                JsonValue.Create("null")),
                        };
                    }

                    return new JsonObject
                    {
                        ["type"] = "string",
                    };
                }
                else if (resolvedType.IsAssignableTo(typeof(IEnumerable<GameStringText>)) || resolvedType.IsAssignableTo(typeof(IEnumerable<AbilityLinkId>)) || resolvedType.IsAssignableTo(typeof(IEnumerable<TalentLinkId>)))
                {
                    bool isNullable = context.PropertyInfo?.IsGetNullable ?? false;

                    if (isNullable)
                    {
                        return new JsonObject
                        {
                            ["type"] = new JsonArray(
                                JsonValue.Create("array"),
                                JsonValue.Create("null")),
                            ["items"] = new JsonObject
                            {
                                ["type"] = "string",
                            },
                        };
                    }

                    return new JsonObject
                    {
                        ["type"] = "array",
                        ["items"] = new JsonObject
                        {
                            ["type"] = "string",
                        },
                    };
                }
                else if (resolvedType == typeof(HeroesDataVersion))
                {
                    bool isNullable = context.PropertyInfo?.IsGetNullable ?? false;

                    if (isNullable)
                    {
                        return new JsonObject
                        {
                            ["type"] = new JsonArray(
                                JsonValue.Create("string"),
                                JsonValue.Create("null")),
                        };
                    }

                    return new JsonObject
                    {
                        ["type"] = "string",
                    };
                }
                else if (resolvedType == typeof(int))
                {
                    bool isNullable = context.PropertyInfo?.IsGetNullable ?? false;

                    if (isNullable)
                    {
                        return new JsonObject
                        {
                            ["type"] = new JsonArray(
                                JsonValue.Create("integer"),
                                JsonValue.Create("null")),
                        };
                    }

                    return new JsonObject
                    {
                        ["type"] = "integer",
                    };
                }
                else if (resolvedType == typeof(double))
                {
                    bool isNullable = context.PropertyInfo?.IsGetNullable ?? false;

                    if (isNullable)
                    {
                        return new JsonObject
                        {
                            ["type"] = new JsonArray(
                                JsonValue.Create("number"),
                                JsonValue.Create("null")),
                        };
                    }

                    return new JsonObject
                    {
                        ["type"] = "number",
                    };
                }
                else if (resolvedType == typeof(string))
                {
                    bool isNullable = context.PropertyInfo?.IsGetNullable ?? false;

                    if (isNullable)
                    {
                        return new JsonObject
                        {
                            ["type"] = new JsonArray(
                                JsonValue.Create("string"),
                                JsonValue.Create("null")),
                        };
                    }

                    return new JsonObject
                    {
                        ["type"] = "string",
                    };
                }
                else if (GetDictionaryTypes(resolvedType) is (Type dictKeyType, Type dictValueType))
                {
                    if (schema is JsonObject dictSchema)
                    {
                        // Add propertyNames for enum keys, keeping it before additionalProperties
                        if (dictKeyType.IsEnum)
                        {
                            JsonArray enumValues = [.. Enum.GetNames(dictKeyType)];

                            if (dictSchema.Remove("additionalProperties", out JsonNode? existingAdditionalProps))
                            {
                                dictSchema["propertyNames"] = new JsonObject
                                {
                                    ["enum"] = enumValues,
                                };
                                dictSchema["additionalProperties"] = existingAdditionalProps;
                            }
                            else
                            {
                                dictSchema["propertyNames"] = new JsonObject
                                {
                                    ["enum"] = enumValues,
                                };
                            }
                        }

                        // Add additionalProperties for the value type if missing
                        if (!dictSchema.ContainsKey("additionalProperties") && GetPrimitiveSchemaType(dictValueType) is JsonObject valueSchema)
                        {
                            dictSchema["additionalProperties"] = valueSchema;
                        }
                    }
                }

                // Strip "null" from type arrays when the context indicates non-nullable
                bool isContextNullable = context.PropertyInfo?.IsGetNullable ?? false;
                if (!isContextNullable && schema is JsonObject schemaObject && schemaObject.TryGetPropertyValue("type", out JsonNode? typeNode) && typeNode is JsonArray typeArray)
                {
                    for (int i = typeArray.Count - 1; i >= 0; i--)
                    {
                        if (typeArray[i]?.GetValue<string>() == "null")
                            typeArray.RemoveAt(i);
                    }

                    if (typeArray.Count == 1)
                        schemaObject["type"] = typeArray[0]!.GetValue<string>();
                }

                return schema;
            },
        };
    }

    private void CreateGameStringSchemaFile()
    {
        JsonNode schema = _jsonSerializerOptions.GetJsonSchemaAsNode(typeof(RootJsonGameStringElement), _jsonSchemaExporterOptions);

        CreateSchemaFile(schema, "gamestrings", "gamestrings");
    }

    private void CreateDataSchemaFile<T>()
        where T : IElementObject
    {
        JsonNode schema = _jsonSerializerOptions.GetJsonSchemaAsNode(typeof(RootDataElement<T>), _jsonSchemaExporterOptions);
        string dataTypeName = typeof(T).Name.ToLowerInvariant();

        CreateSchemaFile(schema, dataTypeName, $"{dataTypeName}data");
    }

    private void CreateSchemaFile(JsonNode schema, string dataTypeName, string fileNamePrefix)
    {
        string outputFilePath = Path.Combine(_options.OutputDirectory, $"{fileNamePrefix}_{_options.Version}.schema.json");

        if (File.Exists(outputFilePath) && !_options.AllowOverwrite)
        {
            _logger.LogError("Output file already exists and overwrite is not allowed: {OutputFilePath} for {DataTypeName}", outputFilePath, dataTypeName);
            _console.MarkupLine($"[red]Could not export '{dataTypeName}' JSON schema, output file already exists: {outputFilePath}[/]");
            return;
        }

        Directory.CreateDirectory(_options.OutputDirectory);

        using FileStream fileStream = new(outputFilePath, FileMode.Create, FileAccess.Write);
        using Utf8JsonWriter writer = new(fileStream, new JsonWriterOptions
        {
            Indented = _options.JsonIndent,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });

        schema.WriteTo(writer, _jsonSerializerOptions);

        _logger.LogInformation("Exported '{TypeName}' JSON schema to {OutputFilePath}", dataTypeName, outputFilePath);
        _console.MarkupLine($"Exported '{dataTypeName}' JSON schema to {outputFilePath}");
    }
}
