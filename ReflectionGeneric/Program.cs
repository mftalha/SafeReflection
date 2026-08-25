using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ReflectionGeneric
{

    internal class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("=== TEST 1: STANDART CLASS (Mükemmel Çalışır) ===\n");
            Personel p = new Personel { Ad = "Ahmet", Soyad = "Yılmaz", Yas = 30 };
            Console.WriteLine($"class sonucu base: '{p}'\n");
            Console.WriteLine($"class sonucu U: \n{p.GetValuesU()}\n");
            Console.WriteLine(p.GetValues());

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 2: PRIMITIVE / VALUE TYPE - INT (Boş Dönüş Yapar) ===\n");
            int sayi = 100;
            Console.WriteLine($"Sayı sonucu base: '{sayi}' \n");
            Console.WriteLine($"Sayı sonucu U: \n {sayi.GetValuesU()} \n");
            Console.WriteLine($"Sayı sonucu: '{sayi.GetValues()}' (Property olmadığı için boş) ");

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 3: VALUE TYPE / STRUCT - GUID (Boş Dönüş Yapar) ===\n");
            Guid id = Guid.NewGuid();
            Console.WriteLine($"Guid sonucu base: '{id}' \n");
            Console.WriteLine($"Guid sonucu U: \n {id.GetValuesU()} \n");
            Console.WriteLine($"Guid sonucu: '{id.GetValues()}' (Property olmadığı için boş) ");

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 4: STRING (Chars[int] İNDEKSLEYİCİSİ YÜZÜNDEN PATLAR!) ===\n");
            try
            {
                string metin = "Merhaba C#";
                Console.WriteLine($"string sonucu base: '{metin}' \n");
                Console.WriteLine($"string sonucu U: \n {metin.GetValuesU()} \n");
                Console.WriteLine(metin.GetValues());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA ALINDI]: {ex.GetType().Name} - {ex.Message}");
            }

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 5: NULL NESNE (PATLAR!) ===\n");
            try
            {
                Personel bosPersonel = null;
                Console.WriteLine($"null class sonucu base: '{bosPersonel}' \n");
                Console.WriteLine($"null class sonucu U: \n {bosPersonel.GetValuesU()} \n");
                Console.WriteLine(bosPersonel.GetValues());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA ALINDI]: {ex.GetType().Name} - {ex.Message}");
            }

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 6: LIST<STRING> (Item[int] İNDEKSLEYİCİSİ YÜZÜNDEN PATLAR!) ===\n");
            try
            {
                List<string> liste = new List<string> { "Elma", "Armut" };
                Console.WriteLine($"string liste sonucu base: '{liste}' \n");
                Console.WriteLine($"string liste sonucu U: \n {liste.GetValuesU()} \n");
                Console.WriteLine(liste.GetValues());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA ALINDI]: {ex.GetType().Name} - {ex.Message}");
            }

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 7: LIST<GUID> (Item[int] İNDEKSLEYİCİSİ YÜZÜNDEN PATLAR!) ===\n");
            try
            {
                List<Guid> guidListesi = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
                Console.WriteLine($"Guid liste sonucu base: '{guidListesi}' \n");
                Console.WriteLine($"Guid liste sonucu U: \n'{guidListesi.GetValuesU()}' \n");
                Console.WriteLine(guidListesi.GetValues());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA ALINDI]: {ex.GetType().Name} - {ex.Message}");
            }

            Console.WriteLine("\n---------------------------------------------------\n");

            Console.WriteLine("=== TEST 8: NULL LIST<GUID> (PATLAR!) ===");
            List<Guid> nullGuidListesi = null;
            
            try
            {
                Console.WriteLine($"Base Hali : {nullGuidListesi}\n");
                Console.WriteLine($"Base Hali U : \n'{nullGuidListesi.GetValuesU()}' \n");
                Console.WriteLine(nullGuidListesi.GetValues());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HATA ALINDI]: {ex.GetType().Name} - {ex.Message}\n");
            }

            /// int list
            /// 
            Console.WriteLine("\n---------------------------------------------------\n");
            Console.WriteLine("\nint list\n");
            List<int> sayilar = new List<int> { 10, 20, 30, 40 };
            Console.WriteLine(sayilar.GetValuesU());

            Console.ReadLine();
        }
    }

    // Extension Metodumuzun Durduğu Sınıf
    public static class HelperExtensions
    {
        
        // sadece class ları karşılayan method : altta tüm değişkenleri ve null durumları yöneten method ile değiştirildi.
        public static string GetValues<T>(this T value)
        {
            StringBuilder sb = new StringBuilder();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                switch (prop.PropertyType.Name)
                {
                    default:
                        sb.AppendFormat("{0} = {1}\n", prop.Name, prop.GetValue(value, null));
                        break;
                }
            }
            return sb.ToString();
        }


        /// <summary>
        /// Gönderilen herhangi bir nesnenin (Class, List, Primitive, Guid, String vb.) 
        /// değerlerini hatasız ve güvenli bir şekilde metin (string) formatına dönüştürür.
        /// Null kontrolleri, liste ve indeksleyici (Indexer) patlamalarına karşı korumalıdır.
        /// Loglama ve debug süreçleri için kullanılır.
        /// </summary>
        /// <typeparam name="T">İşlenecek nesnenin jenerik tipi.</typeparam>
        /// <param name="value">String/Log formatına dönüştürülecek nesne.</param>
        /// <returns>Okunan değerlerin biçimlendirilmiş metin karşılığı.</returns>
        public static string GetValuesU<T>(this T value)
        {
            // 1. GÜVENLİK: Nesne null ise anında dön (Tüm null patlamalarını engeller)
            if (value == null)
                return "null";

            Type type = typeof(T);

            // 2. KISA YOL: Eğer gelen değer zaten String, Guid, int, datetime gibi düz bir tipse...
            // Gidip de "Length = 10" gibi saçma özelliklerini basma, direkt değerin kendisini ver.
            if (type.IsPrimitive || type.IsValueType || type == typeof(string))
            {
                // Not: Struct class'ların (kendi yazdığın structlar) property'leri basılsın 
                // diyorsan burayı sadece string ve IsPrimitive olarak da kısıtlayabilirsin.
                // Ama loglamada genelde düz Guid, int, string direkt toString() ile basılır.
                if (type == typeof(string) || type == typeof(Guid) || type.IsPrimitive || type == typeof(DateTime) || type == typeof(decimal))
                {
                    return value.ToString();
                }
            }

            // 3. LİSTE ÇÖZÜMÜ: Eğer gelen nesne bir Liste ise (List<Guid>, List<string> vb.)
            // Property aramak yerine listenin içindeki elemanları arasına virgül koyarak yaz.
            if (value is IEnumerable list)
            {
                List<string> items = new List<string>();
                foreach (var item in list)
                {
                    items.Add(item != null ? item.ToString() : "null");
                }
                return "[" + string.Join(", ", items) + "]";
            }

            // 4. STANDART CLASS'LAR: Asıl Reflection işleminin yapıldığı yer (Personel, Urun vb.)
            StringBuilder sb = new StringBuilder();
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                // GÜVENLİK: Eğer bu özellik indeksleyici ise (Listelerde Item[int], string'de Chars[int]) ATLA!
                if (prop.GetIndexParameters().Length > 0)
                    continue;

                try
                {
                    var val = prop.GetValue(value, null);
                    sb.AppendFormat("{0} = {1}\n", prop.Name, val ?? "null");
                }
                catch
                {
                    // Okunamayan bir özellik olursa sistemi çökertmek yerine loga hata yaz.
                    sb.AppendFormat("{0} = [Değer Okunamadı]\n", prop.Name);
                }
            }

            return sb.ToString();
        }

    }

    // Test İçin Düz Bir Class
    public class Personel
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public int Yas { get; set; }
    }


}
