namespace myGreeterBot.Entities;

public class EmbedEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string URL { get; set; }
    public string Color { get; set; }
    
    public List<Field> Fields { get; set; }

    public string Timestamp { get; set; }
    
    public FIEntity Image { get; set; }
    public FIEntity Thumbnail { get; set; }
    public Author Author { get; set; }
    public Footer Footer { get; set; }
}