// using Microsoft.Extensions.Time.Testing;

// using TaskTitan.Data.Expressions;
// using TaskTitan.Data.Extensions;
// using TaskTitan.Data.Parsers;

// namespace TaskTitan.Data.Tests;

// public class AttributeTests
// {
//     private readonly FakeTimeProvider _timeProvider;
//     private readonly DateParser _dateParser;

//     public AttributeTests()
//     {
//         _timeProvider = new FakeTimeProvider(new DateTime(2024, 06, 06));
//         _dateParser = new DateParser(_timeProvider);
//     }
//     [Test]
//     public async Task AnAttributeCanBeCreatedWithRelativeDateTimeValue()
//     {
//         string text = "due:tomorrow";

//         var pair = text.Split(':');
//         var attribute = TaskProperty.Create(pair[0], pair[1], _dateParser);

//         var actual = attribute as TaskProperty<DateTime>;
//         await Assert.That(actual).IsNotNull();
//         await Assert.That(actual?.Value).IsNotNull()
//         .And
//         .IsTypeOf<DateTime>();
//         await Assert.That(actual!.Value).IsEqualTo(new DateTime(2024, 06, 07, 0, 0, 0));
//     }
//     [Test]
//     public async Task AnAttributeCanBeCreatedWithActualDateTimeValue()
//     {
//         string text = "due:2024-12-12";

//         var pair = text.Split(':');
//         var attribute = TaskProperty.Create(pair[0], pair[1], _dateParser);

//         var actual = attribute as TaskProperty<DateTime>;
//         await Assert.That(actual).IsNotNull();
//         await Assert.That(actual?.Value).IsNotNull()
//         .And
//         .IsTypeOf<DateTime>();
//     }
//     [Test]
//     public async Task AnAttributeCanBeCreatedWithModifierAndRelativeDateTimeValue()
//     {
//         string text = "due.after:tomorrow";

//         var pair = text.Split(':');
//         var attribute = TaskProperty.Create(pair[0], pair[1], _dateParser);

//         var actual = attribute as TaskProperty<DateTime>;
//         await Assert.That(actual).IsNotNull();
//         await Assert.That(actual?.Value).IsNotNull()
//         .And
//         .IsTypeOf<DateTime>();
//     }
//     [Test]
//     public async Task AnAttributeCanBeCreatedWithStringValue()
//     {
//         string text = "project:home";

//         var pair = text.Split(':');
//         var attribute = TaskProperty.Create(pair[0], pair[1], _dateParser);

//         var actual = attribute as TaskProperty<string>;
//         await Assert.That(actual).IsNotNull();
//         await Assert.That(actual?.Value).IsNotNull()
//         .And
//         .IsTypeOf<string>();
//     }

//     [Test]
//     public async Task AnExpressionCanBeConvertedToDynamicLinq()
//     {
//         var attribute = TaskProperty.Create("due", "tomorrow", _dateParser);
//         var expected = $"Due == {new DateTime(2024, 06, 07, 0, 0, 0)}";
//         var expr = new FilterExpression(attribute);
//         var converted = expr.ToDynamicLinq();

//         await Assert.That(converted).IsEqualTo(expected);
//     }

//     [Test]
//     [Arguments("due.after", "tomorrow", $"Due >= 07/06/2024 00:00:00")]
//     [Arguments("start.before", "tuesday", $"Start < 11/06/2024 00:00:00")]
//     [Arguments("end.is", "wednesday", $"End == 12/06/2024 00:00:00")]
//     [Arguments("until.not", "thursday", $"Until != 13/06/2024 00:00:00")]
//     public async Task ManyDateExpressionsCanBeConvertedToDynamicLinq(string field, string value, string expected)
//     {
//         var attribute = TaskProperty.Create(field, value, _dateParser);
//         var expr = new FilterExpression(attribute);
//         var converted = expr.ToDynamicLinq();

//         await Assert.That(converted).IsEqualTo(expected);
//     }

//     // [Test]
//     // [Arguments("due.after", "tomorrow", $"due >= 07/06/2024 00:00:00")]
//     // [Arguments("start.before", "tuesday", $"start < 11/06/2024 00:00:00")]
//     // [Arguments("end.is", "wednesday", $"end == 12/06/2024 00:00:00")]
//     // [Arguments("until.not", "thursday", $"until != 13/06/2024 00:00:00")]
//     // public async Task ManyNumberExpressionsCanBeConvertedToDynamicLinq(string field, string value, string expected)
//     // {

//     // }

//     [Test]
//     [Arguments("project.equals", "work", $"Project.Equals(\"work\", StringComparison.CurrentCultureIgnoreCase)")]
//     [Arguments("description.has", "take the dog", $"Description.Contains(\"take the dog\", StringComparison.CurrentCultureIgnoreCase)")]
//     [Arguments("status.startswith", "pending", $"Status.StartsWith(\"pending\", StringComparison.CurrentCultureIgnoreCase)")]
//     [Arguments("uuid.endswith", "03b", $"Uuid.EndsWith(\"03b\", StringComparison.CurrentCultureIgnoreCase)")]
//     public async Task ManyTextExpressionsCanBeConvertedToDynamicLinq(string field, string value, string expected)
//     {
//         var attribute = TaskProperty.Create(field, value, _dateParser);
//         var expr = new FilterExpression(attribute);
//         var converted = expr.ToDynamicLinq();

//         await Assert.That(converted).IsEqualTo(expected);
//     }

//     [Test]
//     public async Task BinaryExpressionCanBeConvertedToDynamicLinq()
//     {
//         var expected = $"Due < 07/06/2024 00:00:00 || Project.Equals(\"work\", StringComparison.CurrentCultureIgnoreCase)";
//         var attribute1 = TaskProperty.Create("due.before", "tomorrow", _dateParser);
//         var attribute2 = TaskProperty.Create("project", "work", _dateParser);

//         var binary = new BinaryFilter(attribute1, BinaryOperator.Or, attribute2);

//         var expr = new FilterExpression(binary);

//         var converted = expr.ToDynamicLinq();

//         await Assert.That(converted).IsEqualTo(expected);
//     }

//     [Test]
//     public async Task DeepBinaryExpressionCanBeConvertedToDynamicLinq()
//     {
//         var expected = $"(Due < 07/06/2024 00:00:00 && Project.Equals(\"work\", StringComparison.CurrentCultureIgnoreCase)) || Project.Contains(\"home\", StringComparison.CurrentCultureIgnoreCase)";
//         var attribute1 = TaskProperty.Create("due.before", "tomorrow", _dateParser);
//         var attribute2 = TaskProperty.Create("project", "work", _dateParser);
//         var attribute3 = TaskProperty.Create("project.has", "home", _dateParser);
//         var binary = new BinaryFilter(attribute1, BinaryOperator.And, attribute2);

//         var binary2 = new BinaryFilter(binary, BinaryOperator.Or, attribute3);
//         var expr = new FilterExpression(binary2);

//         var converted = expr.ToDynamicLinq();

//         await Assert.That(converted).IsEqualTo(expected);
//     }
// }
