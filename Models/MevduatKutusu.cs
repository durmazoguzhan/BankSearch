namespace BankSearch.Models
{
	public class MevduatKutusu
	{
		//Kullancıya sunulacak olan vadeli mevduat verilerinin toplandığı model:
		public string BankaAdi { get; set; }
		public double NetKazanc { get; set; }
		public double Faiz { get; set; }
		public double VadeSonuTutar { get; set; }
		public string BasvuruLinki { get; set; }

		public MevduatKutusu(string bankaAdi)
		{
			BankaAdi = bankaAdi;
		}
	}
}
