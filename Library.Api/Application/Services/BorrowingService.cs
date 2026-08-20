using Library.Api.Application.Interfaces;
using Library.Api.Contracts.Borrowings;
using Library.Application.Abstractions.Repositories;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.Domain.Exceptions;

namespace Library.Api.Application.Services;
public class BorrowingService : IBorrowingService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IBorrowingRepository _borrowingRepository;

    public BorrowingService(
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IBorrowingRepository borrowingRepository)
    {
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _borrowingRepository = borrowingRepository;
    }
   
    
    public async Task<List<BorrowingResponse>>
        GetAllAsync()
    {
        var borrowings =
            await _borrowingRepository
                .GetAllAsync();

        return borrowings
            .Select(x =>
                new BorrowingResponse(
                    x.Id,
                    x.BookId,
                    x.MemberId,
                    x.BorrowedDate,
                    x.DueDate,
                    x.ReturnedDate,
                    x.Status.ToString()))
            .ToList();
    }
    public async Task<List<BorrowingResponse>>
        GetByMemberAsync(Guid memberId)
    {
        var borrowings =
            await _borrowingRepository
                .GetByMemberIdAsync(memberId);

        return borrowings
            .Select(x =>
                new BorrowingResponse(
                    x.Id,
                    x.BookId,
                    x.MemberId,
                    x.BorrowedDate,
                    x.DueDate,
                    x.ReturnedDate,
                    x.Status.ToString()))
            .ToList();
    }
    
}