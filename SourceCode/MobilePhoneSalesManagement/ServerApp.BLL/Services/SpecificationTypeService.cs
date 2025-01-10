using ServerApp.BLL.Services.Base;
using ServerApp.BLL.Services.ViewModels;
using ServerApp.DAL.Infrastructure;
using ServerApp.DAL.Models;
using System;

namespace ServerApp.BLL.Services
{
    public interface ISpecificationTypeService : IBaseService<SpecificationType>
    {
        Task<SpecificationTypeVm> AddSpecificationTypeAsync(InputSpecificationTypeVm specificationTypeVm);
        Task<SpecificationTypeVm> UpdateSpecificationTypeAsync(int id, InputSpecificationTypeVm specificationTypeVm);

        Task<SpecificationTypeVm> DeleteSpecificationTypeAsync(int id);

        Task<SpecificationTypeVm?> GetBySpecificationTypeIdAsync(int id);

        Task<IEnumerable<SpecificationTypeVm>> GetAllSpecificationTypeAsync();
    }
    public class SpecificationTypeService : BaseService<SpecificationType>, ISpecificationTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SpecificationTypeService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SpecificationTypeVm>> GetAllSpecificationTypeAsync()
        {
            var specificationTypes = await GetAllAsync();

            var SpecificationTypeViewModels = specificationTypes.Select(specificationType => new SpecificationTypeVm
            {
                SpecificationTypeId = specificationType.SpecificationTypeId,
                Name = specificationType.Name,
                CreatedAt=specificationType.CreatedAt,
                UpdatedAt=specificationType.UpdatedAt
            });

            return SpecificationTypeViewModels;
        }

        public async Task<SpecificationTypeVm?> GetBySpecificationTypeIdAsync(int id)
        {
            var specificationType = await GetByIdAsync(id);
            if (specificationType == null)
            {
                throw new ExceptionNotFound("SpecificationType not found");
            }
            var specificationTypeVm = new SpecificationTypeVm
            {
                SpecificationTypeId = specificationType.SpecificationTypeId,
                Name = specificationType.Name,
                CreatedAt = specificationType.CreatedAt,
                UpdatedAt = specificationType.UpdatedAt
            };

            return specificationTypeVm;
        }
        public async Task<SpecificationTypeVm> AddSpecificationTypeAsync(InputSpecificationTypeVm specificationTypeVm)
        {
            ValidateModelPropertiesWithAttribute(specificationTypeVm);

            var findSpecificationType = await _unitOfWork.GenericRepository<SpecificationType>().GetAsync(b =>
                b.Name == specificationTypeVm.Name
            );
            if (findSpecificationType == null)
            {
                var specificationType = new SpecificationType
                {
                    Name = specificationTypeVm.Name
                };

                if (await AddAsync(specificationType) > 0)
                {
                    return new SpecificationTypeVm
                    {
                        SpecificationTypeId = specificationType.SpecificationTypeId,
                        Name = specificationType.Name
                    };
                }
                throw new ArgumentException("Failed to update SpecificationType");
            }
            throw new ExceptionBusinessLogic("SpecificationType name is already in use.");

        }

        public async Task<SpecificationTypeVm> UpdateSpecificationTypeAsync(int id, InputSpecificationTypeVm specificationTypeVm)
        {
            ValidateModelPropertiesWithAttribute(specificationTypeVm);
            //Tìm SpecificationType
            var specificationType = await _unitOfWork.GenericRepository<SpecificationType>().GetByIdAsync(id);
            if (specificationType == null)
            {
                throw new ExceptionNotFound("SpecificationType not found");
            }
            //Tìm specificationType có tên trùng với dữ liệu nhập vào (trừ specificationType tìm được phía trên)
            var findspecificationType = await _unitOfWork.GenericRepository<SpecificationType>().GetAsync(b =>
                b. SpecificationTypeId!= id &&
                b.Name == specificationTypeVm.Name
             );

            if (findspecificationType != null)
            {
                throw new ExceptionBusinessLogic("SpecificationType name is already in use.");
            }

            specificationType.Name = specificationTypeVm.Name;
            specificationType.UpdatedAt = DateTime.Now;
            var result = await _unitOfWork.GenericRepository<SpecificationType>().ModifyAsync(specificationType);

            if (result <= 0)
            {
                throw new ArgumentException("Failed to update SpecificationType");
            }

            return new SpecificationTypeVm
            {
                SpecificationTypeId = specificationType.SpecificationTypeId,
                Name = specificationType.Name,
                CreatedAt = specificationType.CreatedAt
            };

        }

        public async Task<SpecificationTypeVm> DeleteSpecificationTypeAsync(int id)
        {
            var specificationType = await _unitOfWork.GenericRepository<SpecificationType>().GetByIdAsync(id);

            if (specificationType == null)
            {
                throw new ExceptionNotFound("SpecificationType not found");
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            _unitOfWork.GenericRepository<SpecificationType>().Delete(id);

            if (_unitOfWork.SaveChanges() > 0)
            {
                return new SpecificationTypeVm
                {
                    SpecificationTypeId = specificationType.SpecificationTypeId,
                    Name = specificationType.Name
                };
            }

            // Nếu lưu thất bại
            throw new ArgumentException("Failed to delete specificationType");

        }

    }

}
