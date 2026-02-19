using Application.Interfaces.Models;
using FluentValidation;

namespace Application.Validators
{
    public class MonthlyBalanceDtoValidator : AbstractValidator<MonthlyBalanceDto>
    {
        public MonthlyBalanceDtoValidator()
        {
        }
    }
}
