using ProductManagment_APIs.Data;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using ProductManagment_APIs.Service;
using Microsoft.EntityFrameworkCore;

namespace ProductManagment_APIs.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly LogHelper _log;

        public UserRepository(AppDbContext context, LogHelper log)
        {
            _context = context;
            _log = log;
        }

        public async Task<PagedResponse<UserDto>> GetAllAsync(PagedRequest request)
        {
            try
            {
                var query = _context.MasterUsers
                    .AsNoTracking()
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    var search = request.SearchValue.Trim().ToLower();

                    query = query.Where(x =>
                        x.UserName.ToLower().Contains(search) ||
                        x.Email.ToLower().Contains(search));
                }

                query = request.SortColumn?.ToLower() switch
                {
                    "username" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.UserName)
                        : query.OrderBy(x => x.UserName),

                    "email" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.Email)
                        : query.OrderBy(x => x.Email),

                    _ => query.OrderByDescending(x => x.CreatedAT)
                };

                var totalRecords = await query.CountAsync();

                if (request.PageSize > 0)
                {
                    query = query.Skip((request.PageNumber - 1) * request.PageSize)
                                 .Take(request.PageSize);
                }

                var users = await query.Select(x => new UserDto
                {
                    Id = x.Id,
                    UserName = x.UserName,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    Roles = x.UserRoles.Select(r => r.Role.RoleName).ToList()
                }).ToListAsync();

                _log.Log("Fetched user list.");

                return new PagedResponse<UserDto>
                {
                    TotalRecords = totalRecords,
                    TotalPages = request.PageSize == 0 ? 1 : (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Data = users
                };
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            try
            {
                var user = await _context.MasterUsers
                    .AsNoTracking()
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .Where(x => x.Id == id)
                    .Select(x => new UserDto
                    {
                        Id = x.Id,
                        UserName = x.UserName,
                        Email = x.Email,
                        IsActive = x.IsActive,
                        Roles = x.UserRoles.Select(r => r.Role.RoleName).ToList()
                    })
                    .FirstOrDefaultAsync();

                _log.Log($"Fetched user Id : {id}");

                return user;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> CreateAsync(CreateUserDto dto)
        {
            try
            {
                if (await _context.MasterUsers.AnyAsync(x => x.UserName == dto.UserName))
                    return false;

                var user = new AppUser
                {
                    UserName = dto.UserName,
                    Email = dto.Email,
                    PasswordHash = PasswordHelper.HashPassword(dto.Password),
                    IsActive = true
                };

                _context.MasterUsers.Add(user);
                await _context.SaveChangesAsync();

                if (dto.RoleIds.Any())
                {
                    var userRoles = dto.RoleIds.Select(roleId => new UserRole
                    {
                        Id = user.Id,
                        RoleId = roleId
                    });

                    await _context.UserRoles.AddRangeAsync(userRoles);
                    await _context.SaveChangesAsync();
                }

                _log.Log($"User created : {user.UserName}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
            try
            {
                var user = await _context.MasterUsers
                    .Include(x => x.UserRoles)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return false;

                user.UserName = dto.UserName;
                user.Email = dto.Email;

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    user.PasswordHash = PasswordHelper.HashPassword(dto.Password);
                }

                _context.UserRoles.RemoveRange(user.UserRoles);

                if (dto.RoleIds.Any())
                {
                    var newRoles = dto.RoleIds.Select(roleId => new UserRole
                    {
                        Id = id,
                        RoleId = roleId
                    });

                    await _context.UserRoles.AddRangeAsync(newRoles);
                }

                await _context.SaveChangesAsync();

                _log.Log($"User updated : {id}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.MasterUsers
                    .Include(x => x.UserRoles)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return false;

                _context.UserRoles.RemoveRange(user.UserRoles);
                _context.MasterUsers.Remove(user);

                await _context.SaveChangesAsync();

                _log.Log($"User deleted : {id}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> ChangeStatusAsync(int id)
        {
            try
            {
                var user = await _context.MasterUsers.FindAsync(id);

                if (user == null)
                    return false;

                user.IsActive = !user.IsActive;

                await _context.SaveChangesAsync();

                _log.Log($"User status changed : {id}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }
    }
}