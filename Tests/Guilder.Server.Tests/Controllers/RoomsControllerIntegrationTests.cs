namespace Guilder.Server.Tests.Controllers;


public class RoomsControllerIntegrationTests : IDisposable
{
    public static Room BattleOfWits { get; } =
        new (
            "3a02a800-1e8a-49ef-82f6-be60e1147fdd",
            "1. Battle of Wits",
            "battleofwits@IntelliTect.com");
    
    private WebApplicationFactory Factory { get; } = new();

    [Fact]
    public async Task GetRoom_BattleofWits_Success()
    {
        Factory.Connector = "MicrosoftGraph";
        Room expected = new(BattleOfWits.Id, BattleOfWits.Name, BattleOfWits.Email);
        MeetingClient client = new(Factory.CreateClient());

        Room? actual = await client.GetRoomAsync(BattleOfWits.Id);
        
        Assert.Equal(expected, actual);
    }

    public void Dispose() => Factory?.Dispose();
}
