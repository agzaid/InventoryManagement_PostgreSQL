using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Contracts.Localization
{
    public interface IAppLocalizer
    {
        string this[string key] { get; }
        string GetString(string key);
    }
}
