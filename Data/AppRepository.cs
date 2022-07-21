using BankSearch.Models;
using Microsoft.EntityFrameworkCore;

namespace BankSearch.Data
{
	public class AppRepository : IAppRepository
	{

		//Veritabanındaki işlemlerin ve hesaplama işlemlerinin gerçeklendiği sınıf
		//Bu sınıfta EntityFramework'ün sağladığı pek çok fayda ile birlikte az kod ile çok iş gerçekleyebiliyor ve
		//Code First mimarisinin avantajlarından faydalanabiliyoruz

		private BankSearchContext _context;

		public AppRepository(BankSearchContext context)
		{
			_context = context;
		}

		public void Update<T>(T entity)
		{
			var changedEntity = _context.Entry(entity);
			changedEntity.State = EntityState.Modified;
		}
		public bool SaveAll()
		{
			return _context.SaveChanges() > 0; //herhangi bir değişiklik yapıldıysa(ekle/güncelle/sil) değişiklikleri kaydet
		}

		public List<KrediVadesi> GetKrediVadeleri()
		{
			List<KrediVadesi> krediVadeleri = _context.KrediVadeleri.ToList();
			return krediVadeleri;
		}
		public Kredi GetKrediByBankaAdiAndKrediVadeKodu(string bankaAdi, int krediVadeKodu)
		{
			//Veritabanındaki kabullerimize göre her bankanın her vadeye göre kaydı yalnızca 1 tane olabilir.
			//Bu sebeple böyle bir sorguyla FirstOrDefault fonksiyonu ile birlikte belirli bir kaydı çekebiliyoruz.
			Kredi kredi = _context.Krediler.FirstOrDefault(k => (k.BankaAdi == bankaAdi) && (k.KrediVadeKodu == krediVadeKodu));
			return kredi;
		}
		public KrediVadesi GetKrediVadesiByKrediVadeKodu(int krediVadeKodu)
		{
			return _context.KrediVadeleri.FirstOrDefault(v => v.KrediVadeKodu == krediVadeKodu);
		}
		public KrediVadesi GetKrediVadesiByAySayisi(int aySayisi)
		{
			return _context.KrediVadeleri.FirstOrDefault(v => v.AySayisi == aySayisi);
		}
		public List<KrediKutusu> CalculateKrediler(int krediTutari, int krediVadesi)
		{
			//Kullanıcıya sunulacak olan ihtiyaç kredisi hesaplamalarını yapan metod
			//Bu hesaplamalara vergiler dahil edilmiştir.(Örnek:KKDF)

			List<KrediKutusu> krediKutulari = new List<KrediKutusu>();
			KrediKutusu krediKutusuGaranti=new KrediKutusu("Garanti Bankası");
			KrediKutusu krediKutusuAkbank = new KrediKutusu("Akbank");
			KrediKutusu krediKutusuHalkbank = new KrediKutusu("Halkbank");
			double netFaizOrani;

			krediKutusuGaranti.Faiz = GetKrediByBankaAdiAndKrediVadeKodu("Garanti Bankası", GetKrediVadesiByAySayisi(krediVadesi).KrediVadeKodu).KrediFaizi;
			netFaizOrani = krediKutusuGaranti.Faiz / 100 * (1 + 0.25);
			krediKutusuGaranti.AylikTaksit = Math.Round(krediTutari * netFaizOrani / (1 - Math.Pow(1 / (1 + netFaizOrani), krediVadesi)), 2);
			krediKutusuGaranti.ToplamOdeme = Math.Round(krediKutusuGaranti.AylikTaksit * krediVadesi, 2);
			krediKutusuGaranti.FaizTutari = Math.Round(krediKutusuGaranti.ToplamOdeme - krediTutari, 2);
			krediKutusuGaranti.BasvuruLinki = "https://www.garantibbva.com.tr/krediler/kredi-basvurusu";

			krediKutusuAkbank.Faiz = GetKrediByBankaAdiAndKrediVadeKodu("Akbank", GetKrediVadesiByAySayisi(krediVadesi).KrediVadeKodu).KrediFaizi;
			netFaizOrani = krediKutusuAkbank.Faiz / 100 * (1 + 0.25);
			krediKutusuAkbank.AylikTaksit = Math.Round(krediTutari * netFaizOrani / (1 - Math.Pow(1 / (1 + netFaizOrani), krediVadesi)), 2);
			krediKutusuAkbank.ToplamOdeme = Math.Round(krediKutusuAkbank.AylikTaksit * krediVadesi, 2);
			krediKutusuAkbank.FaizTutari = Math.Round(krediKutusuAkbank.ToplamOdeme - krediTutari, 2);
			krediKutusuAkbank.BasvuruLinki= "https://internetsubesi.akbank.com/WebApplication.UI/entrypoint.aspx?lang=tr-TR&to=9042";

			krediKutusuHalkbank.Faiz = GetKrediByBankaAdiAndKrediVadeKodu("Halkbank", GetKrediVadesiByAySayisi(krediVadesi).KrediVadeKodu).KrediFaizi;
			netFaizOrani = krediKutusuHalkbank.Faiz / 100 * (1 + 0.25);
			krediKutusuHalkbank.AylikTaksit = Math.Round(krediTutari * netFaizOrani / (1 - Math.Pow(1 / (1 + netFaizOrani), krediVadesi)), 2);
			krediKutusuHalkbank.ToplamOdeme = Math.Round(krediKutusuHalkbank.AylikTaksit * krediVadesi, 2);
			krediKutusuHalkbank.FaizTutari = Math.Round(krediKutusuHalkbank.ToplamOdeme - krediTutari, 2);
			krediKutusuHalkbank.BasvuruLinki = "https://www.halkbank.com.tr/tr/bireysel/basvurular/kredi-basvurulari/ihtiyac-kredisi.html";

			krediKutulari.Add(krediKutusuGaranti);
			krediKutulari.Add(krediKutusuAkbank);
			krediKutulari.Add(krediKutusuHalkbank);
			return krediKutulari;
		}

		public List<MevduatVadesi> GetMevduatVadeleri()
		{
			List<MevduatVadesi> mevduatVadeleri = _context.MevduatVadeleri.ToList();
			return mevduatVadeleri;
		}
		public Mevduat GetMevduatByBankaAdiAndMevduatVadeKodu(string bankaAdi, int mevduatVadeKodu)
		{
			//Veritabanındaki kabullerimize göre her bankanın her vadeye göre kaydı yalnızca 1 tane olabilir.
			//Bu sebeple böyle bir sorguyla FirstOrDefault fonksiyonu ile birlikte belirli bir kaydı çekebiliyoruz.
			Mevduat mevduat = _context.Mevduatlar.FirstOrDefault(m => (m.BankaAdi == bankaAdi) && (m.MevduatVadeKodu == mevduatVadeKodu));
			return mevduat;
		}
		public MevduatVadesi GetMevduatVadesiByMevduatVadeKodu(int mevduatVadeKodu)
		{
			MevduatVadesi mevduatVadesi = _context.MevduatVadeleri.FirstOrDefault(m => m.MevduatVadeKodu == mevduatVadeKodu);
			return mevduatVadesi;
		}
		public MevduatVadesi GetMevduatVadesiByGunSayisi(int gunSayisi)
		{
			return _context.MevduatVadeleri.FirstOrDefault(m => m.GunSayisi == gunSayisi);
		}
		public List<MevduatKutusu> CalculateMevduatlar(int anapara, int mevduatVadesi)
		{
			//Kullanıcıya sunulacak olan vadeli mevduat hesaplamalarını yapan metod
			//Bu hesaplamalara vergiler dahil edilmiştir.(Örnek:Stopaj)

			List<MevduatKutusu> mevduatKutulari = new List<MevduatKutusu>();

			MevduatKutusu mevduatKutusuVakifbank = new MevduatKutusu("Vakıfbank");
			MevduatKutusu mevduatKutusuHalkbank = new MevduatKutusu("Halkbank");
			MevduatKutusu mevduatKutusuIsbank = new MevduatKutusu("İş Bankası");
			double brutKazanc;

			mevduatKutusuVakifbank.Faiz = GetMevduatByBankaAdiAndMevduatVadeKodu(
					mevduatKutusuVakifbank.BankaAdi, GetMevduatVadesiByGunSayisi(mevduatVadesi).MevduatVadeKodu).MevduatFaizi;
			brutKazanc = Math.Round(anapara * mevduatKutusuVakifbank.Faiz * mevduatVadesi / 36500, 2);
			mevduatKutusuVakifbank.NetKazanc= Math.Round(brutKazanc - (brutKazanc * 0.05), 2);
			mevduatKutusuVakifbank.VadeSonuTutar = anapara + mevduatKutusuVakifbank.NetKazanc;
			mevduatKutusuVakifbank.BasvuruLinki = "https://subesiz.vakifbank.com.tr/bireysel/tr/login/sifre";

			mevduatKutusuHalkbank.Faiz = GetMevduatByBankaAdiAndMevduatVadeKodu(
					mevduatKutusuHalkbank.BankaAdi, GetMevduatVadesiByGunSayisi(mevduatVadesi).MevduatVadeKodu).MevduatFaizi;
			brutKazanc = Math.Round(anapara * mevduatKutusuHalkbank.Faiz * mevduatVadesi / 36500, 2);
			mevduatKutusuHalkbank.NetKazanc = Math.Round(brutKazanc - (brutKazanc * 0.05), 2);
			mevduatKutusuHalkbank.VadeSonuTutar = anapara + mevduatKutusuHalkbank.NetKazanc;
			mevduatKutusuHalkbank.BasvuruLinki = "https://www.halkbank.com.tr/tr/bireysel/basvurular/diger-basvurular/mevduat-talep-formu.html";

			mevduatKutusuIsbank.Faiz = GetMevduatByBankaAdiAndMevduatVadeKodu(
					mevduatKutusuIsbank.BankaAdi, GetMevduatVadesiByGunSayisi(mevduatVadesi).MevduatVadeKodu).MevduatFaizi;
			brutKazanc = Math.Round(anapara * mevduatKutusuIsbank.Faiz * mevduatVadesi / 36500, 2);
			mevduatKutusuIsbank.NetKazanc = Math.Round(brutKazanc - (brutKazanc * 0.05), 2);
			mevduatKutusuIsbank.VadeSonuTutar = anapara + mevduatKutusuIsbank.NetKazanc;
			mevduatKutusuIsbank.BasvuruLinki = "https://www.isbank.com.tr/Internet/index.aspx?iscepnavid=ACNAOpenDepositAccountMenu&campaignid=vada&CampTxCode=OpenDepositAccount.OpenDepositAccount.Steps.OpenDepositAccountMenuRequest";

			mevduatKutulari.Add(mevduatKutusuVakifbank);
			mevduatKutulari.Add(mevduatKutusuHalkbank);
			mevduatKutulari.Add(mevduatKutusuIsbank);

			return mevduatKutulari;
		}
	}
}
