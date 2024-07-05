namespace WebHookSample.Controllers.Config;

using Microsoft.AspNetCore.Mvc;

public static class CustomCacheProfile
{
    #region Profiles
    public const string NoCache = "NoCache";
    public const string Any60 = "Any60";
    #endregion

    #region Method
    public static void ApplyProfile(this MvcOptions opt)
    {
        opt.CacheProfiles.Add(NoCache, new CacheProfile() { NoStore = true });
        opt.CacheProfiles.Add(Any60, new CacheProfile()
        {
            Location = ResponseCacheLocation.Any,
            Duration = 60
        });
    }
    #endregion
}