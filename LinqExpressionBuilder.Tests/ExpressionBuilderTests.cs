using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace LinqExpressionBuilder.Tests
{
    public class ExpressionBuilderTests
    {
        private static readonly List<TestEntity> Data = new()
        {
            new TestEntity { Id = 1, Name = "Alice",   Email = "alice@example.com",  Age = 25 },
            new TestEntity { Id = 2, Name = "Bob",     Email = "bob@example.com",    Age = 30 },
            new TestEntity { Id = 3, Name = "Charlie", Email = "charlie@example.com",Age = 35 },
            new TestEntity { Id = 4, Name = "Dave",    Email = "dave@example.com",   Age = 40 },
        };

        // --- Equals / NotEquals ---

        [Fact]
        public void Equals_ReturnsMatchingEntity()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Id", Operation = Operation.Equals, Value = 2 }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Single(result);
            Assert.Equal("Bob", result[0].Name);
        }

        [Fact]
        public void NotEquals_ExcludesMatchingEntity()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Id", Operation = Operation.NotEquals, Value = 1 }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Equal(3, result.Count);
            Assert.DoesNotContain(result, e => e.Name == "Alice");
        }

        // --- Comparison operators ---

        [Fact]
        public void GreaterThan_ReturnsCorrectEntities()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Age", Operation = Operation.GreaterThan, Value = 30 }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Equal(2, result.Count);
            Assert.All(result, e => Assert.True(e.Age > 30));
        }

        [Fact]
        public void LessThanOrEqual_ReturnsCorrectEntities()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Age", Operation = Operation.LessThanOrEqual, Value = 30 }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Equal(2, result.Count);
        }

        // --- String operations ---

        [Fact]
        public void Contains_ReturnsMatchingEntities()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Email", Operation = Operation.Contains, Value = "example" }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Equal(4, result.Count);
        }

        [Fact]
        public void StartsWith_ReturnsMatchingEntity()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Name", Operation = Operation.StartsWith, Value = "Al" }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Single(result);
            Assert.Equal("Alice", result[0].Name);
        }

        [Fact]
        public void EndsWith_ReturnsMatchingEntity()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Name", Operation = Operation.EndsWith, Value = "ie" }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Single(result);
            Assert.Equal("Charlie", result[0].Name);
        }

        // --- And / Or connectors ---

        [Fact]
        public void AndConnector_CombinesFiltersCorrectly()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Age",  Operation = Operation.GreaterThanOrEqual, Value = 30, Connector = Connector.And },
                new() { PropertyName = "Name", Operation = Operation.StartsWith,         Value = "B", Connector = Connector.And }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Single(result);
            Assert.Equal("Bob", result[0].Name);
        }

        [Fact]
        public void OrConnector_CombinesFiltersCorrectly()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Name", Operation = Operation.Equals, Value = "Alice", Connector = Connector.And },
                new() { PropertyName = "Name", Operation = Operation.Equals, Value = "Dave",  Connector = Connector.Or }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, e => e.Name == "Alice");
            Assert.Contains(result, e => e.Name == "Dave");
        }

        // --- Edge cases ---

        [Fact]
        public void EmptyFilters_ReturnsNull()
        {
            var result = ExpressionBuilder.GetExpression<TestEntity>(new List<SearchFilter>());
            Assert.Null(result);
        }

        [Fact]
        public void SingleFilter_WorksCorrectly()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Id", Operation = Operation.Equals, Value = 3 }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            Assert.Single(result);
            Assert.Equal("Charlie", result[0].Name);
        }

        [Fact]
        public void OddNumberOfFilters_WorksCorrectly()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Age", Operation = Operation.GreaterThan,    Value = 20 },
                new() { PropertyName = "Age", Operation = Operation.LessThan,       Value = 40, Connector = Connector.And },
                new() { PropertyName = "Age", Operation = Operation.NotEquals,      Value = 30, Connector = Connector.And }
            };
            var expr = ExpressionBuilder.GetExpression<TestEntity>(filters).Compile();
            var result = Data.Where(expr).ToList();

            // Age > 20 AND Age < 40 AND Age != 30 → Alice(25), Charlie(35)
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void Contains_OnNonStringProperty_ThrowsException()
        {
            var filters = new List<SearchFilter>
            {
                new() { PropertyName = "Age", Operation = Operation.Contains, Value = 1 }
            };
            Assert.Throws<InvalidOperationException>(() =>
                ExpressionBuilder.GetExpression<TestEntity>(filters));
        }
    }
}
