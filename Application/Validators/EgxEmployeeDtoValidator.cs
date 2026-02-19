using Application.Interfaces.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class EgxEmployeeDtoValidator : AbstractValidator<EgxEmployeeDto>
    {
        public EgxEmployeeDtoValidator()
        {
            // Rule for Name is always applied (for both Create and Update)
            //RuleFor(p => p.DepDesc)
            //    .NotEmpty().WithMessage("Name is required.")
            //    .NotNull()
            //    .MaximumLength(50).WithMessage("Name must not exceed 50 characters.");

            //// RuleSet for Update: Only runs when explicitly told to by the calling code.
            //RuleSet("Update", () =>
            //{
            //    RuleFor(p => p.DepCode)
            //        .GreaterThan(0).WithMessage("Department ID must be provided for an update.");
            //});
        }
    }
}
