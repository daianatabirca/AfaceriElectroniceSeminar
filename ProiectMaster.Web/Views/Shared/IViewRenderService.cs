using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ProiectMaster.Web.Views.Shared
{
    public interface IViewRenderService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
