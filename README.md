# LinqExpressionBuilder

Herhangi bir C# nesnesi için dinamik olarak LINQ `Expression<Func<T, bool>>` üreten hafif bir kütüphane.

`GetById`, `GetByName` gibi her alan için ayrı metod yazmak yerine tek bir genel filtre mekanizmasıyla tüm sorgularını yönet.

---

## Özellikler

- Nesneyi doğrudan parametre olarak gönder, filtreler otomatik oluşturulsun
- `And` / `Or` bağlaç desteği — karmaşık sorgu koşulları kurulabilir
- 9 operasyon: `Equals`, `NotEquals`, `GreaterThan`, `GreaterThanOrEqual`, `LessThan`, `LessThanOrEqual`, `Contains`, `StartsWith`, `EndsWith`
- Compile-time güvenli property adı alma (`GetPropertyName`)
- Null-safe, açık hata mesajları

---

## Kurulum

Projeyi doğrudan solution'ına ekleyebilirsin:

```
LinqExpressionBuilder/
├── ExpressionBuilder.cs
├── ExpressionBuilderHelper.cs
├── CustomSearchList.cs
├── SearchFilter.cs
└── Operation.cs
```

---

## Kullanım

### 1. Temel Kullanım

```csharp
var filters = new List<SearchFilter>
{
    new() { PropertyName = "Name",  Operation = Operation.StartsWith, Value = "Al" },
    new() { PropertyName = "Age",   Operation = Operation.GreaterThan, Value = 20, Connector = Connector.And }
};

var expression = ExpressionBuilder.GetExpression<Person>(filters);
var result = dbContext.Persons.Where(expression).ToList();
```

### 2. Nesneyi Otomatik Tarayarak Filtre Oluşturma

```csharp
var entity = new Person { Name = "Alice", Age = 25 };

var operations = new Dictionary<string, Operation>
{
    { ExpressionBuilderHelper.GetPropertyName(() => entity.Name), Operation.Contains },
    { ExpressionBuilderHelper.GetPropertyName(() => entity.Age),  Operation.Equals   }
};

var filters  = CustomSearchList.CustomSearchFilter(entity, operations);
var typedFilters = ExpressionBuilderHelper.ChangeFilterValueType<Person>(filters);
var expression = ExpressionBuilder.GetExpression<Person>(typedFilters);
```

### 3. Or Bağlacı ile Kullanım

```csharp
var filters = new List<SearchFilter>
{
    new() { PropertyName = "Name", Operation = Operation.Equals, Value = "Alice", Connector = Connector.And },
    new() { PropertyName = "Name", Operation = Operation.Equals, Value = "Bob",   Connector = Connector.Or  }
};

// t => t.Name == "Alice" || t.Name == "Bob"
var expression = ExpressionBuilder.GetExpression<Person>(filters);
```

---

## Desteklenen Operasyonlar

| Operasyon          | Açıklama                        | Tip        |
|--------------------|---------------------------------|------------|
| `Equals`           | Eşit                            | Tüm tipler |
| `NotEquals`        | Eşit değil                      | Tüm tipler |
| `GreaterThan`      | Büyüktür                        | Sayısal    |
| `GreaterThanOrEqual` | Büyük eşit                    | Sayısal    |
| `LessThan`         | Küçüktür                        | Sayısal    |
| `LessThanOrEqual`  | Küçük eşit                      | Sayısal    |
| `Contains`         | İçerir                          | `string`   |
| `StartsWith`       | İle başlar                      | `string`   |
| `EndsWith`         | İle biter                       | `string`   |

---

## Testler

`LinqExpressionBuilder.Tests` projesi xUnit tabanlı 12 test içerir:

```
dotnet test
```

---

## Teknoloji

- .NET Standard 2.1
- Harici bağımlılık yok
- `System.Linq.Expressions` üzerine kurulu
