using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSearch.Models
{
	public class MevduatVadesi
	{
		//Veritabanındaki MevduatVadeleri tablosu için kullandığımız model:
		[Key]
		[Column(Order = 1)]
		public int MevduatVadeKodu { get; set; }

		public int GunSayisi { get; set; }

	}
}
