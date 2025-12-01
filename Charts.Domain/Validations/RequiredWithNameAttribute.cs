using System.ComponentModel.DataAnnotations;

namespace Charts.Domain.Validations
{
    public class RequiredWithNameAttribute : ValidationAttribute
    {
        private readonly string _namePropertyName;

        public RequiredWithNameAttribute(string namePropertyName = "Name")
        {
            _namePropertyName = namePropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Проверяем, что значение заполнено
            if (value == null || (value is string str && string.IsNullOrWhiteSpace(str)))
            {
                // Получаем значение свойства Name
                var nameProperty = validationContext.ObjectType.GetProperty(_namePropertyName);
                var nameValue = nameProperty?.GetValue(validationContext.ObjectInstance) as string;

                var fieldName = string.IsNullOrWhiteSpace(nameValue)
                    ? "поля"
                    : $"'{nameValue}'";

                var displayName = validationContext.DisplayName ?? validationContext.MemberName ?? "поле";

                return new ValidationResult(
                    $"Поле {fieldName} → {displayName} обязательно для заполнения",
                    new[] { validationContext.MemberName ?? string.Empty }
                );
            }

            return ValidationResult.Success;
        }
    }
}
