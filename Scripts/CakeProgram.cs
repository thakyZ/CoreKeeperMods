using Cake.Frosting;

namespace Cake.NekoBoiNick.CoreKeeperMods;
public static class CakeProgram {
  public static int Main(string[] args) {
    return new CakeHost()
      .UseContext<BuildContext>()
      .Run(args);
  }
}
