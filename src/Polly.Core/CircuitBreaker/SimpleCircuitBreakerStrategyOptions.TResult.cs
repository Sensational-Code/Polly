using System.ComponentModel.DataAnnotations;

namespace Polly.CircuitBreaker;

/// <summary>
/// The options for the simple circuit breaker resilience strategy.
/// </summary>
/// <typeparam name="TResult">The type of result the circuit breaker strategy handles.</typeparam>
/// <remarks>
/// The circuit will break if <see cref="FailureThreshold"/> exceptions or results that are handled by the resilience strategy are encountered consecutively.
/// <para>
/// The circuit will stay broken for the <see cref="CircuitBreakerStrategyOptions{TResult}.BreakDuration"/>. Any attempt to execute the resilience strategy
/// while the circuit is broken, will immediately throw a <see cref="BrokenCircuitException"/> containing the exception or result
/// that broke the circuit.
/// </para>
/// <para>
/// If the first action after the break duration period results in a handled exception or result, the circuit will break
/// again for another <see cref="CircuitBreakerStrategyOptions{TResult}.BreakDuration"/>; if no exception or handled result is encountered, the circuit will reset.
/// </para>
/// </remarks>
public class SimpleCircuitBreakerStrategyOptions<TResult> : CircuitBreakerStrategyOptions<TResult>
{
    /// <summary>
    /// Gets or sets the number of the outcome failures handled by <see cref="CircuitBreakerStrategyOptions{TResult}.ShouldHandle"/> before opening the circuit.
    /// </summary>
    /// <value>
    /// The default value is 100. Must be greater than 0.
    /// </value>
    [Range(1, int.MaxValue)]
    public int FailureThreshold { get; set; } = CircuitBreakerConstants.DefaultFailureThreshold;
}
