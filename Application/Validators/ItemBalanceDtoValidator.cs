using Application.Interfaces.Models;
using FluentValidation;

namespace Application.Validators
{
    public class ItemBalanceDtoValidator : AbstractValidator<ItemBalanceDto>
    {
        public ItemBalanceDtoValidator()
        {
        }
    }
}
