namespace GalacticBountyNetwork.Controllers;

[Route("api/[controller]")]
public class EventsController(Subject<CloudEvent> events)
    : Controller
{

    [HttpPost("pub")]
    public IActionResult Pub([FromBody]CloudEvent e)
    {
        events.OnNext(e);
        return this.Accepted();
    }

}
