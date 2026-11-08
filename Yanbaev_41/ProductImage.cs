using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yanbaev_41
{
    public partial class Product
    {
        public string MainImagePath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ProductPhoto))
                    return null;

                return "/photo/" + ProductPhoto.Trim();
            }
        }
    }
}

