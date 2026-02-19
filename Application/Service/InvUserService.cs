using Application.Interfaces.Contracts.Persistance;
using Application.Interfaces.Contracts.Service;
using Application.Interfaces.Models;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using FluentValidation;
using System.Collections.Generic;
using System.Text;

namespace Application.Service
{
    internal class InvUserService : IInvUserService
    {
        private readonly IInvUserRepository _repository;
        private readonly IValidator<InvUserDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public InvUserService(
            IInvUserRepository repository, IValidator<InvUserDto> validator,IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public Task<int> CreateInvUserAsync(InvUserDto command)
        {
            throw new NotImplementedException();
        }

        public Task DeleteInvUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IReadOnlyList<InvUserDto>> GetAllInvUsersAsync()
        {
            var users = await _repository.GetAllUsersWithEmployeeDetailsAsync();

            return _mapper.Map<IReadOnlyList<InvUserDto>>(users);
        }

        public Task<InvUserDto> GetInvUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateInvUserAsync(InvUserDto command)
        {
            throw new NotImplementedException();
        }
    }
}
