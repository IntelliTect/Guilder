using Microsoft.AspNetCore.Mvc;

namespace Guilder.Server.Controllers;

[ApiController]
[Route("room/{roomId}/[controller]")]
public class MeetingController
{
    [HttpGet]
    public IEnumerable<MeetingController> Get(string roomId)
    {
        return new List<MeetingController>();
    }
}
