using RestSharp;
using Newtonsoft.Json.Linq;
using BankSearch.Data;
using BankSearch.Models;

namespace BankSearch.BackgroundServices
{
	public class APIService : BackgroundService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger<APIService> _logger;

		//BackgroundService'den AppRepository'e erişebilmek ve bir scope'da işlem yapabilmek için IServiceProvider'a ihtiyacımız var
		public APIService(IServiceProvider serviceProvider, ILogger<APIService> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				UpdateKrediFaizleri(GetKrediFaizleri());
				_logger.LogInformation("Updated Consumer Loan Interest Rates! Time: {time}", DateTimeOffset.Now);
				UpdateMevduatFaizleri(GetMevduatFaizleri());
				_logger.LogInformation("Updated Term Deposit Interest Rates! Time: {time}", DateTimeOffset.Now);
				await Task.Delay(10800000, stoppingToken); //3 saatte bir çalışıyor.
			}
		}

		private IRestResponse GetKrediFaizleri()
		{
			//Garanti bankasından REST API ile JSON formatında verileri çekiyoruz.
			//Not: Şuan için API planı SandBox'tır. Dolayısıyla gerçek olmayan, test verileri çekmektedir.
			//API Planı Production planına geçirildiğinde bu kodlarda bir değişikliğe gerek olmadan gerçek ve güncel veriler çekilebilecektir.
			string uri = "https://apis.garantibbva.com.tr:443/loans/v1/paymentPlan";
			var client = new RestClient(uri);
			var request = new RestRequest(Method.POST);
			request.AddHeader("apikey", "l7xx8af86c14ea7e44e0ab3fbfcc6137ae09");
			request.AddQueryParameter("loanType", "1");
			request.AddQueryParameter("campaignCode", "BankSearch");
			request.AddQueryParameter("loanAmount", "10000");
			IRestResponse response = client.Execute(request);
			return response;
		}

		private void UpdateKrediFaizleri(IRestResponse response)
		{
			//Her vade kodu için bu işlemler tekrar edilerek veritabanındaki Kredi Faizleri güncellenecektir.
			for (int krediVadeKodu = 1; krediVadeKodu <= 8; krediVadeKodu++)
			{
				//AppRepository'e erişebilmek için ve GarbageCollector'u beklemeden işimiz bittiğinde Dispose işlemini gerçekleyerek
				//programın daha hafif bir şekilde çalışabilmesini sağlayacak olan using yapısı kullanılarak işlemler burada gerçekleştirilmiştir.
				using (IServiceScope scope = _serviceProvider.CreateScope())
				{
					//ServiceProvider yardımıyla IAppRepository scopeunun dahil edilmesi
					IAppRepository appRepository =
						scope.ServiceProvider.GetRequiredService<IAppRepository>();

					int aySayisi = appRepository.GetKrediVadesiByKrediVadeKodu(krediVadeKodu).AySayisi;

					//Faiz oranının JSON formatındaki veriden sorgularla çekilmesi
					double krediFaizi = JObject.Parse(response.Content)["data"]["list"]
				.Where(p => (int)p["dueNum"] == aySayisi)
				.Select(p => (double)p["monthlyAnnualCostRate"])
				.FirstOrDefault();
					//Verilerin Kredi nesnesi oluşturularak güncellenmesi
					Kredi krediGaranti = appRepository.GetKrediByBankaAdiAndKrediVadeKodu("Garanti Bankası", krediVadeKodu);
					krediGaranti.KrediFaizi = Math.Round(krediFaizi,2);

					//Not: Akbank ve Halkbank'ın API sistemleri şuan için doğru bir şekilde hizmet verememekte. Bu sebeple Garanti Bankası'nın
					//verileri çekilerek Akbank ve Halkbank için dummy veriler oluşturulmuştur.
					Kredi krediAkbank = appRepository.GetKrediByBankaAdiAndKrediVadeKodu("Akbank", krediVadeKodu);
					krediAkbank.KrediFaizi = Math.Round(krediFaizi+0.2,2);
					Kredi krediHalkbank = appRepository.GetKrediByBankaAdiAndKrediVadeKodu("Halkbank", krediVadeKodu);
					krediHalkbank.KrediFaizi = Math.Round(krediFaizi+0.4,2);
					appRepository.Update(krediGaranti);
					appRepository.Update(krediAkbank);
					appRepository.Update(krediHalkbank);
					appRepository.SaveAll();
				}
			}
		}

		private void GetToken()
		{
			//Vakıfbank'ın API'ı, Oauth 2.0 yetkilendirmesini kullanmaktadır. Bu sebeple böyle bir işlemle bir token alınarak sonrasında
			//istenilen API'lerin GET,POST sorguları gerçekleştirilecektir.
			//Not: VakıfBank API'ı şuan için teknik sorunlara sahip ve Vakıfbank yazılımcılarının sorunları çözmesi bekleniyor.
			//Bu sebeple Token oluşturulamıyor. Bu scope VakıfBank'ın API sorunlarını çözdüğünde kod değişikliğine ihtiyaç duyulmadan çalışacaktır.
			string uri = "https://apigw.vakifbank.com.tr:8443/auth/oauth/v2/token";
			var client = new RestClient(uri);
			var request = new RestRequest(Method.POST);
			request.AddQueryParameter("client_id", "l7xx54b9f8e06a97469492ada7efa829f9eb");
			request.AddQueryParameter("client_secret", "de5839fabbfc40a582589d3479481a0e");
			request.AddQueryParameter("grant_type", "client_credentials");
			request.AddQueryParameter("scope", "public");
			IRestResponse response = client.Execute(request);
			_logger.LogInformation(response.Content);
		}

		private IRestResponse GetMevduatFaizleri()
		{
			//Not: Vakıfbank API'ındaki teknik sorunlar sebebiyle bu scope şuan doğru bir şekilde çalışmamaktadır.
			// Teknik ekibin sorunları çözmesi halinde bu metod kod güncellemesi ihtiyacı olmadan çalışabilecektir.
			string uri = "https://apigw.vakifbank.com.tr:8443/timeDepositRates";
			var client = new RestClient(uri);
			var request = new RestRequest(Method.POST);
			request.AddHeader("apikey", "l7xx54b9f8e06a97469492ada7efa829f9eb");
			request.AddHeader("apisecret", "de5839fabbfc40a582589d3479481a0e");
			request.AddQueryParameter("CurrencyCode", "TL");
			request.AddQueryParameter("ProductCode", "55500485");
			IRestResponse response = client.Execute(request);
			_logger.LogInformation(response.Content);
			return response;
		}

		private void UpdateMevduatFaizleri(IRestResponse response)
		{
			//Not: Vakıfbank API'larındaki sorunlar nedeniyle bu metot şuan mevduatFaizi ismiyle tanımlanan değişkenle dummy veri şeklinde
			//çalışmaktadır. Teknik ekibin sorunları düzeltmesi halinde bu metodda kod değişiklikleri gerekmektedir.
			//Vadeli Mevduat işlemleri ileriye dönük projenin güncelleme basamağı olarak bırakılmak zorunda kalmıştır.
			double mevduatFaizi = 12.0;
			for (int mevduatVadeKodu = 1; mevduatVadeKodu <= 5; mevduatVadeKodu++)
			{

				//AppRepository'e erişebilmek için ve GarbageCollector'u beklemeden işimiz bittiğinde Dispose işlemini gerçekleyerek
				//programın daha hafif bir şekilde çalışabilmesini sağlayacak olan using yapısı kullanılarak işlemler burada gerçekleştirilmiştir.
				using (IServiceScope scope = _serviceProvider.CreateScope())
				{
					//ServiceProvider yardımıyla IAppRepository scopeunun dahil edilmesi
					IAppRepository appRepository =
						scope.ServiceProvider.GetRequiredService<IAppRepository>();

					int gunSayisi = appRepository.GetMevduatVadesiByMevduatVadeKodu(mevduatVadeKodu).GunSayisi;

					//Verilerin Mevduat nesnesi oluşturularak güncellenmesi

					//Not: İş Bankası ve Halkbank'ın API sistemleri şuan için doğru bir şekilde hizmet verememekte. Bu sebeple
					//VakıfBank'ın veri çekme metotları hazırlanarak VakıfBank için gerçek veri temeli, diğer 2 banka için de
					//dummy veri temeli oluşturulmuştur.
					Mevduat mevduatVakifbank = appRepository.GetMevduatByBankaAdiAndMevduatVadeKodu("Vakıfbank", mevduatVadeKodu);
					mevduatVakifbank.MevduatFaizi = Math.Round(mevduatFaizi, 2);
					Mevduat mevduatHalkbank = appRepository.GetMevduatByBankaAdiAndMevduatVadeKodu("Halkbank", mevduatVadeKodu);
					mevduatHalkbank.MevduatFaizi = Math.Round(mevduatFaizi+0.2, 2);
					Mevduat mevduatIsBankasi = appRepository.GetMevduatByBankaAdiAndMevduatVadeKodu("İş Bankası", mevduatVadeKodu);
					mevduatIsBankasi.MevduatFaizi = Math.Round(mevduatFaizi+0.4, 2);
					appRepository.Update(mevduatVakifbank);
					appRepository.Update(mevduatHalkbank);
					appRepository.Update(mevduatIsBankasi);
					appRepository.SaveAll();

					mevduatFaizi++;
				}
			}
		}
	}
}