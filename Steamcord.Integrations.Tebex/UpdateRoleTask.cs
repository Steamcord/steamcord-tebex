using System.ComponentModel.DataAnnotations.Schema;

namespace Steamcord.Integrations.Tebex;

[Table(nameof(UpdateRoleTask))]
public class UpdateRoleTask
{
    public UpdateRoleTask(TaskType type, string roleId, string steamId)
    {
        Type = type;
        RoleId = roleId;
        SteamId = steamId;
    }

    public int Id { get; set; }
    public TaskType Type { get; set; }
    public string RoleId { get; set; }
    public string SteamId { get; set; }
}
