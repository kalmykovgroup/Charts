using Charts.Api.Application.Contracts.Metadata.Dtos;

namespace Charts.Api.Halpers
{
    public static class Resolve 
    {
        private static string Normalize(string s) => s.Replace("_", "", StringComparison.Ordinal).ToLowerInvariant();

        public static FieldDto ResolveField(IReadOnlyList<FieldDto> fields, string logicalName)
        {
            // 1) точное совпадение
            var f = fields.FirstOrDefault(x => x.Name == logicalName);
            if (f != null) return f;

            // 2) регистронезависимое
            f = fields.FirstOrDefault(x => x.Name.Equals(logicalName, StringComparison.OrdinalIgnoreCase));
            if (f != null) return f;

            // 3) fuzzy: игнор подчёркиваний + регистра
            var n = Normalize(logicalName);
            var matches = fields.Where(x => Normalize(x.Name) == n).ToList();

            if (matches.Count == 1) return matches[0];
            if (matches.Count > 1) throw new InvalidOperationException(
                $"Ambiguous column '{logicalName}'. Candidates: {string.Join(", ", matches.Select(m => m.Name))}");

            throw new InvalidOperationException($"Unknown column '{logicalName}'. Available: {string.Join(", ", fields.Select(m => m.Name))}");
        }

    }
}
