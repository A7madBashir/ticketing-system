using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace TicketingSystem.Data.Converter;

public class DateTimeConverter(ConverterMappingHints mappingHints = null!)
    : ValueConverter<DateTime, DateTime>(
        convertToProviderExpression: src =>
            src.Kind == DateTimeKind.Utc ? src : DateTime.SpecifyKind(src, DateTimeKind.Utc),
        convertFromProviderExpression: dst =>
            dst.Kind == DateTimeKind.Utc ? dst : DateTime.SpecifyKind(dst, DateTimeKind.Utc),
        mappingHints: defaultHints.With(mappingHints)
    )
{
    private static readonly ConverterMappingHints defaultHints = new(size: 26);

    public DateTimeConverter()
        : this(null!) { }
}
