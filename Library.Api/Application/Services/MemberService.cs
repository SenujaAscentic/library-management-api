using Library.Api.Application.contracts.Members;
using Library.Api.Application.Interfaces;
using Library.Api.Contracts.Members;
using Library.Api.Domain.Entities;
using Library.Api.Infrastructure.Repositories.Interfaces;

namespace Library.Api.Application.Services;

public class MemberService : IMemberService
{
    private readonly IMemberRepository _memberRepository;

    public MemberService(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task<List<MemberResponse>> GetAllAsync()
    {
        var members = await _memberRepository.GetAllAsync();
        return members.Select(member => new MemberResponse(
            member.Id,
            member.FullName,
            member.Email,
            member.PhoneNumber,
            member.RegisteredDate,
            member.IsActive)).ToList();
    }

    public async Task<MemberResponse?> GetByIdAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) return null;

        return new MemberResponse(
            member.Id,
            member.FullName,
            member.Email,
            member.PhoneNumber,
            member.RegisteredDate,
            member.IsActive);
    }

    public async Task<MemberResponse> CreateAsync(CreateMemberRequest request)
    {
        var existingMember = await _memberRepository.GetByEmailAsync(request.Email);
        if (existingMember != null)
        {
            throw new Exception("Email already exists.");
        }
        var member = new Member
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            RegisteredDate = DateTime.UtcNow,
            IsActive = true
        };

        await _memberRepository.AddAsync(member);
        await _memberRepository.SaveChangesAsync();

        return new MemberResponse(
            member.Id,
            member.FullName,
            member.Email,
            member.PhoneNumber,
            member.RegisteredDate,
            member.IsActive);
    }

    public async Task UpdateAsync(Guid id, UpdateMemberRequest request)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) 
        {
            throw new Exception("Member not found");
        }
        var duplicateMember = await _memberRepository.GetByEmailAsync(request.Email);
        if (duplicateMember != null && duplicateMember.Id != id)
        {
            throw new Exception("Email already exists.");
        }

        member.FullName = request.FullName;
        member.Email = request.Email;
        member.PhoneNumber = request.PhoneNumber;

        _memberRepository.Update(member);
        await _memberRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member == null) throw new Exception("Member not found");

        _memberRepository.Delete(member);
        await _memberRepository.SaveChangesAsync();
    }
}