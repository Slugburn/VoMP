using System.Collections.Generic;
using VoMP.Core.Actions;
using VoMP.Core.Behavior.Choices;

namespace VoMP.Core.Behavior
{
    public interface IBehavior
    {
        IActionChoice ChooseAction(Player player, List<IAction> validActions);
        List<Route> GetMovePath(Player player, int distance);
        CityBonus ChooseOtherCityBonus(Player player);
        Reward ChooseCamelOrCoin(Player player, int count);
        Reward ChooseGoodsToGain(Player player, int count);
        Contract ChooseContractToDiscard(Player player);
        Cost ChooseGoodsToPay(Player player, Cost cost);
        IEnumerable<CityBonus> ChooseTradingPostBonuses(Player player, int count);
    }
}