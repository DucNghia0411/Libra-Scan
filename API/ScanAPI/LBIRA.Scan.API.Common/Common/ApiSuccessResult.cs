using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Common.Common;

public class ApiSuccessResult<T> : ApiResult<T>
{
    public ApiSuccessResult()
    {
        IsSuccessed = true;
    }
    public ApiSuccessResult(T resultObj)
    {
        IsSuccessed = true;
        ResultObj = resultObj;
    }
}
