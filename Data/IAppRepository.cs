using BankSearch.Models;

namespace BankSearch.Data
{
	public interface IAppRepository
	{
		//AppRepository classının interface'i
		//Diğer scope'larda bu interface kullanılarak gerekli işlemler çözümlenecek.
		//Bu interface ise esasında AppRepository classını çalıştıracak.

		void Update<T>(T entity);
		bool SaveAll();

		Kredi GetKrediByBankaAdiAndKrediVadeKodu(string bankaAdi, int krediVadeKodu);
		List<KrediVadesi> GetKrediVadeleri();
		KrediVadesi GetKrediVadesiByKrediVadeKodu(int krediVadeKodu);
		KrediVadesi GetKrediVadesiByAySayisi(int aySayisi);
		List<KrediKutusu> CalculateKrediler(int krediTutari, int krediVadesi);

		Mevduat GetMevduatByBankaAdiAndMevduatVadeKodu(string bankaAdi, int mevduatVadeKodu);
		List<MevduatVadesi> GetMevduatVadeleri();
		MevduatVadesi GetMevduatVadesiByMevduatVadeKodu(int mevduatVadeKodu);
		MevduatVadesi GetMevduatVadesiByGunSayisi(int gunSayisi);
		List<MevduatKutusu> CalculateMevduatlar(int anapara, int mevduatVadesi);

	}
}
