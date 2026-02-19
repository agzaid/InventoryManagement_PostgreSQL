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
    internal class StoreService : IStoreService
    {
        private readonly IValidator<StoreDto> _validator;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public StoreService( IValidator<StoreDto> validator,IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _validator = validator;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreDto> GetSystemSettingsAsync()
        {
            var settings = await _unitOfWork.StoreRepository.GetAsyncById(1);
            var resu = _mapper.Map<StoreDto>(settings);
            return resu;
        }

        public async Task<int> UpdateSystemSettingsAsync(StoreDto model)
        {
            var settings = await _unitOfWork.StoreRepository.GetAsyncById(1);
            settings.SysLock = model.SysLock;
            var mappedModel= _mapper.Map<Store>(settings);
            await _unitOfWork.StoreRepository.UpdateAsync(mappedModel);

            int rowsAffected = await _unitOfWork.SaveChangesAsync();

            return rowsAffected;
        }
    }
}
