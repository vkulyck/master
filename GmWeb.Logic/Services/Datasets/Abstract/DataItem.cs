using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmWeb.Logic.Services.Datasets.Abstract;
public abstract record class DataItem
{
    public abstract bool Validate();
}
