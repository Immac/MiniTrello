using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTrello.Domain.DataObjects
{
    public class OrganizationNameDescriptionModel: IHandlesErrors
    {
        public string Name { set; get; }
        public string Description { set; get; }

        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
