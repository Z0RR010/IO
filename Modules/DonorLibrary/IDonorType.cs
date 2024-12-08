using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface IDonorType
    {
        string getName();
        string getAddress();
        string getContactNumber();
        string getIdentifier();
        string getTypeName();
    }
}
