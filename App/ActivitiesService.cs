using App.Contracts;
using Domain;

namespace App;

public static class ActivitiesService
{
    public static ActivityDto[] GetAllActivities()
    {
        var activities = ActivityHelper.GetAllActivities();

        return activities.Select(t => new ActivityDto
            {
                Activity = t.ActivityType.ToString(),
                Description = t.Description
            })
            .ToArray();
    }
}