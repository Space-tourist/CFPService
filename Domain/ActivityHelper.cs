namespace Domain;

public static class ActivityHelper
{
    private static readonly ActivityType[] AvailableActivities =
    {
        ActivityType.Report,
        ActivityType.Discussion,
        ActivityType.Masterclass
    };
    
    public static Activity GetActivity(ActivityType activityType)
    {
        return activityType switch
        {
            ActivityType.Report => new Activity
            {
                ActivityType = activityType,
                Description = "Доклад, 35-45 минут"
            },
            ActivityType.Discussion => new Activity
            {
                ActivityType = activityType,
                Description = "Дискуссия / круглый стол, 40-50 минут"
            },
            ActivityType.Masterclass => new Activity
            {
                ActivityType = activityType,
                Description = "Мастеркласс, 1-2 часа"
            },
            _ => throw new ArgumentOutOfRangeException(nameof(activityType), activityType, null)
        };
    }

    public static Activity[] GetAllActivities()
    {
        return AvailableActivities
            .Select(GetActivity)
            .ToArray();
    }
}