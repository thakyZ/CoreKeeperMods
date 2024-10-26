using Cake.Frosting;
using Cake.NekoBoiNick.CoreKeeperMods.Enums;

namespace Cake.NekoBoiNick.CoreKeeperMods.Tasks;

[TaskName("Build")]
public sealed class BuildTask : FrostingTask<BuildContext> {
  public override bool ShouldRun(BuildContext context) {
    return context.MSBuildTask is MSBuildTask.Build or MSBuildTask.Rebuild;
  }

  public override void Run(BuildContext context) {
  }
}
