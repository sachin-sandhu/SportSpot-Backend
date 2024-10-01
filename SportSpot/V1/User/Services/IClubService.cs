namespace SportSpot.V1.User
{
    public interface IClubService
    {
        Task<ClubEntity?> GetClubById(Guid id);
        Task<ClubEntity> CreateClub(Guid id, ClubRegisterRequestDto request);
        Task DeleteClub(Guid id);
        Task DeleteClub(ClubEntity club);
    }
}
