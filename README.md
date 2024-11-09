![image](https://github.com/user-attachments/assets/a7d8f8ba-bc52-4cff-91d1-bc62bb39631c)


# Event Store Nedir?

Event Store, Event Sourcing için geliştirilmiş olan event’leri depolamamızı sağlayan ve bununla birlikte ‘Read/Query’ veritabanlarına kayıt atabilmek için dahili bir ‘Message Broker’ hizmeti sağlayan Open Source bir veritabanıdır. HTTP ve TCP protokollerini desteklemektedir.

## Event Sourcing’in Temeli
Event Sourcing, uygulamanızdaki her bir değişikliği olay (event) olarak saklama yaklaşımıdır. Birçok geleneksel sistem, son durumu (state) veritabanında saklar. Event Sourcing’de, bu son durumu elde etmek için tüm olayları sırayla uygulayarak elde edersiniz.

## Event Sourcing’in Avantajları

- Geçmiş Değişiklikleri İzleme: Tüm değişiklikler olay olarak saklandığından, sistemdeki her değişiklik izlenebilir.

- Esneklik: Olayları yeniden uygulayarak farklı durumları simüle edebilirsiniz.

- Kolay Geri Alma: Hatalı bir değişiklik yapıldığında, bu değişikliği geri alma veya o değişikliğe kadar olan durumu geri yükleme imkanı sağlar.


#

![image](https://github.com/user-attachments/assets/352e8fd4-6eb0-4279-8295-42015fa98f18)
