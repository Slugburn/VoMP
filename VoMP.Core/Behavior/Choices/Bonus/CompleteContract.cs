namespace VoMP.Core.Behavior.Choices.Bonus
{
    public class CompleteContract : IActionChoice
    {
        private readonly Player _player;
        private readonly Contract _contract;

        public CompleteContract(Player player,  Contract contract)
        {
            _player = player;
            _contract = contract;
        }

        public void Execute()
        {
            _player.PayCost(_contract.Cost, $"complete {_contract}");
            _player.Contracts.Remove(_contract);
            _player.CompletedContracts.Add(_contract);
            _player.GainReward(_contract.Reward, $"completing {_contract}");
        }

        public bool IsValid()
        {
            return true;
        }
    }
}