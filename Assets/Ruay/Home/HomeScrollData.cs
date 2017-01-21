public class HomeScrollData{
    public enum ContentType
    {
        PhotoWithName,
        IconWithName,
        OnlyPhoto,
        OnlyIcon,
        OnlyName
    }
    public string Name;
    public ContentType ViewType;
    public string Photo;
}
