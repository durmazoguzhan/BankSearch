using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSearch.Models
{
	
	public class Kredi
	{
		//Veritabanındaki Krediler tablosu için kullandığımız model:
		[Key]
		[Column(Order = 1)]
		public int Id { get; set; }

		public string BankaAdi { get; set; }
		public int KrediVadeKodu { get; set; }
		public double KrediFaizi { get; set; }

		public Kredi(string bankaAdi, int krediVadeKodu, double krediFaizi)
		{
			BankaAdi = bankaAdi;
			KrediVadeKodu = krediVadeKodu;
			KrediFaizi = krediFaizi;
		}

		public Kredi(int id,string bankaAdi, int krediVadeKodu, double krediFaizi)
		{
			Id = id;
			BankaAdi = bankaAdi;
			KrediVadeKodu = krediVadeKodu;
			KrediFaizi = krediFaizi;
		}
	}
}
