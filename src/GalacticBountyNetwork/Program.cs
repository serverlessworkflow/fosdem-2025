var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("open-api", new OpenApiInfo
    {
        Title = "Galactic Bounty Network (GBN)",
        Version = "v1"
    });
    options.CustomOperationIds(e => ((ControllerActionDescriptor)e.ActionDescriptor).ActionName.ToCamelCase());
});
builder.Services.AddAsyncApi();
builder.Services.AddAsyncApiGeneration(options =>
{

});
builder.Services.AddAsyncApiDocument(document =>
{
    var serverId = "http";
    var channelId = "bounties";
    var operationId = "trackBounty";
    var messageId = "cloudEvent";
    var stringSchema = new JsonSchemaBuilder().Type(SchemaValueType.String).Build();
    var objectSchema = new JsonSchemaBuilder().Type(SchemaValueType.Object).AdditionalProperties(true).Build();
    document.UsingAsyncApiV3()
        .WithTitle("GalacticBountyNetwork")
        .WithVersion("1.0.0")
        .WithServer(serverId, server => server
            .WithHost("http://localhost:5151")
            .WithProtocol(AsyncApiProtocol.Http, "2.0")
            .WithBinding(new HttpServerBindingDefinition()))
        .WithChannel(channelId, channel => channel
            .WithAddress("/api/bounties/track")
            .WithServer($"#/servers/{serverId}")
            .WithMessage(messageId, message => message
                .WithContentType(CloudEventContentType.Json)
                .WithPayloadSchema(schemaDefinition => schemaDefinition
                    .WithJsonSchema(schema => schema
                        .Type(SchemaValueType.Object)
                        .Properties(new Dictionary<string, JsonSchema>()
                        {
                            { CloudEventAttributes.SpecVersion, stringSchema },
                            { CloudEventAttributes.Id, stringSchema },
                            { CloudEventAttributes.Time, stringSchema },
                            { CloudEventAttributes.Source, stringSchema },
                            { CloudEventAttributes.Type, stringSchema },
                            { CloudEventAttributes.Subject, stringSchema },
                            { CloudEventAttributes.DataSchema, stringSchema },
                            { CloudEventAttributes.DataContentType, stringSchema },
                            { CloudEventAttributes.Data, objectSchema },
                        })
                        .Required(CloudEventAttributes.GetRequiredAttributes())
                        .AdditionalProperties(true)))
                .WithBinding(new HttpMessageBindingDefinition()))
            .WithBinding(new HttpChannelBindingDefinition()))
        .WithOperation(operationId, operation => operation
            .WithAction(V3OperationAction.Send)
            .WithChannel($"#/channels/{channelId}")
            .WithMessage($"#/channels/{channelId}/messages/{messageId}")
            .WithBinding(new HttpOperationBindingDefinition()
            {
                Method = Neuroglia.AsyncApi.Bindings.Http.HttpMethod.POST
            }));
});
builder.Services.AddHttpClient();
builder.Services.AddJsonSerializer();
builder.Services.AddSingleton<Subject<CloudEvent>>();
var app = builder.Build();
app.MapControllers();
app.UseSwagger(c =>
{
    c.RouteTemplate = "docs/{documentName}.json";
});
app.MapAsyncApiDocuments();
_ = app.RunAsync();

var serializer = app.Services.GetRequiredService<IJsonSerializer>();
var httpClient = app.Services.GetRequiredService<HttpClient>();
var events = app.Services.GetRequiredService<Subject<CloudEvent>>();
var id = string.Empty;
var firstName = string.Empty;
var lastName = string.Empty;
var highlightStyle = new Style(Color.Yellow, Color.Black, Decoration.Bold);
var exit = false;
Dictionary<string, Bounty>? bounties = null;
await InitializeAsync();
var commands = Commands.ToDictionary();
while (!exit)
{
    AnsiConsole.Clear();
    WriteTerminalHeader();
    var command = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(commands.Keys).HighlightStyle(highlightStyle));
    switch (command)
    {
        case Commands.Bounty.List.Name:
            await ListBountiesAsync();
            break;
        case Commands.Logout.Name:
            await LogoutAsync();
            break;
    }
}

void WriteTerminalHeader()
{
    var panel = new Panel("[bold yellow]:skull_and_crossbones:  Galactic Bounty Network Command Interface[/]")
            .Border(BoxBorder.Heavy)
            .Header("[bold red]GBN Terminal[/]")
            .Expand();
    AnsiConsole.Write(panel);
}

Table CreateTableFor(Bounty bounty)
{
    var table = new Table()
        .Border(TableBorder.Rounded)
        .Title($"[bold red]Bounty: {bounty.TargetName}[/]")
        .AddColumn("[cyan]Property[/]")
        .AddColumn("[yellow]Details[/]");
    table.AddRow("[bold]Bounty ID[/]", $"[green]{bounty.Id}[/]");
    table.AddRow("[bold]Status[/]", $"[blue]{bounty.Status}[/]");
    table.AddRow("[bold]Last Known Location[/]", $"[magenta]{bounty.LastKnownLocation}[/]");
    table.AddRow("[bold]Reward[/]", $"[yellow]{bounty.Reward:N0} credits[/]");
    table.AddRow("[bold]Issued By[/]", $"[red]{bounty.IssuedBy}[/]");
    if (bounty.KnownAffiliations != null && bounty.KnownAffiliations.Count > 0) table.AddRow("[bold]Affiliations[/]", $"[silver]{string.Join(", ", bounty.KnownAffiliations)}[/]");
    else table.AddRow("[bold]Affiliations[/]", "[grey]None[/]");
    table.AddRow("[bold]Posted Date[/]", $"[green]{bounty.PostedDate:yyyy-MM-dd}[/]");
    table.AddRow("[bold]Expiration Date[/]", bounty.ExpirationDate.HasValue
        ? $"[red]{bounty.ExpirationDate:yyyy-MM-dd}[/]"
        : "[grey]No Expiration[/]");
    table.AddRow("[bold]Alive Only[/]", bounty.AliveOnly ? "[green]Yes[/]" : "[red]No[/]");
    if (!string.IsNullOrWhiteSpace(bounty.AdditionalNotes)) table.AddRow("[bold]Additional Notes[/]", $"[italic]{bounty.AdditionalNotes}[/]");
    return table;
}

async Task PublishEventAsync(CloudEvent e)
{
    var content = new StringContent(serializer.SerializeToText(e), Encoding.UTF8, CloudEvents.ContentType);
    await httpClient.PostAsync(CloudEvents.Endpoint, content);
}

async Task InitializeAsync()
{
    Console.Title = "GBN Terminal";
    Console.OutputEncoding = Encoding.UTF8;
    WriteTerminalHeader();
    id = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]:key: Enter member ID:[/]"));
    Console.Clear();
    WriteTerminalHeader();
    firstName = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]:key: Enter first name:[/]"));
    Console.Clear();
    WriteTerminalHeader();
    lastName = AnsiConsole.Prompt(new TextPrompt<string>("[yellow]:key: Enter last name:[/]"));
    Console.Clear();
    WriteTerminalHeader();
    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots2)
        .SpinnerStyle(Style.Parse("yellow bold"))
        .StartAsync("[yellow]Connecting...[/]", async context =>
        {
            var e = new CloudEvent()
            {
                Id = Guid.NewGuid().ToString(),
                Source = CloudEvents.Source,
                Type = CloudEvents.Session.Initializing.Type,
                Subject = id,
                Data = new
                {
                    firstName,
                    lastName
                }
            };
            await PublishEventAsync(e);
            e = await events.FirstAsync(e => e.Type == CloudEvents.Session.Initialized.Type);
            AnsiConsole.MarkupLine(":information:  Connected");
            AnsiConsole.WriteLine();
            var payload = serializer.Deserialize<CloudEvents.Session.Initialized.Payload>((JsonElement)e.Data!)!;
            foreach (var line in payload.Message.Split("\r\n")) AnsiConsole.MarkupLine($":yellow_circle: {line}");
            AnsiConsole.WriteLine();
            context.Status("[yellow]Data uplink active... Press any key to access the Network.[/]");
            Console.ReadKey();
        });
    AnsiConsole.Clear();
}

async Task ListBountiesAsync()
{
    if (bounties == null)
    {
        var e = new CloudEvent()
        {
            Id = Guid.NewGuid().ToString(),
            Source = CloudEvents.Source,
            Type = CloudEvents.Command.Executing.Type,
            Subject = id,
            Data = new
            {
                Commands.Bounty.List.Command
            }
        };
        await PublishEventAsync(e);
        await AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots2)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .StartAsync("[yellow]Processing...[/]", async context =>
            {
                e = await events.FirstAsync(e => e.Type == CloudEvents.Command.Executed.Type);
            });
        var payload = serializer.Deserialize<CloudEvents.Command.Executed.Payload>((JsonElement)e.Data!)!;
        bounties = serializer.Deserialize<List<Bounty>>(payload.Output)!.ToDictionary(b => b.TargetName, b => b);
    }
    while (true)
    {
        AnsiConsole.Clear();
        WriteTerminalHeader();
        var rule = new Rule("[yellow]Available bounties[/]");
        rule.Justification = Justify.Left;
        AnsiConsole.Write(rule);
        var @return = "[blue]↩  Return[/]";
        var choices = bounties.Keys.ToList();
        choices.Add(@return);
        var bountyKey = AnsiConsole.Prompt(new SelectionPrompt<string>().PageSize(bounties.Count + 1).AddChoices(choices).HighlightStyle(highlightStyle));
        AnsiConsole.Clear();
        WriteTerminalHeader();
        if (bountyKey == @return) break;
        var bounty = bounties[bountyKey];
        choices = [Commands.Bounty.List.Choices.TakeContract, Commands.Bounty.List.Choices.ReturnToList];
        AnsiConsole.Write(new Panel(CreateTableFor(bounty)));
        var action = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices).HighlightStyle(highlightStyle));
        switch (action)
        {
            case Commands.Bounty.List.Choices.TakeContract:
                await TakeBountyAsync(bounty);
                break;
            case Commands.Bounty.List.Choices.ReturnToList:
            default:
                continue;
        }
        break;
    }
}

async Task TakeBountyAsync(Bounty bounty)
{
    AnsiConsole.Clear();
    WriteTerminalHeader();
    var command = StringFormatter.Format(Commands.Bounty.Contract.Take.Command, bounty.Id);
    var e = new CloudEvent()
    {
        Id = Guid.NewGuid().ToString(),
        Source = CloudEvents.Source,
        Type = CloudEvents.Command.Executing.Type,
        Subject = id,
        Data = new
        {
            command
        }
    };
    await PublishEventAsync(e);
    await AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots2)
        .SpinnerStyle(Style.Parse("yellow bold"))
        .StartAsync("[yellow]Processing...[/]", async context =>
        {
            e = await events.FirstAsync(e => e.Type == CloudEvents.Command.Executed.Type);
        });
    await Task.Delay(1000);
    await TrackBountyAsync(bounty);
}

async Task TrackBountyAsync(Bounty bounty)
{
    var rule = new Rule($"[yellow]Bounty Tracker Module[/]");
    rule.Justification = Justify.Left;
    AnsiConsole.Write(rule);
    var e = new CloudEvent()
    {
        Id = Guid.NewGuid().ToString(),
        Source = CloudEvents.Source,
        Type = CloudEvents.Command.Executing.Type,
        Subject = id,
        Data = new
        {
            Commands.Bounty.Track.Command
        }
    };
    await PublishEventAsync(e);
    await AnsiConsole.Status()
      .Spinner(Spinner.Known.Dots2)
      .SpinnerStyle(Style.Parse("yellow bold"))
      .StartAsync($"[yellow]Tracking {bounty.TargetName}...[/]", async context =>
      {
          await foreach(var evt in events.ToAsyncEnumerable())
          {
              if (evt.Type == CloudEvents.Bounty.Tracking.Feedback.Type)
              {
                  var payload = serializer.Deserialize<BountyTrackingUpdate>((JsonElement)evt.Data!)!;
                  var statusColor = payload.Status switch
                  {
                      "Sighted" => "green",
                      "Engaged" => "orangered1",
                      "Captured" => "blue",
                      "Escaped" => "yellow",
                      "Unknown" => "grey",
                      _ => "white"
                  };
                  AnsiConsole.MarkupLine($"[bold {statusColor}]▶ {payload.Status}[/] at [bold yellow]{payload.Location}[/]");
              }
              else if (evt.Type == CloudEvents.Command.Executed.Type)
              {
                  context.SpinnerStyle = Style.Parse("red bold");
                  context.Status("[bold red]🎯 Target within engagement range! Prepare for action.[/]");
                  Console.ReadKey();
                  break;
              }
          }
      });
    await LogoutAsync();
}

async Task LogoutAsync()
{
    var e = new CloudEvent()
    {
        Id = Guid.NewGuid().ToString(),
        Source = CloudEvents.Source,
        Type = CloudEvents.Command.Executing.Type,
        Subject = id,
        Data = new
        {
            Commands.Logout.Command
        }
    };
    await PublishEventAsync(e);
    exit = true;
}

static class CloudEvents
{

    public static readonly Uri Endpoint = new("https://localhost:5001/api/v1/events/pub");
    public static readonly Uri Source = new("https://boba.fett.gbn.terminal.io");
    public const string TypePrefix = "io.galactic-bounty-network.events";
    public const string ContentType = "application/cloudevents+json";

    public static class Session
    {

        public const string TypePrefix = $"{CloudEvents.TypePrefix}.session";

        public static class Initializing
        {

            public static readonly string Type = $"{TypePrefix}.initializing.v1";

        }

        public static class Initialized
        {

            public static readonly string Type = $"{TypePrefix}.initialized.v1";

            public class Payload
            {

                public string Message { get; set; } = null!;

            }

        }

        public static class Started
        {

            public static readonly string Type = $"{TypePrefix}.started.v1";

        }

    }

    public static class Command
    {

        public const string TypePrefix = $"{CloudEvents.TypePrefix}.command";

        public static class Executing
        {

            public static readonly string Type = $"{TypePrefix}.executing.v1";

        }

        public static class Executed
        {

            public static readonly string Type = $"{TypePrefix}.executed.v1";

            public class Payload
            {

                public JsonElement Output { get; set; }

            }

        }

    }

    public static class Bounty
    {

        public const string TypePrefix = $"{CloudEvents.TypePrefix}.bounty";

        public static class Tracking
        {

            public const string TypePrefix = $"{Bounty.TypePrefix}.tracking";

            public static class Feedback
            {

                public static readonly string Type = $"{TypePrefix}.feedback.v1";

            }

        }

    }

}

static class Commands
{

    public static class Bounty
    {

        public const string Command = "bounty";

        public static class List
        {

            public const string Name = ":bookmark_tabs: Bounties";

            public static readonly string Command = $"{Bounty.Command} list";

            public static class Choices
            {

                public const string TakeContract = ":bullseye: Take contract";
                public const string ReturnToList = "[blue]↩  Return[/]";

            }

        }

        public static class Contract
        {

            public static readonly string Command = $"{Bounty.Command} contract";

            public static class Take
            {

                public static readonly string Command = $"{Contract.Command} take {{bountyId}}";

            }

        }

        public static class Track
        {

            public static readonly string Command = $"{Bounty.Command} track";

        }

    }

    public static class Logout
    {

        public const string Name = "[red]:cross_mark: Logout[/]";

        public static readonly string Command = "logout";

    }

    public static Dictionary<string, string> ToDictionary()
    {
        return new()
        {
            { Bounty.List.Name, Bounty.List.Command },
            { Logout.Name, Logout.Command }
        };
    }

}