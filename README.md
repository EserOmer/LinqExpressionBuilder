# LinqExpressionBuilder
Dynamic Search Extension For Linq

GetById, GetByName vs.. gibi sorgu işlemlerinde bir nesne için birden çok fonksiyon yazmamız gerekebiliyor. Bundan kaynaklanan problemler arasında aynı kodun biden fazla
yazılmış olmasından tutun, içerisinde çok fazla property olan nesneler için teker teker fonksiyonlar yazmak oluyor.
Projeler derinleştikçe bu problemler projenin yükünü arttırmaya başlıyor.

Bu sebeple şuan linq üzerinden sorgular yaptığım için, doğrudan nesneyi parametre olarak gönderip, o nesnenin istenilen propertyleri üzerinde bir linq expression oluşturacak
bu uzantıyı yazdım

-Eksikleri bulunan bu uzantıda üretilmiş olan örnek linq expresion aşağıdaki gibidir;
ExpressionBuilderDemo(entity) fonksiyonu çağrıldığında dönen expression;
t => (((t.EqualProperty == 1) AndAlso t.EndsWithProperty.EndsWith("e")) AndAlso (t.ConstainProperty.Contains("co") AndAlso t.StartWihtProperty.StartsWith("St")))
