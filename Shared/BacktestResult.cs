namespace Shared
{
    /// <summary>
    /// Result of the backtest
    /// </summary>
    /// <param name="Symbol">Symbol name</param>
    /// <param name="Profit">Profit from backtest</param>
    /// <param name="StrategyArguments">Used arguments</param>
    /// <param name="Start">Start date for backtest</param>
    /// <param name="End">End date for backtest</param>
    public record BacktestResult(
            string Symbol,
            double Profit,
            StrategyArgument[] StrategyArguments,
            DateTime Start,
            DateTime End
        );
}
