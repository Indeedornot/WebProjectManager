namespace IdentityServer.Pages.Account.Register;

public class AvatarHelper
{
    private Uri _avatarUrl;

    public Uri AvatarUrl
    {
        get => _avatarUrl;
        set
        {
            _avatarUrl = value;
            _avatarInput = value.ToString();
        }
    }

    private string _avatarInput;

    public string AvatarInput
    {
        get => _avatarInput;
        set
        {
            _avatarInput = value;
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri url))
            {
                AvatarUrl = url;
            }
        }
    }

    public AvatarHelper()
    {
        AvatarUrl = GetRandomAvatar();
    }

    public void RandomizeAvatar()
    {
        AvatarUrl = GetRandomAvatar();
    }

    private static Uri GetRandomAvatar()
    {
        return new Uri($"https://gravatar.com/avatar/{Guid.NewGuid().ToString("N")}?s=400&d=retro");
    }
}
