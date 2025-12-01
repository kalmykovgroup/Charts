using System.Text.Json.Serialization;

namespace Charts.Domain.Contracts.Types
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FilterOp
    {
        /// <summary>Равно (=)</summary>
        [JsonStringEnumMemberName("Eq")] Eq,

        /// <summary>Не равно (&lt;&gt;)</summary>
        [JsonStringEnumMemberName("Ne")] Ne,

        /// <summary>Строго больше (&gt;)</summary>
        [JsonStringEnumMemberName("Gt")] Gt,

        /// <summary>Больше либо равно (&gt;=)</summary>
        [JsonStringEnumMemberName("Gte")] Gte,

        /// <summary>Строго меньше (&lt;)</summary>
        [JsonStringEnumMemberName("Lt")] Lt,

        /// <summary>Меньше либо равно (&lt;=)</summary>
        [JsonStringEnumMemberName("Lte")] Lte,

        /// <summary>Между значениями, включительно (SQL: BETWEEN a AND b)</summary>
        [JsonStringEnumMemberName("Between")] Between,

        /// <summary>Принадлежит множеству (SQL: IN)</summary>
        [JsonStringEnumMemberName("In")] In,

        /// <summary>Не принадлежит множеству (SQL: NOT IN)</summary>
        [JsonStringEnumMemberName("Nin")] Nin,

        /// <summary>LIKE (регистрозависимый шаблон: %, _)</summary>
        [JsonStringEnumMemberName("Like")] Like,

        /// <summary>ILIKE (регистронезависимый шаблон)</summary>
        [JsonStringEnumMemberName("ILike")] ILike,

        /// <summary>IS NULL</summary>
        [JsonStringEnumMemberName("IsNull")] IsNull,

        /// <summary>IS NOT NULL</summary>
        [JsonStringEnumMemberName("NotNull")] NotNull
    }

}
