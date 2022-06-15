﻿namespace SuperLinq;

public static partial class SuperEnumerable
{
	/// <summary>
	/// Produces a projection of a sequence by evaluating pairs of elements separated by a negative offset.
	/// </summary>
	/// <remarks>
	/// This operator evaluates in a deferred and streaming manner.<br/>
	/// For elements prior to the lag offset, <c>default(T)</c> is used as the lagged value.<br/>
	/// </remarks>
	/// <typeparam name="TSource">The type of the elements of the source sequence</typeparam>
	/// <typeparam name="TResult">The type of the elements of the result sequence</typeparam>
	/// <param name="source">The sequence over which to evaluate lag</param>
	/// <param name="offset">The offset (expressed as a positive number) by which to lag each value of the sequence</param>
	/// <param name="resultSelector">A projection function which accepts the current and lagged items (in that order) and returns a result</param>
	/// <returns>A sequence produced by projecting each element of the sequence with its lagged pairing</returns>

	public static IEnumerable<TResult> Lag<TSource, TResult>(this IEnumerable<TSource> source, int offset, Func<TSource, TSource?, TResult> resultSelector)
	{
		source.ThrowIfNull();
		resultSelector.ThrowIfNull();

		return source.Select(Some)
					 .Lag(offset, default, (curr, lag) => resultSelector(curr.Value, lag is (true, var some) ? some : default));
	}

	/// <summary>
	/// Produces a projection of a sequence by evaluating pairs of elements separated by a negative offset.
	/// </summary>
	/// <remarks>
	/// This operator evaluates in a deferred and streaming manner.<br/>
	/// </remarks>
	/// <typeparam name="TSource">The type of the elements of the source sequence</typeparam>
	/// <typeparam name="TResult">The type of the elements of the result sequence</typeparam>
	/// <param name="source">The sequence over which to evaluate lag</param>
	/// <param name="offset">The offset (expressed as a positive number) by which to lag each value of the sequence</param>
	/// <param name="defaultLagValue">A default value supplied for the lagged value prior to the lag offset</param>
	/// <param name="resultSelector">A projection function which accepts the current and lagged items (in that order) and returns a result</param>
	/// <returns>A sequence produced by projecting each element of the sequence with its lagged pairing</returns>

	public static IEnumerable<TResult> Lag<TSource, TResult>(this IEnumerable<TSource> source, int offset, TSource defaultLagValue, Func<TSource, TSource, TResult> resultSelector)
	{
		source.ThrowIfNull();
		resultSelector.ThrowIfNull();
		offset.ThrowIfLessThan(1);

		return _(); IEnumerable<TResult> _()
		{
			using var iter = source.GetEnumerator();

			var i = offset;
			var lagQueue = new Queue<TSource>(offset);
			// until we progress far enough, the lagged value is defaultLagValue
			var hasMore = true;
			// NOTE: The if statement below takes advantage of short-circuit evaluation
			//       to ensure we don't advance the iterator when we reach the lag offset.
			//       Do not reorder the terms in the condition!
			while (i-- > 0 && (hasMore = iter.MoveNext()))
			{
				lagQueue.Enqueue(iter.Current);
				// until we reach the lag offset, the lagged value is the defaultLagValue
				yield return resultSelector(iter.Current, defaultLagValue);
			}

			if (hasMore) // check that we didn't consume the sequence yet
			{
				// now the lagged value is derived from the sequence
				while (iter.MoveNext())
				{
					var lagValue = lagQueue.Dequeue();
					yield return resultSelector(iter.Current, lagValue);
					lagQueue.Enqueue(iter.Current);
				}
			}
		}
	}
}
