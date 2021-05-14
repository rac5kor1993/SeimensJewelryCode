using NewProject.BuisnessLogic;
using NewProject.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NewProject.Interface
{
    public interface IJeweleryBL
    {
        bool GetUser(string nam);        
        GoldPriceCalculation GetTotalGoldPrice(GoldPriceCalculation goldprice, string userrole);
        MemoryStream CreatePdf(GoldPriceCalculation goldprice);
    }
}
