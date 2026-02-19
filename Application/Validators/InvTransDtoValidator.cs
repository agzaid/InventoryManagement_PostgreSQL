using Application.Interfaces.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Validators
{
    public class InvTransDtoValidator : AbstractValidator<InvTransDto>
    {
        public InvTransDtoValidator()
        {
            //RuleFor(x => x.UserName)
            //.NotEmpty().WithMessage("اسم المستخدم مطلوب")
            //.MaximumLength(10).WithMessage("الحد الأقصى لاسم المستخدم 10 أحرف");

            //RuleFor(x => x.UserPasswd)
            //    .NotEmpty().WithMessage("يجب إدخال كلمة المرور")
            //    .Length(6, 16).WithMessage("كلمة المرور يجب أن تكون بين 6 و 16 خانة");
        }
    }
}
