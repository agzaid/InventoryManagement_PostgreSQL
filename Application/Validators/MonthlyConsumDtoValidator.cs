using Application.Interfaces.Models;
using FluentValidation;

namespace Application.Validators
{
    public class MonthlyConsumDtoValidator : AbstractValidator<MonthlyConsumDto>
    {
        public MonthlyConsumDtoValidator()
        {
        }
    }
}
