using Application.Interfaces.Contracts.Localization;
using Microsoft.Extensions.Localization;

namespace Infrastructure.Localization
{
    public class AppLocalizer : IAppLocalizer
    {
        private readonly IStringLocalizer<Infrastructure.Resources.CommonResource> _localizer;

        public AppLocalizer(IStringLocalizer<Infrastructure.Resources.CommonResource> localizer)
        {
            _localizer = localizer;
        }

        public string this[string key] => _localizer[key];

        public string GetString(string key) => _localizer[key];
    }
}
