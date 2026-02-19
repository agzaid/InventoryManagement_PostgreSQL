using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum TrType
    {
        ReceiveGoods = 1,
        IssuedGoods = 2,
        OutgoingTransferGoods = 3,
        IncomingTransReturn = 4,
        IncomingItemsReturn = 5,
        IssuedItemsReturn = 6,
        DeadStock = 7,
    }
}
