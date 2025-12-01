using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Charts.Domain.Contracts.Types;
using Charts.Domain.Validations;

namespace Charts.Domain.Contracts.Metadata.Dtos
{
    public class FieldDto
    {
        public string Name { get; set; } = string.Empty;

        public int VisualOrder { get; set; } = 0;


        [RequiredWithName] // ← автоматически подставит Name из этого же объекта
        [MinLength(1, ErrorMessage = "Тип поля не может быть пустым")]
        public string Type { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SqlParamType? SqlParamType = null; // нормализованный тип для параметров/фильтров, если можем определить
        public bool? IsNumeric { get; set; } = null;
        public bool? IsTime { get; set; } = null;



        public FieldDto()
        {

        }

        public FieldDto(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public FieldDto(string name, string type, int visualOrder): this(name, type) 
        { 
            VisualOrder = visualOrder;
        }

        public FieldDto(string name, string type, SqlParamType? sqlParamType, bool? isNumeric, bool? isTime)
        {
            Name = name;
            Type = type;
            IsNumeric = isNumeric;
            IsTime = isTime;
            SqlParamType = sqlParamType;
        }
    }
}
