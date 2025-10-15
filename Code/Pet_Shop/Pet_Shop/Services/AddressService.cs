using Microsoft.EntityFrameworkCore;
using Pet_Shop.Data;
using Pet_Shop.Models.Entities;
using Pet_Shop.Models.ViewModels;

namespace Pet_Shop.Services
{
    public class AddressService
    {
        private readonly PetShopDbContext _context;
        private readonly ILogger<AddressService> _logger;

        public AddressService(PetShopDbContext context, ILogger<AddressService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<AddressViewModel>> GetUserAddressesAsync(int userId)
        {
            try
            {
                var addresses = await _context.Addresses
                    .Where(a => a.UserID == userId)
                    .OrderBy(a => a.IsDefault ? 0 : 1)
                    .ThenBy(a => a.CreatedDate)
                    .ToListAsync();

                return addresses.Select(a => new AddressViewModel
                {
                    AddressID = a.AddressID,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    Address = a.AddressLine,
                    Ward = a.Ward,
                    District = a.District,
                    City = a.City,
                    IsDefault = a.IsDefault
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting addresses for user {userId}: {ex.Message}");
                return new List<AddressViewModel>();
            }
        }

        public async Task<AddressViewModel?> GetAddressByIdAsync(int addressId, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressID == addressId && a.UserID == userId);

                if (address == null) return null;

                return new AddressViewModel
                {
                    AddressID = address.AddressID,
                    FullName = address.FullName,
                    Phone = address.Phone,
                    Address = address.AddressLine,
                    Ward = address.Ward,
                    District = address.District,
                    City = address.City,
                    IsDefault = address.IsDefault
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting address {addressId} for user {userId}: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CreateAddressAsync(int userId, AddressViewModel addressModel)
        {
            try
            {
                // If this is set as default, unset other default addresses
                if (addressModel.IsDefault)
                {
                    await UnsetDefaultAddressesAsync(userId);
                }

                var address = new Address
                {
                    UserID = userId,
                    FullName = addressModel.FullName,
                    Phone = addressModel.Phone,
                    AddressLine = addressModel.Address,
                    Ward = addressModel.Ward,
                    District = addressModel.District,
                    City = addressModel.City,
                    IsDefault = addressModel.IsDefault,
                    CreatedDate = DateTime.Now
                };

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Address created for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating address for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateAddressAsync(int userId, AddressViewModel addressModel)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressID == addressModel.AddressID && a.UserID == userId);

                if (address == null)
                {
                    _logger.LogWarning($"Address {addressModel.AddressID} not found for user {userId}");
                    return false;
                }

                // If this is set as default, unset other default addresses
                if (addressModel.IsDefault && !address.IsDefault)
                {
                    await UnsetDefaultAddressesAsync(userId);
                }

                address.FullName = addressModel.FullName;
                address.Phone = addressModel.Phone;
                address.AddressLine = addressModel.Address;
                address.Ward = addressModel.Ward;
                address.District = addressModel.District;
                address.City = addressModel.City;
                address.IsDefault = addressModel.IsDefault;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Address {addressModel.AddressID} updated for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating address {addressModel.AddressID} for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteAddressAsync(int addressId, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressID == addressId && a.UserID == userId);

                if (address == null)
                {
                    _logger.LogWarning($"Address {addressId} not found for user {userId}");
                    return false;
                }

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Address {addressId} deleted for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting address {addressId} for user {userId}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SetDefaultAddressAsync(int addressId, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.AddressID == addressId && a.UserID == userId);

                if (address == null)
                {
                    _logger.LogWarning($"Address {addressId} not found for user {userId}");
                    return false;
                }

                // Unset other default addresses
                await UnsetDefaultAddressesAsync(userId);

                // Set this address as default
                address.IsDefault = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Address {addressId} set as default for user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting default address {addressId} for user {userId}: {ex.Message}");
                return false;
            }
        }

        private async Task UnsetDefaultAddressesAsync(int userId)
        {
            var defaultAddresses = await _context.Addresses
                .Where(a => a.UserID == userId && a.IsDefault)
                .ToListAsync();

            foreach (var addr in defaultAddresses)
            {
                addr.IsDefault = false;
            }
        }
    }
}
