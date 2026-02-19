using Application.Interfaces.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class HInvTransDtoValidator : AbstractValidator<HInvTransDto>
    {
        public HInvTransDtoValidator()
        {
        }
    }
}
