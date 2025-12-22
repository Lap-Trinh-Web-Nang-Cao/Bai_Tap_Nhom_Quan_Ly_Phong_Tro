using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using USER_QUANLYPHONGTRO.Models.Dtos;
using USER_QUANLYPHONGTRO.Models.Dtos.Rooms;

namespace USER_QUANLYPHONGTRO.Services.Interfaces
{
    /// <summary>
    /// Interface để gọi API phòng từ Backend
    /// </summary>
    public interface IPhongApiService
    {
        /// <summary>
        /// Lấy danh sách phòng công khai (có phân trang)
        /// </summary>
        Task<(List<PhongDto> rooms, int totalCount, int totalPages)> GetPublicRoomsAsync(
            int page = 1,
            int pageSize = 10,
            Guid? nhaTroId = null,
            long? minPrice = null,
            long? maxPrice = null);

        /// <summary>
        /// Lấy chi tiết một phòng
        /// </summary>
        Task<PhongDto> GetRoomDetailAsync(Guid phongId);
    }
}
