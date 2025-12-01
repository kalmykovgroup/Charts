namespace Charts.Domain.Contracts.Metadata.Dtos
{
    public class EntityDto
    {
        public string Name { get; set; } = string.Empty;
        public List<FieldDto> Fields { get; set; } = [];

        public EntityDto()
        {
        }
        public EntityDto(string name)
        {
            Name = name;
        }
        public EntityDto(string name, List<FieldDto> fields) : this(name) 
        {
            Fields = fields;
        }



    }
}
