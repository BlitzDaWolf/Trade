using Shared;

namespace Command.Models
{
    public class StrategyArgumentModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Value { get; set; }
        public string Argument { get; set; }
        public string Strategy { get; set; }

        public Guid ResultModelId { get; set; }

        public string FQN => Strategy + ":" + Argument;

        public static StrategyArgumentModel FromGrpc(StrategyArgument result, Guid resultModelId)
        {
            return new StrategyArgumentModel
            {
                Argument = result.Argument,
                Strategy = result.Strategy,
                Value = result.value,
                ResultModelId = resultModelId
            };
        }
    }
}
