﻿using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Session.Services
{
    public interface ISessionService
    {
        Task<SessionDto> CreateSession(SessionCreateRequestDto createRequestDto, AuthUserEntity user);
        Task KickUser(AuthUserEntity target, SessionEntity session, AuthUserEntity sender);
        Task Leave(AuthUserEntity user, SessionEntity session);
        Task Join(AuthUserEntity user, SessionEntity session);
        Task Delete(AuthUserEntity user, SessionEntity session);
        Task DeleteAll();
        Task DeleteAll(AuthUserEntity user);
        Task LeaveAll(AuthUserEntity user);
        Task<SessionDto> GetDto(Guid sessionId, AuthUserEntity user);
        Task<SessionEntity> Get(Guid sessionId);
        Task<(List<SessionDto>, bool)> GetSessionsInRange(SessionSearchQueryDto requestDto, AuthUserEntity sender);
        bool IsMember(SessionEntity session, AuthUserEntity user);
        Task<(List<SessionDto>, bool)> GetSessionsFromUser(AuthUserEntity user, SessionUserSearchQueryDto requestDto);
    }
}
