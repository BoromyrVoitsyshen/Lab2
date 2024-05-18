using Lab2.Models;

namespace Lab2.Interfaces
{
    public interface ICountryInterface
    {
        ICollection<Country> GetCountries();
        Country GetCountry(int countryId);
        ICollection<Owner> GetOwnersFromACountry(int countryId);
        bool CountryExists(int countryId);
        bool CreateCountry(Country country);
        bool UpdateCountry(Country country);
        bool DeleteCountry(Country country);
        bool Save();

    }
}
