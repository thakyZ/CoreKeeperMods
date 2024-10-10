#nullable enable
using System.Linq;
using Unity.Entities;
using CoreLib.Commands;
using CoreLib.Commands.Communication;
using NekoBoiNick.CoreKeeper.Common.Util;

namespace MoreCommands.Chat.Commands
{
  public class HomeCommand : IServerCommandHandler
  {
    public CommandOutput Execute(string[] parameters, Entity sender)
    {
      var playerEntity = sender.GetPlayerEntity();

      if (parameters.Length >= 1)
      {
        var allParameters = string.Join(" ", parameters).TrimQuotes(out CommandOutput? failedCommand);

        if (failedCommand is CommandOutput commandOutput) {
          return commandOutput;
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
        }
        else if (parameters[0] == "set" && parameters.Length >= 2)
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

        return GoToHome(playerEntity, allParameters);
      }

      return GetDescription();
    }

    public string GetDescription()
    {
      return "Use /home to manage homes. " + '\n' +
        "/home {number} Teleport to home with index supplied " + '\n' +
        "/home {label} Teleport to home with label " + '\n' +
        "/home set {label} Set home point with label " + '\n' +
        "  Note: the label of a home cannot start with these words: list, set, or only contain numbers " + '\n' +
        "/home list Get all homes that are saved to your character " + '\n' +
        "" + '\n' +
        "Admin Commands:" + '\n' +
        "/home list {player} Get list of homes of a player by name ";
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

      PlayerController? findPlayer = CoreLib.Util.Players.GetAllPlayers().First(x => x.playerName == playerName);

      if (findPlayer == null)
      {
        return new CommandOutput("Player not found.", CommandStatus.Error);
      }
      else
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
