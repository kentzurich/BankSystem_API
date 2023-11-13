using Application.Account.DTO;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Account
{
    public class BalanceInquiry
    {
        public class Query : IRequest<Result<AccountDTO>>
        {
        }

        public class Handler : IRequestHandler<Query, Result<AccountDTO>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<AccountDTO>> Handle(Query request, CancellationToken cancellationToken)
            {
                var accountDetails = await _context.UserAccounts
                    .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUserName());

                return Result<AccountDTO>.Success(_mapper.Map<AccountDTO>(accountDetails));
            }
        }
    }
}
