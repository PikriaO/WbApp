using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace WbApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public abstract class BaseController : Controller
    { 
            private IMediator _mediator;

            protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
        
    }
}
