namespace myGreeterBot.Entities;

public class EmbedEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string URL { get; set; }
    public string Color { get; set; }
    
    public List<Field> Fields { get; set; }

    public Timestamp Timestamp { get; set; }
    
    public string Image { get; set; }
    public string Thumbnail { get; set; }
    public Author Author { get; set; }
    public Footer Footer { get; set; }
}