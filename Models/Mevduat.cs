using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSearch.Models
{
	public class Mevduat
	{
		//Veritabanındaki Mevduatlar tablosu için kullandığımız model:
		[Key]
		[Column(Order = 1)]
		public int Id { get; set; }

		public string BankaAdi { get; set; }
		public int MevduatVadeKodu { get; set; }
		public double MevduatFaizi { get; set; }

	}
}
