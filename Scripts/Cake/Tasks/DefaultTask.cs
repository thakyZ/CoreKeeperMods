using Cake.Frosting;

namespace Cake.NekoBoiNick.CoreKeeperMods.Tasks;
[TaskName("Default")]
[IsDependentOn(typeof(CsProjectFileModifyTask)), IsDependentOn(typeof(CleanTask)), IsDependentOn(typeof(BuildTask))]
public sealed class DefaultTask : FrostingTask<BuildContext>;
