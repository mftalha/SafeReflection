# CSharp Safe Reflection Extension

Bu proje, C# içerisinde herhangi bir tipi (`class`, `struct`, `List`, `Primitive`, `null`) güvenli bir şekilde string (log) formatına çeviren kurşun geçirmez (bulletproof) bir Reflection Utility metodudur.

## Çözülen Sorunlar:
1. **NullReferenceException:** Null gelen nesnelerde uygulamanın çökmesi engellendi.
2. **TargetParameterCountException:** `string` (Chars[int]) ve `List<T>` (Item[int]) içindeki indexer (indeksleyici) property'lerin Reflection'ı çökertme sorunu çözüldü.
3. **Performans Kaybı:** İlkel (Primitive) tipler ve Listeler gereksiz Reflection döngülerinden kurtarıldı.