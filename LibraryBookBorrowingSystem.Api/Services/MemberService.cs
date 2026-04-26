using LibraryBookBorrowingSystem.Api.Dtos;
using LibraryBookBorrowingSystem.Api.Models;
using LibraryBookBorrowingSystem.Api.Repositories;

namespace LibraryBookBorrowingSystem.Api.Services;

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
        return members.Select(m => ToResponse(m)).ToList();
    }

    public async Task<MemberResponse?> GetByIdAsync(Guid id)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member is null) return null;
        return ToResponse(member);
    }

    public async Task<MemberResponse> CreateAsync(CreateMemberRequest request)
    {
        var member = new Member
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Email = request.Email,
            MembershipDate = DateTime.UtcNow
        };

        var created = await _memberRepository.AddAsync(member);
        return ToResponse(created);
    }

    public async Task<MemberResponse?> UpdateAsync(Guid id, UpdateMemberRequest request)
    {
        var member = await _memberRepository.GetByIdAsync(id);
        if (member is null) return null;

        member.FullName = request.FullName;
        member.Email = request.Email;

        var updated = await _memberRepository.UpdateAsync(member);
        return updated is null ? null : ToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _memberRepository.DeleteAsync(id);
    }

    private static MemberResponse ToResponse(Member m) => new MemberResponse
    {
        Id = m.Id,
        FullName = m.FullName,
        Email = m.Email,
        MembershipDate = m.MembershipDate
    };
}
