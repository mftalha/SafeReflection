# C# Safe Reflection Extension (Güvenli Nesne Loglayıcı)

Bu proje, C# içerisinde herhangi bir nesneyi tipinden bağımsız olarak (`class`, `struct`, `List`, `Primitive`, `null`) güvenli bir şekilde metin (string) formatına çeviren, hata toleranslı (fault-tolerant) bir Reflection (Yansıma) yardımcı metodudur. Genellikle projelerin ortak (Core/Common) katmanlarında loglama ve debug (hata ayıklama) operasyonları için kullanılır.

## 📌 Reflection (Yansıma) Nedir ve Neden Kullanılır?
C#'ta **Reflection**, kod çalışırken (runtime) bir nesnenin türünü, içindeki özelliklerini (property) ve değerlerini dinamik olarak incelememizi sağlayan güçlü bir mekanizmadır. 

Geliştirme süreçlerinde, loglama yaparken nesnelerin içindeki değerleri ekrana veya dosyaya yazdırmak isteriz. Ancak her sınıf için tek tek `ToString()` metodunu ezmek (override) ciddi bir iş yüküdür. Reflection sayesinde, sistemdeki tüm nesnelerin içindeki alanları otomatik olarak tarayıp `PropertyAdı = Değeri` şeklinde formatlayabiliriz.

## 🚀 Bu Metodun Amacı
Standart bir Reflection döngüsü karmaşık nesnelerle karşılaştığında uygulamanın çökmesine (Exception) sebep olabilir. Bu genişletme (Extension) metodu, geliştiricinin değişken tipi ne olursa olsun (`string`, `List<Guid>`, `UrunEntity` vb.) arkasına `.GetValuesU()` yazarak sistemi çökertmeden güvenle log alabilmesini sağlar.

---

## 🛠 Teknik Gelişim ve Çözülen Sorunlar (Geliştirici Notu)
*Bu bölüm, temel bir Reflection metodunun neden yeterli olmadığını ve bu yapının hangi kronik hataları çözmek için tasarlandığını özetler.*

Normal şartlarda basit bir `typeof(T).GetProperties()` döngüsü standart class'lar için sorunsuz çalışır. Ancak yapı canlıya alındığında aşağıdaki senaryolarda uygulama çökmeye başlar. Bu proje, bu hataları önlemek için şu güvenlik filtrelerini içerir:

1. **NullReferenceException Koruması:** 
   Loglanmak istenen nesne bellekte `null` ise, temel Reflection nesnenin içine girmeye çalışıp sistemi patlatır.
   * **Çözüm:** Metodun en başında `value == null` kontrolü yapılarak anında "null" metni dönülür.

2. **TargetParameterCountException Koruması (İndeksleyici Hatası):**
   `string` veya `List<T>` gibi yapılar, içlerinde parametre alan özellikler barındırır (Örn: string için `Chars[int]`). Reflection bu property'leri normal bir alan gibi okumaya çalıştığında indeks parametresi bulamadığı için patlar.
   * **Çözüm:** `prop.GetIndexParameters().Length > 0` kontrolü ile indeksleyiciler (Indexers) tespit edilip okuma işleminden atlanır.

3. **Performans ve Gereksiz Yük Optimizasyonu:**
   `int`, `Guid`, `DateTime` gibi ilkel (Primitive) tipler veya Listeler için Reflection kullanmak gereksiz bir bellek ve işlemci yüküdür.
   * **Çözüm:** Metot, gelen tipi analiz eder. Eğer gelen veri ilkel bir tip veya string ise Reflection'a hiç girmeden doğrudan `.ToString()` yapar. Eğer bir liste (`IEnumerable`) ise içindeki elemanları virgülle ayırarak şık bir dizi formatına `[A, B, C]` dönüştürür.


## 📊 Test Sonuçları (Konsol Çıktısı)
Eski standart reflection metodu (`GetValues`) ile yeni güvenli metodun (`GetValuesU`) karşılaştırmalı test sonuçları aşağıdadır. Eski metodun nasıl çöktüğünü (Exception fırlattığını) ve yeni metodun bunları nasıl başarıyla işlediğini görebilirsiniz:

```text
=== TEST 1: STANDART CLASS (Mükemmel Çalışır) ===
class sonucu base: 'ReflectionGeneric.Personel'
class sonucu U:
Ad = Ahmet
Soyad = Yılmaz
Yas = 30

Ad = Ahmet
Soyad = Yılmaz
Yas = 30
---------------------------------------------------

=== TEST 2: PRIMITIVE / VALUE TYPE - INT (Boş Dönüş Yapar) ===
Sayı sonucu base: '100'
Sayı sonucu U:
 100
Sayı sonucu: '' (Property olmadığı için boş) 
---------------------------------------------------

=== TEST 3: VALUE TYPE / STRUCT - GUID (Boş Dönüş Yapar) ===
Guid sonucu base: '11f998d6-8b19-45bc-a2d5-a025d932deef'
Guid sonucu U:
 11f998d6-8b19-45bc-a2d5-a025d932deef
Guid sonucu: '' (Property olmadığı için boş) 
---------------------------------------------------

=== TEST 4: STRING (Chars[int] İNDEKSLEYİCİSİ YÜZÜNDEN PATLAR!) ===
string sonucu base: 'Merhaba C#'
string sonucu U:
 Merhaba C#
[HATA ALINDI]: TargetParameterCountException - Parameter count mismatch.
---------------------------------------------------

=== TEST 5: NULL NESNE (PATLAR!) ===
null class sonucu base: ''
null class sonucu U:
 null
[HATA ALINDI]: TargetException - Non-static method requires a target.
---------------------------------------------------

=== TEST 6: LIST<STRING> (Item[int] İNDEKSLEYİCİSİ YÜZÜNDEN PATLAR!) ===
string liste sonucu base: 'System.Collections.Generic.List`1[System.String]'
string liste sonucu U:
 [Elma, Armut]
[HATA ALINDI]: TargetParameterCountException - Parameter count mismatch.
---------------------------------------------------

=== TEST 7: LIST<GUID> (Item[int] İNDEKSLEYİCİSİ YÜZÜNDEN PATLAR!) ===
Guid liste sonucu base: 'System.Collections.Generic.List`1[System.Guid]'
Guid liste sonucu U:
'[3d85f185-a3fd-47fd-a73b-ce2731485e66, 525e9eab-5431-4564-a0fa-f021b8151dc8]'
[HATA ALINDI]: TargetParameterCountException - Parameter count mismatch.
---------------------------------------------------

=== TEST 8: NULL LIST<GUID> (PATLAR!) ===
Base Hali :
Base Hali U :
'null'
[HATA ALINDI]: TargetException - Non-static method requires a target.
---------------------------------------------------

