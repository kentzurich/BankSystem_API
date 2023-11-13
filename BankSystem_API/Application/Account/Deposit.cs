using Application.Account.DTO;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using DataAccess;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Account
{
    public class Deposit
    {
        public class Command : IRequest<Result<AccountDTO>>
        {
            public int Amount { get; set; } = 0;
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator() 
            {
                RuleFor(x => x.Amount)
                    .InclusiveBetween(100, 10000)
                    .WithMessage("You can deposit amount within 100 - 10,000 only.");
            }
        }

        public class Handler : IRequestHandler<Command, Result<AccountDTO>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;
            private readonly IValidator<Command> _validator;

            public Handler(DataContext context, 
                           IMapper mapper, 
                           IUserAccessor userAccessor, 
                           IValidator<Command> validator)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
                _validator = validator;
            }

            public async Task<Result<AccountDTO>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);

                if(validationResult.Errors.Count > 0)
                    return Result<AccountDTO>.Failure(validationResult.Errors[0].ErrorMessage);

                var userAccount = await _context.UserAccounts
                    .FirstOrDefaultAsync(u => u.UserName == _userAccessor.GetUserName());

                userAccount.Balance += request.Amount;

                _context.UserAccounts.Update(userAccount);

                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<AccountDTO>.Failure("Failed to deposit cash.");

                return Result<AccountDTO>.Success(_mapper.Map<AccountDTO>(userAccount));
            }
        }
    }
}
