using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MiniTrello.Domain.DataObjects
{
    public interface IHandlesErrors
    {
         int ErrorCode { set; get; }
        string ErrorMessage { set; get; }
    }

}
