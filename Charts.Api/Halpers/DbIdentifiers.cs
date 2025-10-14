namespace Charts.Api.Halpers
{
    public static class DbIdentifiers
    {
        public static (string Schema, string Table) ParseEntityName(string entity, string defaultSchema)
        {
            if (string.IsNullOrWhiteSpace(entity))
                throw new InvalidOperationException("Entity name is empty");

            var s = entity.Trim();

            // "Schema"."Table" или "Table"
            if (s[0] == '"')
            {
                var parts = new List<string>();
                int i = 0;
                while (i < s.Length)
                {
                    if (s[i] == '"')
                    {
                        int j = ++i;
                        while (j < s.Length && s[j] != '"') j++;
                        if (j >= s.Length) throw new InvalidOperationException("Unclosed quote in entity name.");
                        parts.Add(s.Substring(i, j - i));
                        i = j + 1;
                    }
                    else if (s[i] == '.' || char.IsWhiteSpace(s[i])) i++;
                    else throw new InvalidOperationException("Bad quoted entity format.");
                }
                return parts.Count switch
                {
                    1 => (defaultSchema, parts[0]),
                    2 => (parts[0], parts[1]),
                    _ => throw new InvalidOperationException("Bad quoted entity format.")
                };
            }

            // Schema.Table или Table
            var dot = s.IndexOf('.');
            if (dot >= 0)
            {
                var schema = s[..dot].Trim();
                var table = s[(dot + 1)..].Trim();
                if (schema.Length == 0 || table.Length == 0)
                    throw new InvalidOperationException("Bad unquoted entity format (Schema.Table).");
                return (schema, table);
            }

            return (defaultSchema, s);
        }
    }

}
