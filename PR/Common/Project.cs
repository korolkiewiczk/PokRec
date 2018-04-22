namespace Common
{
    public class Project : JsonObject<Project>
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public Boards Boards { get; set; }
    }
}
