namespace BankSearch.Models
{
	public class KrediKutusu
	{
		//Kullancıya sunulacak olan ihtiyaç kredisi verilerinin toplandığı model:
		public string BankaAdi { get; set; }
		public double FaizTutari { get; set; }
		public double Faiz { get; set; }
		public double AylikTaksit { get; set; }
		public double ToplamOdeme { get; set; }
		public string BasvuruLinki { get; set; }

		public KrediKutusu(string bankaAdi)
		{
			BankaAdi = bankaAdi;
		}
	}
}
