using FluentAssertions;
using Library.Api.Application.Services;
using Library.Api.Common.Exceptions;
using Library.Api.Contracts.Borrowings;
using Library.Api.Domain.Entities;
using Library.Api.Infrastructure.Repositories.Interfaces;
using Moq;

namespace Library.Api.Tests.Services;

public class BorrowingServiceTests
{
    private readonly Mock<IBookRepository> _bookRepository;
    private readonly Mock<IMemberRepository> _memberRepository;
    private readonly Mock<IBorrowingRepository> _borrowingRepository;

    public BorrowingServiceTests()
    {
        _bookRepository = new Mock<IBookRepository>();
        _memberRepository = new Mock<IMemberRepository>();
        _borrowingRepository = new Mock<IBorrowingRepository>();
    }

    [Fact]
    public async Task Borrow_Should_Throw_When_Member_Is_Inactive()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            AvailableCopies = 5
        };

        var member = new Member
        {
            Id = Guid.NewGuid(),
            IsActive = false
        };

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);

        _memberRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(member);

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        var request = new BorrowBookRequest(
            book.Id,
            member.Id);

        // Act
        Func<Task> action = () => service.BorrowAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage("Member is inactive");
    }

    [Fact]
    public async Task Borrow_Should_Throw_When_Book_Is_Unavailable()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            AvailableCopies = 0
        };

        var member = new Member
        {
            Id = Guid.NewGuid(),
            IsActive = true
        };

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);

        _memberRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(member);

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        var request = new BorrowBookRequest(
            book.Id,
            member.Id);

        // Act
        Func<Task> action = () => service.BorrowAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage("Book is unavailable");
    }

    [Fact]
    public async Task Borrow_Should_Throw_When_Limit_Is_Exceeded()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            AvailableCopies = 5
        };

        var member = new Member
        {
            Id = Guid.NewGuid(),
            IsActive = true
        };

        var activeBorrowings = new List<Borrowing>
        {
            new(),
            new(),
            new()
        };

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);

        _memberRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(member);

        _borrowingRepository
            .Setup(x => x.GetActiveBorrowingsByMemberAsync(It.IsAny<Guid>()))
            .ReturnsAsync(activeBorrowings);

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        var request = new BorrowBookRequest(
            book.Id,
            member.Id);

        // Act
        Func<Task> action = () => service.BorrowAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage("Member borrowing limit exceeded");
    }

    [Fact]
    public async Task Return_Should_Throw_When_Book_Is_Already_Returned()
    {
        // Arrange
        var borrowing = new Borrowing
        {
            Id = Guid.NewGuid(),
            ReturnedDate = DateTime.UtcNow
        };

        _borrowingRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(borrowing);

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        // Act
        Func<Task> action =
            () => service.ReturnAsync(borrowing.Id);

        // Assert
        await action.Should()
            .ThrowAsync<BusinessRuleException>()
            .WithMessage("Book already returned");
    }

    [Fact]
    public async Task Borrow_Should_Decrease_Available_Copies()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            AvailableCopies = 5
        };

        var member = new Member
        {
            Id = Guid.NewGuid(),
            IsActive = true
        };

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);

        _memberRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(member);

        _borrowingRepository
            .Setup(x => x.GetActiveBorrowingsByMemberAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new List<Borrowing>());

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        var request = new BorrowBookRequest(
            book.Id,
            member.Id);

        // Act
        await service.BorrowAsync(request);

        // Assert
        book.AvailableCopies.Should().Be(4);
    }

    [Fact]
    public async Task Borrow_Should_Throw_When_Book_Not_Found()
    {
        // Arrange
        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Book?)null);

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        var request = new BorrowBookRequest(
            Guid.NewGuid(),
            Guid.NewGuid());

        // Act
        Func<Task> action = () => service.BorrowAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Book not found");
    }

    [Fact]
    public async Task Borrow_Should_Throw_When_Member_Not_Found()
    {
        // Arrange
        var book = new Book
        {
            Id = Guid.NewGuid(),
            AvailableCopies = 5
        };

        _bookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(book);

        _memberRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Member?)null);

        var service = new BorrowingService(
            _bookRepository.Object,
            _memberRepository.Object,
            _borrowingRepository.Object);

        var request = new BorrowBookRequest(
            book.Id,
            Guid.NewGuid());

        // Act
        Func<Task> action = () => service.BorrowAsync(request);

        // Assert
        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage("Member not found");
    }
}