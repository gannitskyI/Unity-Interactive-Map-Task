using System;
using UnityEngine;

[Serializable]
public class PinInfo
{
    public string Id;
    public Vector2 Position;
    public string Title;
    public string Description;
    public string PhotoPath;

    public PinInfo(Vector2 position, string title, string description, string photoPath = null)
    {
        Id = Guid.NewGuid().ToString();
        Position = position;
        Title = title;
        Description = description;
        PhotoPath = photoPath;
    }
}
