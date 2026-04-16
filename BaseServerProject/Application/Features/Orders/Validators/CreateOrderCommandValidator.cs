using FluentValidation;
using BaseServerProject.Application.Features.Orders.Commands;
using BaseServerProject.Application.Features.Orders.DTOs;

namespace BaseServerProject.Application.Features.Orders.Validators
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Phương thức thanh toán không được để trống");

            RuleFor(x => x.Items)
                .NotNull()
                .WithMessage("Danh sách sản phẩm không được null")
                .NotEmpty()
                .WithMessage("Giỏ hàng không được trống");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.VariantID)
                    .GreaterThan(0)
                    .WithMessage("VariantID không hợp lệ");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Số lượng phải lớn hơn 0");

                item.RuleFor(x => x.UnitPrice)
                    .GreaterThan(0)
                    .WithMessage("Đơn giá phải lớn hơn 0");

                item.RuleFor(x => x.DiscountAmount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Giảm giá item không hợp lệ");
            });

            RuleFor(x => x.DiscountAmount)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Giảm giá không hợp lệ");
        }
    }
}