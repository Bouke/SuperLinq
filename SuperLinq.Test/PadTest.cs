#region License and Terms
// SuperLinq - Extensions to LINQ to Objects
// Copyright (c) 2008 Jonathan Skeet. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

#nullable enable

namespace SuperLinq.Test;

using NUnit.Framework;

[TestFixture]
public class PadTest
{
	[Test]
	public void PadNegativeWidth()
	{
		AssertThrowsArgument.Exception("width", () =>
			Array.Empty<object>().Pad(-1));
	}

	[Test]
	public void PadIsLazy()
	{
		new BreakingSequence<object>().Pad(0);
	}

	[Test]
	public void PadWithFillerIsLazy()
	{
		new BreakingSequence<object>().Pad(0, new object());
	}

	public class ValueTypeElements
	{
		[Test]
		public void PadWideSourceSequence()
		{
			var result = new[] { 123, 456, 789 }.Pad(2);
			result.AssertSequenceEqual(123, 456, 789);
		}

		[Test]
		public void PadEqualSourceSequence()
		{
			var result = new[] { 123, 456, 789 }.Pad(3);
			result.AssertSequenceEqual(123, 456, 789);
		}

		[Test]
		public void PadNarrowSourceSequenceWithDefaultPadding()
		{
			var result = new[] { 123, 456, 789 }.Pad(5);
			result.AssertSequenceEqual(123, 456, 789, 0, 0);
		}

		[Test]
		public void PadNarrowSourceSequenceWithNonDefaultPadding()
		{
			var result = new[] { 123, 456, 789 }.Pad(5, -1);
			result.AssertSequenceEqual(123, 456, 789, -1, -1);
		}

		[Test]
		public void PadNarrowSourceSequenceWithDynamicPadding()
		{
			var result = "hello".ToCharArray().Pad(15, i => i % 2 == 0 ? '+' : '-');
			result.AssertSequenceEqual("hello-+-+-+-+-+".ToCharArray());
		}
	}

	public class ReferenceTypeElements
	{
		[Test]
		public void PadWideSourceSequence()
		{
			var result = new[] { "foo", "bar", "baz" }.Pad(2);
			result.AssertSequenceEqual("foo", "bar", "baz");
		}

		[Test]
		public void PadEqualSourceSequence()
		{
			var result = new[] { "foo", "bar", "baz" }.Pad(3);
			result.AssertSequenceEqual("foo", "bar", "baz");
		}

		[Test]
		public void PadNarrowSourceSequenceWithDefaultPadding()
		{
			var result = new[] { "foo", "bar", "baz" }.Pad(5);
			result.AssertSequenceEqual("foo", "bar", "baz", null, null);
		}

		[Test]
		public void PadNarrowSourceSequenceWithNonDefaultPadding()
		{
			var result = new[] { "foo", "bar", "baz" }.Pad(5, string.Empty);
			result.AssertSequenceEqual("foo", "bar", "baz", string.Empty, string.Empty);
		}
	}
}
