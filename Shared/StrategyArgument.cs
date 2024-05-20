namespace Shared
{
    public record StrategyArgument(string Strategy, string Argument, int value)
    {
        public string FQN => Strategy + ":" + Argument;
    }
}
