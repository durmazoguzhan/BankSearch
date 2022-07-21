using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankSearch.Models
{
	public class KrediVadesi
	{
		//Veritabanındaki KrediVadeleri tablosu için kullandığımız model:
		[Key]
		[Column(Order = 1)]
		public int KrediVadeKodu { get; set; }

		public int AySayisi { get; set; }

	}
}
