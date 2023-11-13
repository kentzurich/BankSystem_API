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
    public class TransferCash
    {
        public class Command : IRequest<Result<TransactionDTO>>
        {
            public Guid ReceiverAccountId { get; set; }
            public int Amount { get; set; } = 0;
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(x => x.ReceiverAccountId).NotEmpty();

                RuleFor(x => x.Amount)
                    .InclusiveBetween(100, 10000)
                    .WithMessage("You can transfer amount within 100 - 10,000 only.");
            }
        }

        public class Handler : IRequestHandler<Command, Result<TransactionDTO>>
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

            public async Task<Result<TransactionDTO>> Handle(Command request, CancellationToken cancellationToken)
            {
                var validationResult = _validator.Validate(request);

                if (validationResult.Errors.Count > 0)
                    return Result<TransactionDTO>.Failure(validationResult.Errors[0].ErrorMessage);

                var sender = await _context.UserAccounts
                    .FirstOrDefaultAsync(a => a.UserName == _userAccessor.GetUserName());

                sender.Balance -= request.Amount;

                //validate if amount has balance
                if (sender.Balance < 0)
                    return Result<TransactionDTO>.Failure("Insufficient funds.");

                var receiver = await _context.UserAccounts
                    .FirstOrDefaultAsync(a => a.AccountId == request.ReceiverAccountId);

                if(receiver is null) return null;

                if(sender.Id == receiver.Id) 
                    return Result<TransactionDTO>.Failure("Can't send amount to own account.");

                receiver.Balance += request.Amount;

                _context.UserAccounts.UpdateRange(sender, receiver);

                var result = await _context.SaveChangesAsync() > 0;

                var transaction = new TransactionDTO()
                {
                    SenderAccountId = sender.AccountId,
                    SenderName = $"{sender.FirstName} {sender.MiddleName} {sender.LastName}",
                    ReceiverAccountId = receiver.AccountId,
                    ReceiverName = $"{receiver.FirstName} {receiver.MiddleName} {receiver.LastName}",
                    Amount = request.Amount
                };

                if (!result) return Result<TransactionDTO>.Failure("Failed to process transaction.");

                return Result<TransactionDTO>.Success(transaction);
            }
        }
    }
}
