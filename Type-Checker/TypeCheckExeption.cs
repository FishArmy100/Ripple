using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripple
{
    class TypeCheckExeption : Exception
    { 
        public TypeCheckExeption(string message) : base(message)
        {

        }
    }

}
