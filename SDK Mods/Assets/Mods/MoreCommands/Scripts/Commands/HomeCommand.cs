using System.Linq;
using System.Text;

using CoreLib.Commands;
using CoreLib.Commands.Communication;

using MoreCommands.Util;

using Unity.Entities;

namespace MoreCommands.Chat.Commands
{
#nullable enable
  public class HomeCommand : IServerCommandHandler
  {
    public CommandOutput Execute(string[] parameters, Entity sender)
    {
      var playerEntity = sender.GetPlayerEntity();

      if (parameters.Length >= 1)
      {
        return GoToHome(playerEntity, 0);
      } else if (parameters.Length == 1)
      {
        var allParameters = string.Join(" ", parameters).TrimQuotes(out var failedCommand);

        if (failedCommand != null)
        {
          return (CommandOutput)failedCommand;
        }

        if (int.TryParse(allParameters, out var houseIndex))
        {
          return GoToHome(playerEntity, houseIndex);
        } else if (parameters[0] == "list")
        {
          if (parameters.Length >= 2)
          {
            allParameters = string.Join(" ", parameters.Skip(1)).TrimQuotes(out var failedCommand2);

            if (failedCommand2 != null)
            {
              return (CommandOutput)failedCommand2;
            }

            if (string.IsNullOrEmpty(allParameters))
            {
              return new CommandOutput("Player specified is blank.", CommandStatus.Error);
            }

            return ListHomes(playerEntity, allParameters);
          } else
          {
            return ListHomes(playerEntity);
          }
        } else if (parameters[0] == "set")
        {
          if (parameters.Length >= 2)
          {
            allParameters = string.Join(" ", parameters.Skip(1)).TrimQuotes(out var failedCommand2);

            if (failedCommand2 != null)
            {
              return (CommandOutput)failedCommand2;
            }

            if (string.IsNullOrEmpty(allParameters) || allParameters.StartsWith("list") || allParameters.StartsWith("set") || !int.TryParse(allParameters, out var _))
            {
              return new CommandOutput("Label specified is invalid.", CommandStatus.Error);
            }

            return SetHome(playerEntity, allParameters);
          }
        } else
        {
          return GoToHome(playerEntity, allParameters);
        }
      }

      return new CommandOutput("Command failed somehow spectacularly.", CommandStatus.Error);
    }

    public string GetDescription()
    {
      return new StringBuilder()
        .AppendLine("Use /home to manage homes. ")
        .AppendLine("/home {number} Teleport to home with index supplied ")
        .AppendLine("/home {label} Teleport to home with label ")
        .AppendLine("/home set {label} Set home point with label ")
        .AppendLine("  Note: the label of a home cannot start with these words: list, set, or only contain numbers ")
        .AppendLine("/home list Get all homes that are saved to your character ")
        .AppendLine("")
        .AppendLine("Admin Commands:")
        .AppendLine("/home list {player} Get list of homes of a player by name ")
        .ToString();
    }

    public string[] GetTriggerNames()
    {
      return new[] { "home" };
    }

    private CommandOutput SetHome(Entity playerEntity, string label)
    {
      return "Soup";
    }

    private CommandOutput ListHomes(Entity playerEntity, string playerName)
    {
      if (!playerEntity.IsAdmin())
      {
        return new CommandOutput("No privileges for this command.", CommandStatus.Error);
      }

      var findPlayer = CoreLib.Util.Players.GetAllPlayers().First(x => x.playerName == playerName);

      if (findPlayer == null)
      {
        return new CommandOutput("Player not found.", CommandStatus.Error);
      } else
      {
      }

      return "Soup";
    }

    private CommandOutput ListHomes(Entity playerEntity)
    {
      return "Soup";
    }

    private CommandOutput GoToHome(Entity playerEntity, int houseIndex)
    {
      return "Soup";
    }

    private CommandOutput GoToHome(Entity playerEntity, string houseLabel)
    {
      return "Soup";
    }
  }
#nullable disable
}
