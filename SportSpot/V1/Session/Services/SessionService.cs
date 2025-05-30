﻿using MongoDB.Driver.GeoJsonObjectModel;
using SportSpot.Events.Services;
using SportSpot.V1.Exceptions.Location;
using SportSpot.V1.Exceptions.Session;
using SportSpot.V1.Location.Dtos;
using SportSpot.V1.Location.Services;
using SportSpot.V1.Session.Dtos;
using SportSpot.V1.Session.Entities;
using SportSpot.V1.Session.Events;
using SportSpot.V1.Session.Mapper;
using SportSpot.V1.Session.Repositories;
using SportSpot.V1.User.Entities;

namespace SportSpot.V1.Session.Services
{
    public class SessionService(ISessionRepository _sessionRepository, ILocationService _locationService, IEventService _eventService) : ISessionService
    {
        public async Task<SessionDto> CreateSession(SessionCreateRequestDto createRequestDto, AuthUserEntity user)
        {
            if (DateTime.UtcNow >= createRequestDto.Date)
                throw new SessionInvalidDataException();

            if (createRequestDto.MinParticipants < 0 || createRequestDto.MaxParticipants < 0)
                throw new SessionInvalidParticipantsException();

            if (createRequestDto.MinParticipants > createRequestDto.MaxParticipants)
                throw new SessionInvalidParticipantsException();

            if (createRequestDto.Tags.Count > 50)
                throw new SessionTooManyTagsException();

            foreach (string tag in createRequestDto.Tags)
            {
                if (tag.Length > 50)
                {
                    throw new SessionTagTooLongException();
                }
            }

            AzureAddressDto adress = await _locationService.GetAddress("de-DE", createRequestDto.Latitude, createRequestDto.Longitude);
            if (adress.Municipality == null || adress.PostalCode == null)
                throw new LocationNotFoundException();

            SessionEntity sessionEntity = new()
            {
                Id = Guid.CreateVersion7(),
                SportType = createRequestDto.SportType,
                Title = createRequestDto.Title,
                Description = createRequestDto.Description,
                Date = createRequestDto.Date.ToUniversalTime(),
                CreatorId = user.Id,
                CreatedAt = DateTime.UtcNow,
                MinParticipants = createRequestDto.MinParticipants,
                MaxParticipants = createRequestDto.MaxParticipants,
                Location = new SessionLocationEntity
                {
                    Coordinates = new GeoJson2DGeographicCoordinates(createRequestDto.Longitude, createRequestDto.Latitude),
                    City = adress.Municipality,
                    ZipCode = adress.PostalCode
                },
                Tags = createRequestDto.Tags,
            };

            await _sessionRepository.Add(sessionEntity);
            return sessionEntity.ToDto();
        }

        public async Task<SessionEntity> Get(Guid sessionId)
        {
            return await _sessionRepository.GetSession(sessionId) ?? throw new SessionNotFoundException();
        }

        public async Task<SessionDto> GetDto(Guid sessionId, AuthUserEntity user)
        {
            SessionEntity session = await Get(sessionId);
            return session.ToDto(IsMember(session, user));
        }

        public async Task Join(AuthUserEntity user, SessionEntity session)
        {
            if (session.CreatorId == user.Id)
                throw new SessionCreatorJoinException();

            if (session.Participants.Contains(user.Id))
                throw new SessionAlreadyJoinedException();

            if (session.Participants.Count + 1 >= session.MaxParticipants)
                throw new SessionFullException();

            if (DateTime.UtcNow >= session.Date)
                throw new SessionExpiredException();

            session.Participants.Add(user.Id);
            await _sessionRepository.UpdateSession(session);
        }

        public async Task KickUser(AuthUserEntity target, SessionEntity session, AuthUserEntity sender)
        {
            if (target.Id == sender.Id)
                throw new SessionKickSelfException();

            if (session.CreatorId != sender.Id)
                throw new SessionNotCreatorException();

            if (!session.Participants.Contains(target.Id))
                throw new SessionNotJoinedException();

            session.Participants.Remove(target.Id);
            await _sessionRepository.UpdateSession(session);
        }

        public async Task Leave(AuthUserEntity user, SessionEntity session)
        {
            if (session.CreatorId == user.Id)
                throw new SessionCreatorLeaveException();

            if (!session.Participants.Contains(user.Id))
                throw new SessionNotJoinedException();

            session.Participants.Remove(user.Id);
            await _sessionRepository.UpdateSession(session);
        }

        public async Task Delete(AuthUserEntity user, SessionEntity session)
        {
            if (user.Id != session.CreatorId)
                throw new SessionNotCreatorException();

            await _sessionRepository.DeleteSession(session);
            await _eventService.FireEvent(new SessionDeletedEvent { SessionEntity = session, Executer = user });
        }

        public async Task DeleteAll()
        {
            List<SessionEntity> sessions = await _sessionRepository.GetAll();
            foreach (SessionEntity sessionEntity in sessions)
            {
                await _sessionRepository.DeleteSession(sessionEntity);
            }
        }

        public async Task<(List<SessionDto>, bool)> GetSessionsInRange(SessionSearchQueryDto requestDto, AuthUserEntity sender)
        {
            LocationInvalidException.ValidateLatitude(requestDto.Latitude);
            LocationInvalidException.ValidateLongitude(requestDto.Longitude);
            if (requestDto.Page < 0 || requestDto.Size <= 0 || requestDto.Size > 1000)
                throw new SessionInvalidPageException();
            if (requestDto.Distance < 0 || requestDto.Distance > 5000)
                throw new SessionInvalidDistanceException();
            (List<SessionEntity> sessions, bool hasMoreEntries) = await _sessionRepository.GetSessionsInRange(requestDto, sender.Id);
            List<SessionDto> result = sessions.Select(x => x.ToDto(false)).ToList();
            return (result, hasMoreEntries);
        }

        public bool IsMember(SessionEntity session, AuthUserEntity user) => session.CreatorId == user.Id || session.Participants.Contains(user.Id);

        public async Task DeleteAll(AuthUserEntity user)
        {
            List<SessionEntity> sessions = await _sessionRepository.GetSessionsFromUser(user);
            foreach (SessionEntity sessionEntity in sessions)
            {
                await Delete(user, sessionEntity);
            }
        }

        public async Task<(List<SessionDto>, bool)> GetSessionsFromUser(AuthUserEntity user, SessionUserSearchQueryDto requestDto)
        {
            if (requestDto.Page < 0 || requestDto.Size <= 0 || requestDto.Size > 1000)
                throw new SessionInvalidPageException();
            (List<SessionEntity> sessions, bool hasMoreEntries) = await _sessionRepository.GetSessionsFromUser(user, requestDto);
            List<SessionDto> result = sessions.Select(x => x.ToDto(false)).ToList();
            return (result, hasMoreEntries);
        }

        public async Task LeaveAll(AuthUserEntity user)
        {
            List<SessionEntity> sessions = await _sessionRepository.GetSessionsUserIsMember(user);
            foreach (SessionEntity sessionEntity in sessions)
            {
                await Leave(user, sessionEntity);
            }
        }
    }
}
