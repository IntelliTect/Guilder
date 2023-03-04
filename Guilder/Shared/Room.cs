namespace Guilder.Shared;

public class Room
{
    public string Id { get; set; }
    public string Name { get; set; }

    public Room(string id, string name)
    {
        Id = id;
        Name = name;
    }
}
